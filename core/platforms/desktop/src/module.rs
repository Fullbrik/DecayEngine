//We start off by defining the platform dependent library names
#[cfg(target_os = "linux")]
const LIB_NAME: &str = "libmodule.so";

#[cfg(target_os = "macos")]
const LIB_NAME: &str = "libmodule.dylib";

#[cfg(target_os = "windows")]
const LIB_NAME: &str = "Module.dll";

//Import any headers
#[path = ".std/cross_module.rs"]
pub mod cross_module;

//And then importing everything
extern crate libloading as lib;
pub use cross_module::Packet;
use std::ffi::CString;
use std::fs::File;
use std::io::prelude::*;
use std::os::raw::c_char;
use std::path::{Path, PathBuf};
use std::str::FromStr;
use std::{collections::HashMap, mem};

//This is the signature of the Event Pointers used in cross-module communication
type EventPtr = unsafe extern "C" fn();

pub struct Module {
    #[allow(dead_code)]
    name: String, //The name of the module

    #[allow(dead_code)] //This might help with lifetime stuff so I'm leaving it in
    library: lib::Library,
    on_load: Box<dyn Fn()>,   //Called when we load the module
    on_init: Box<dyn Fn()>,   //Called after all initial modules are initialized
    on_unload: Box<dyn Fn()>, //Called when we unload the module
    get_event_ptr: Box<dyn Fn(*const c_char) -> EventPtr>,
    receive_packet: Box<dyn Fn(*const u8, usize)>,
}

impl Module {
    fn new(name: String, lib_path: PathBuf) -> Module {
        unsafe {
            //Unsafe because we are loading stuff from libraries potentially written in C.
            let library = lib::Library::new(lib_path.as_os_str()).expect(
                format!(
                    "Failed top load module library: {}",
                    lib_path
                        .to_str()
                        .expect("Error getting path for module library")
                )
                .as_str(),
            );

            //Get all of our module's functions
            //Check the comments on the Module struct for what these functions do
            let on_load = library
                .get::<unsafe extern "C" fn()>(b"module_on_load")
                .expect("Failed to get function module_on_load")
                .into_raw();

            let on_init = library
                .get::<unsafe extern "C" fn()>(b"module_on_init")
                .expect("Failed to get function module_on_init")
                .into_raw();

            let on_unload = library
                .get::<unsafe extern "C" fn()>(b"module_on_unload")
                .expect("Failed to get function module_on_unload")
                .into_raw();

            let get_event_ptr = library
                .get::<unsafe extern "C" fn(name: *const c_char) -> EventPtr>(
                    b"module_get_event_ptr",
                )
                .expect("Failed to get function module_get_event_ptr")
                .into_raw();

            let receive_packet = library
                .get::<unsafe extern "C" fn(data: *const u8, size: usize)>(b"module_receive_packet")
                .expect("Failed to get function module_receive_packet")
                .into_raw();

            //Return back the module
            return Module {
                name: name,
                library: library,
                on_load: Box::new(move || on_load()),
                on_init: Box::new(move || on_init()),
                on_unload: Box::new(move || on_unload()),
                get_event_ptr: Box::new(move |name| get_event_ptr(name)),
                receive_packet: Box::new(move |data, size| receive_packet(data, size)),
            };
        }
    }

    pub fn get_event_ptr(&self, name: &str) -> impl Fn() {
        //Get the event function pointer and return it
        let c_string = CString::new(name).unwrap();
        let ptr = (*self.get_event_ptr)(c_string.as_ptr());
        unsafe {
            //Return a closure because it makes life easier for me. If you can find a better way, pls send a pull request.
            return move || ptr();
        }
    }

    pub fn send_packet(&self, packet: Packet) {
        let data = packet.get_data();
        let size = data.len();
        let raw_data = data.as_ptr();

        mem::forget(packet.data);

        (*self.receive_packet)(raw_data, size);
    }
}

pub struct ModuleManager {
    module_paths: HashMap<String, String>, //All of the paths to different modules. This is so we can load a module from its name
    pub modules: HashMap<String, Module>,  //All of the loaded modules

