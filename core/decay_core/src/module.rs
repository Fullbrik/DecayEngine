pub struct Module {}

pub struct ModuleManager {
    modules: Vec<Module>,
}

impl ModuleManager {
    pub fn new() -> ModuleManager {
        ModuleManager {
            modules: Vec::new(),
        }
    }

    pub fn load(path: &str) {}
}
