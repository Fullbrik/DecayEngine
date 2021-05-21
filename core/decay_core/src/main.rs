mod module;
use module::ModuleManager;

use clap::{App, Arg};

fn main() {
    //Parse the console parameters
    let matches = App::new("Decay Engine")
        .version("0.0.1")
        .author("Mcp613")
        .arg(
            Arg::new("module")
                .about("The module (.dmod) to load")
                .short('m')
                .long("module")
                .value_name("MODULE")
                .takes_value(true),
        )
        .get_matches();

    //Create the Module manager
    let mut manager = ModuleManager::new();
    //And add all our paths from the module.json file and load it. Make sure you added the $DECAY environment variable
    manager.add_modules_from_json(include_str!("../modules.json"), true);

    //We've loaded all our initial modules, so we call this
    manager.complete_load_initial_modules();

    //Quick temporary test to make sure cross-module event pointers can be grabbed and called
    manager
        .modules
        .get("Example")
        .unwrap()
        .get_event_ptr("example")();

    //Unload everything when we're done
    manager.unload_all();
}