    can_init: bool, //If all initial modules are loaded, we can init a module right after we load it
}

impl ModuleManager {
    pub fn new() -> ModuleManager {
        ModuleManager {
            module_paths: HashMap::new(),
            modules: HashMap::new(),
            can_init: false,
        }
    }

    pub fn add_modules_from_json(&mut self, json: &str, load_any: bool) {
        //Parse the json
        let value: serde_json::Value = serde_json::from_str(
            //Replace the DECAY environment variable
            json.replace(
                "$DECAY",
                std::env::var("DECAY")
                    .expect("No decay environment variable")
                    .as_str(),
            )
            .as_str(),
        )
        .expect("Parsing module json has errors");

        //Get all the paths
        let paths = value
            .get("paths")
            .expect("Couldn't find any paths to load modules from");

        //Go through each path and add it to the map
        for (n, p) in paths.as_object().unwrap() {
            self.add_to_paths(n.to_string(), p.as_str().unwrap().to_string());
        }

        //If we should load modules that the JSON asks us to, we do so
        if load_any {
            let loaded = value
                .get("loaded")
                .expect("Couldn't find any modules to load at start");
            for module in loaded.as_array().unwrap() {
                self.load(module.as_str().unwrap().to_string());
            }
        }
    }

    pub fn add_to_paths(&mut self, name: String, path: String) {
        self.module_paths.insert(name, path); //Add a module to the path list
    }

    pub fn load(&mut self, name: String) {
        let path = self.module_paths.get(&name).unwrap().clone(); //Get the module's path from its name
        self.load_file(path.as_str()); // and load it
    }

    fn load_file(&mut self, file: &str) {
        println!("Loading module file: {}...", file);

        let mut f = File::open(file).expect("File not found"); //Open the module's config (.dmod) file
        let mut text = String::new();

        f.read_to_string(&mut text).expect("Unable to read file"); //and read the text

        //Get the folder the config file
        let path = Path::new(file);
        let mut directory: &str = "";
        if !path.is_dir() {
            directory = path
                .parent()
                .expect("File has no folder or something")
                .to_str()
                .expect("Could not make path a str");
        }

        //load the module
        self.load_json(text.as_str(), directory);
    }

    fn load_json(&mut self, json: &str, directory: &str) {
        let value: serde_json::Value = serde_json::from_str(json).expect("Could not parse to json"); //Parse the json

        let name = value["Name"]
            .as_str()
            .expect("Attempted to load module with no name")
            .to_string(); //Get the module's name

        println!("Loading module: {}...", name);

        let build_folder = value["Folders"]["build"]
            .as_str()
            .expect("Build folder was not a string"); //Get the build folder

        //Get the actual module library itself
        let mut lib_path = PathBuf::from_str(directory).expect("Error creating path buffer");
        lib_path.push(Path::new(build_folder));
        lib_path.push(Path::new(LIB_NAME));

        //Load the module and call on_load (Remember how I said it gets called here. Thats because I wrote the comments Kevin. You can't complain about my code anymore because there are comments in it.)
        let module = Module::new(name.clone(), lib_path);
        self.modules.insert(name.clone(), module);
        (*self.modules.get(&name).unwrap().on_load)();

        //If all other modules are init-ed, init this one
        if self.can_init {
            (*self.modules.get(&name).unwrap().on_init)();
        }

        println!("Loaded module: {}", name);
    }

    pub fn complete_load_initial_modules(&mut self) {
        //If we haven't already ini-ed the other modules
        if !self.can_init {
            println!("Initializing modules...");

            //We do this first because it makes me feel happy
            self.can_init = true;

            //Init all the modules
            for module in self.modules.values() {
                (*module.on_init)();
            }

            println!("Initialized modules");
        }
    }

    pub fn unload(&mut self, name: String) {
        //Get the module
        let module = &self.modules[&name];
        //Call on_unload
        (*(module.on_unload))();
        //And remove it from the list
        self.modules.remove(&name);
    }

    pub fn unload_all(&mut self) {
        //Unload all the modules
        for module in self.modules.values() {
            (*module.on_unload)();
        }

        //And remove them from the list
        self.modules.clear();
    }
}
