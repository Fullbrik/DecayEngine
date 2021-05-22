#[path = ".headers/cross_module.rs"]
pub mod cross_module;

use cross_module::Packet;
use std::{ffi::CStr, mem};
use std::os::raw::c_char;

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
//Get a function pointer given an event name. NOTE: DO NOT MODIFY THIS FUNCTION!! Modify get_func_ptr instead
#[no_mangle]
pub extern "C" fn module_get_event_ptr(name: *const c_char) -> extern "C" fn() {
    unsafe { get_event_ptr(CStr::from_ptr(name).to_str().unwrap()) }
}

//Called when we receive a packet from cross-module communication. NOTE: DO NOT MODIFY THIS FUNCTION!! Modify receive_packet instead
#[no_mangle]
pub extern "C" fn module_receive_packet(data: *mut u8, size: usize) {
    mem::forget(data);
    mem::forget(size);
    receive_packet(Packet::from_data(data, size));
}

//Export your functions here. Make sure each function is prefaced with 'extern "C"' before the definition
fn get_event_ptr(name: &str) -> extern "C" fn() {
    match name {
        "example" => example_event,
        _ => no_event,
    }
}

//For when we call a non-existent event
extern "C" fn no_event() {}

//Just an example event
extern "C" fn example_event() {
    println!("Did example event");
}

//When we receive a packet, we call this
fn receive_packet(mut packet: Packet) {
    println!("{:?}", packet.get_data());
    let first = packet.pull_i8();
    let second = packet.pull_u16();
    println!("{}, {}", first, second);
}
