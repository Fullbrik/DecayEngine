const PLUGIN_NAME: &str = "Example";

// Occurs when the plugin gets loaded in to the engine
pub fn plugin_on_load() {
    println!("Plugin {} loaded", PLUGIN_NAME);
}
// Occurs when the plugin gets initialized. This will always happen after all initial plugins are loaded
pub fn plugin_on_init() {
    println!("Plugin {} initialized", PLUGIN_NAME);
}
// Occurs when the plugin gets unloaded
pub fn plugin_on_unload() {
    println!("Plugin {} unloaded", PLUGIN_NAME);
}
