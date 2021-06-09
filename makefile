decay-engine:
	# Clean up first
	rm -rf ./core/platforms/desktop/target fi || true

	rm ./core/platforms/desktop/Cargo.lock || true

	# Copy the engine into the engine folder
	mkdir -p engine
	cp -R core/platforms/desktop/** engine/

	# Make the folder for the std for the engine
	mkdir -p engine/src/.std

	# Build all modules with the build tool
	dotnet run -p ./core/DecayModuleTool/ -- -f ./modules -l -m Engine -r -s -b -c ./engine/modules --copy-std-to-folder ./engine/src/.std

	# Build the engine
	cargo build --manifest-path engine/Cargo.toml

	# Copy the modules
	mkdir -p engine/target/debug/modules/
	cp -R engine/modules/** engine/target/debug/modules/