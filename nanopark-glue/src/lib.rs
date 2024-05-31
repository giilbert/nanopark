mod serial;

use godot::{engine::Engine, prelude::*};
use serial::SerialController;

#[derive(GodotClass)]
#[class(base=Object)]
struct ControllerInputSingleton {
    base: Base<Object>,
    serial: SerialController,
}

#[godot_api]
impl ControllerInputSingleton {
    #[func]
    fn list_ports(&self) -> Array<GString> {
        self.serial.list_ports().iter().map(|s| s.into()).collect()
    }

    #[func]
    fn connect(&mut self, port: GString) {
        self.serial.connect(&port.to_string());
    }

    #[func]
    fn poll(&mut self) {
        self.serial.poll();
    }

    #[func]
    fn disconnect(&mut self) {
        self.serial.disconnect();
    }

    #[func]
    fn get_controllers_state(&mut self) -> String {
        serde_json::to_string(&self.serial.state.lock().controllers).expect("error serializing")
    }
}

#[godot_api]
impl IObject for ControllerInputSingleton {
    fn init(base: Base<Object>) -> Self {
        Self {
            serial: SerialController::new(),
            base,
        }
    }
}

struct GlueExtension;

#[gdextension]
unsafe impl ExtensionLibrary for GlueExtension {
    fn on_level_init(level: InitLevel) {
        if level == InitLevel::Scene {
            // The StringName identifies your singleton and can be
            // used later to access it.
            Engine::singleton().register_singleton(
                StringName::from("ControllerInput"),
                ControllerInputSingleton::new_alloc().upcast(),
            );
        }
    }

    fn on_level_deinit(level: InitLevel) {
        if level == InitLevel::Scene {
            // Get the `Engine` instance and `StringName` for your singleton.
            let mut engine = Engine::singleton();
            let singleton_name = StringName::from("ControllerInput");

            // We need to retrieve the pointer to the singleton object,
            // as it has to be freed manually - unregistering singleton
            // doesn't do it automatically.
            let singleton = engine
                .get_singleton(singleton_name.clone())
                .expect("cannot retrieve the singleton");

            // Unregistering singleton and freeing the object itself is needed
            // to avoid memory leaks and warnings, especially for hot reloading.
            engine.unregister_singleton(singleton_name);
            singleton.free();
        }
    }
}
