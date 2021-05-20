const MODULE_NAME: &str = "Example";

// Occurs when the module gets loaded in to the engine
#[no_mangle]
pub extern "C" fn module_on_load() {
    println!("Module \"{}\" loaded! :)", MODULE_NAME);
}
// Occurs when the module gets initialized. This will always happen after all initial modules are loaded
#[no_mangle]
pub extern "C" fn module_on_init() {
    println!("Module \"{}\" initialized! :)", MODULE_NAME);
}
// Occurs when the module gets unloaded
#[no_mangle]
pub extern "C" fn module_on_unload() {
    println!("Module \"{}\" unloaded! :)", MODULE_NAME);
}
