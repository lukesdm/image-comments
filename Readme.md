# ImageComments (a Visual Studio Extension)

## Overview
This is an extension for the Visual Studio code editor that allows images to be displayed amongst code, allowing for visually rich comments. For example...

![](http://lukesdm.github.com/image-comments/media/example-1.png)

## Usage Info

### Preamble
Disclaimer: This project is a WIP and it's pretty rough around the edges. Please report issues on the GitHub repo.

Requires: Visual Studio 2013 (Community/Professional or better)

### Download/Installation
[Download](http://github.com/lukesdm/image-comments/raw/master/Output/ImageComments.vsix), then double-click/activate the VSIX file in Explorer.
### How to use
Image-comments are declared with: 

`/// <image url="X:\Path\To\Image.ext" scale="Y" />`

The `scale` attribute multiplies the source width and height by Y and is optional.


You can use the VS environment variables $(ProjectDir) and $(SolutionDir) in URLs, e.g.:

`/// <image <url="$(SolutionDir)\CommonImages\Fourier.jpg`


Images are displayed using the [WPF Image control](http://msdn.microsoft.com/en-us/library/ms610982) with a [BitmapFrame](http://msdn.microsoft.com/en-us/library/ms619213) source, and accepted image and URL formats are tied to those, e.g. BMP, PNG, JPG all work as image formats, and C:\Path\To\Image.png, http://www.server.com/image.png and \\\server\folder\image.png all work as URLs.


If there's a problem trying to load the image or parse the tag, the tag will be squiggly-underlined and hovering over this will show the error, e.g


![](http://lukesdm.github.com/image-comments/media/error-example-1.png)


The languages currently supported are C#, C, C++ and VB. For VB though, replace the beginning `///` with `'''`.


Image-comments don't really have anything to do with XML comments, but the format is convenient and it should be pretty straight-forward to transform them for Sandcastle documentation creation.


The extension adds a command in the Tools menu to toggle image-comment display on or off.


### Uninstallation
In VS, open the Extension Manager, select ImageComments, then click uninstall. A restart of VS is required.

### Some known issues
* After adding an image-comment using a local image, you can't edit the image until VS is closed. (High priority to fix).
* The caret/selection highlight height on image-comment lines grows as high the image.
* Image loading from HTTP sources usually doesn't work. But if you twiddle the tag to make it invalid then valid again, it works. It's probably due to treating the asynchronous image loading process as synchronous in the implementation (which works for local images).
* You need to scroll/'bump' the editor window to see the effect of the on/off toggle command.

## Development Info
Requires: Visual Studio 2013 SDK

### Build instructions
Providing the VS SDK is installed, you should be able to build by opening the solution and hitting F6. Debugging has to be configured manually - On the Project Properties->Debug tab, choose 'Start External Program' and command line e.g. (if using default install location) 'C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe' with arguments '/rootsuffix Exp'. The 'Release' build configuration copies the .vsix package into the Solution's .\Output directory.

### Program structure
It's a very small project and may be fairly self explanatory if you are familiar with Visual Studio editor extensions.
There are two components to the extension:

* ImageCommentsEditorComponent. Contains 97% of the functionality. 
* ImageCommentsPackage. Adds a command to enable/disable functionality; VSIX definition.

For testing information, see .\Testing\Testing.html
### Some known implementation issues
The code is a bit rough - it may not need a rewrite from scratch, but there's a bunch of stuff to be done

* Error/exception handling should be improved
* Program/project structure could be improved
* No automated tests and manual testing has been limited.
* There are some fairly obvious potential optimisations, but so far performance impact on plain Visual Studio seems minimal (in a release build on a 1.4GHz Core 2 Duo laptop with 1GB RAM). It would probably just add unneccessary complexity, but further testing might show otherwise.
* ...

## License
Eclipse Public License v1.0. See [license text](http://github.com/lukesdm/image-comments/raw/master/License.txt) for details.
