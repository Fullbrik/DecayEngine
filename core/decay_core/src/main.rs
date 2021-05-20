mod module;
use module::ModuleManager;

use clap::{App, Arg};

fn main() {
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

    let mut mm = ModuleManager::new();
    mm.load_file(matches.value_of("module").expect("Please supply a module"));
}
