This is fnalibs, an archive containing the native libraries used by FNA.

These are the folders included:

- x86: 32-bit Windows
- x64: 64-bit Windows
- lib64: Linux (64-bit only)
- libaarch64: Linux (64-bit ARM only)
- osx: macOS (64-bit only)
- vulkan: MoltenVK ICD for macOS
	- Place this at Game.app/Contents/Resources/vulkan/

The library dependency list is as follows:

- SDL2, used as the platform layer
- FNA3D, used in the Graphics namespace
- FAudio, used in the Audio/Media namespaces
- libtheorafile, only used for VideoPlayer

For x86_64 Linux, the minimum OS requirement is glibc 2.28.
For aarch64 Linux, the minimum OS requirement is glibc 2.34.
