DOWNLOADED FOR SADCONSOLE ON SEPT 26, 2017
==========================================

This is fnalibs, an archive containing the native libraries used by FNA.

These are the folders included:

- x86: 32-bit Windows
- x64: 64-bit Windows
- lib: 32-bit Linux
- lib64: 64-bit Linux
- osx: macOS Universal (32/64-bit)

The library dependency tree is as follows:

- SDL2, used as the platform layer
- MojoShader, used in the graphics subsystem
- soft_oal/libopenal, used in the audio subsystem
- SDL2_image, only used for Texture2D.FromStream and Texture2D.SaveAsPng
	- libpng*, only used if you work with PNG images
		- zlib, only referenced on Windows
	- libjpeg*, only used if you work with JPEG images
- libvorbisfile, only used for Song
	- libogg
	- libvorbis
- libtheorafile, only used for VideoPlayer
	- libogg
	- libvorbis
	- libtheoradec

* For macOS, libpng and libjpeg are not referenced at all.
