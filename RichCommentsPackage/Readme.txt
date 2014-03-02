Overview
~~~~~~~~
This VS Package defines the extension contents and handles basic VS integration, e.g. the enable/disable command.

Remarks
~~~~~~~
All the DLLs and misc. files that are shortcuts are dependencies. They're in this folder because of the way VSIX packaging works. It would be possible to put them somewhere more suitable and run a post-build script to package them manually, but this wouldn't run at the right point during a build for debugging.