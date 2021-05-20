#[cfg(target_os = "linux")]
const LIB_NAME: &str = "libmodule.so";

#[cfg(target_os = "macos")]
const LIB_NAME: &str = "libmodule.dylib";

#[cfg(target_os = "windows")]
const LIB_NAME: &str = "Module.dll";

extern crate libloading as lib;

use std::fs::File;
use std::io::prelude::*;
use std::path::{Path, PathBuf};
use std::str::FromStr;

pub struct Module {
    on_load: lib::Symbol<fn()>,
}

impl Module {
    fn new(lib_path: PathBuf) -> Module {
        unsafe {
            let library = lib::Library::new(lib_path.as_os_str()).expect(
                format!(
                    "Failed top load module library: {}",
                    lib_path
                        .to_str()
                        .expect("Error getting path for module library")
                )
                .as_str(),
            );

            let on_load: lib::Symbol<unsafe extern "C" fn()> = library
                .get(b"module_on_load")
                .expect("Failed to get function module_on_load");

            let on_init: lib::Symbol<unsafe extern "C" fn()> = library
                .get(b"module_on_init")
                .expect("Failed to get function module_on_init");

            let on_unload: lib::Symbol<unsafe extern "C" fn()> = library
                .get(b"module_on_unload")
                .expect("Failed to get function module_on_unload");

            return Module { on_load: on_load };
        }
    }
}

pub struct ModuleManager {
    modules: Vec<Module>,
}

impl ModuleManager {
    pub fn new() -> ModuleManager {
        ModuleManager {
            modules: Vec::new(),
        }
    }

    pub fn load_file(&mut self, file: &str) {
        println!("Loading module file: {}", file);

        let mut f = File::open(file).expect("File not found");
        let mut text = String::new();

        f.read_to_string(&mut text).expect("Unable to read file");

        let path = Path::new(file);
        let mut directory: &str = "";
        if !path.is_dir() {
            directory = path
                .parent()
                .expect("File has no folder or something")
                .to_str()
                .expect("Could not make path a str");
        }

        self.load_json(text.as_str(), directory);
    }

    pub fn load_json(&mut self, json: &str, directory: &str) {
        let value: serde_json::Value = serde_json::from_str(json).expect("Could not parse to json");

        println!("Loading module: {}", value["Name"]);

        let buildFolder = value["Folders"]["build"]
            .as_str()
            .expect("Build folder was not a string");

        let mut lib_path = PathBuf::from_str(directory).expect("Error creating path buffer");
        lib_path.push(Path::new(buildFolder));
        lib_path.push(Path::new(LIB_NAME));

        unsafe {
            let module = Module::new(lib_path);
            self.modules.push(module);
        }

        println!("Loaded module: {}", value["Name"]);
    }
}
