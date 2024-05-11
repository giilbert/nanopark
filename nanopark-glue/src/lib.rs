use godot::{engine::Engine, prelude::*};

#[derive(GodotClass)]
#[class(init, base=Object)]
struct ControllerInputSingleton {
    base: Base<Object>,
}

#[godot_api]
impl ControllerInputSingleton {
    #[func]
    fn foo(&mut self, number: i32) {
        godot_print!("Hello from Rust! The number is {}", number);
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