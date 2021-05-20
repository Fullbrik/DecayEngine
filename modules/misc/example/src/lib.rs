const MODULE_NAME: &str = "Example";

// Occurs when the module gets loaded in to the engine
pub fn module_on_load() {
    println!("module {} loaded", MODULE_NAME);
}
// Occurs when the module gets initialized. This will always happen after all initial modules are loaded
pub fn module_on_init() {
    println!("module {} initialized", MODULE_NAME);
}
// Occurs when the module gets unloaded
pub fn module_on_unload() {
    println!("module {} unloaded", MODULE_NAME);
}
