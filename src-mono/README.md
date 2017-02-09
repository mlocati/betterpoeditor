Building and running under Linux
================================

This program can be run under Linux (debian), in the Mono environment.

The mono runtime can be installed with the following command (run it in a terminal):

`sudo apt-get install mono-runtime`

and

`sudo apt-get install mono-reference-assemblies-2.0`

The application must be compiled for mono. For this you have to install the following components:

`sudo apt-get install mono-xbuild`

`sudo apt-get install mono-mcs`

`sudo apt-get install mono-devel`


To build the application simply do this in a terminal:

`cd folder-where-betterpoeditor-was-extracted/`

`cd src-mono/`

`make`

`make install`


Thats all. Once the make process ends, you can launch the application with this command line

`betterpoeditor`

Compiling .po files into .mo files
==================================
You needs the msgfmt utility, part of the gettext package. To install it simply open

`sudo apt-get install gettext`
