<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>ImageComments Readme</title>
    <style type="text/css">
        .style2
        {
            font-family: "Courier New", Courier, monospace;
        }
    </style>
</head>
<body>

    <h1>
        ImageComments (a Visual Studio Extension)</h1>
    <h2>
        Overview</h2>
    <p>
        This is an extension for the Visual Studio code editor that allows images to be 
        displayed amongst code, allowing for visually rich comments. For example...</p>
        <img src ="./readme_files/example1.png" />
    <p>
        Its creation was instigated by wanting to show nicely formatted formulae 
        alongside the code that implements them.</p>
    <h2>
        Usage Info</h2>
    <h3>
        Preamble</h3>
    <p>
        Disclaimer: This is project is a WIP at the 
        prototype stage 
        and may be unstable. Please report 
        issues on the GitHub repo.<br />
        Requires: Visual Studio 2010 Standard, Premium etc. 
        <em>(Not for 2012 yet) </em>.Net 4.<br />Also: currently only works in C# files</p>
    <h3>
        Installation</h3>
    <p>
        Activate the VISX file</p>
    <h3>
        How to use</h3>
    <p>
        Image-comments are declared with <br /> &nbsp;&nbsp;&nbsp; <span class="style2">/// &lt;image url=&quot;X:\Path\To\Image.ext&quot; <em>
        scale=&quot;Y&quot;</em> /&gt;</span></p>
    <p>
        The <span class="style2">scale</span> attribute multiplies the source width and 
        height by Y and is optional.</p>
    <p>
        Images are displayed using the
        <a href="http://msdn.microsoft.com/en-us/library/ms610982(v=vs.100).aspx">WPF 
        Image control</a> with a
        <a href="http://msdn.microsoft.com/en-us/library/ms619213(v=vs.100).aspx">
        BitmapFrame</a> source, and accepted image and URL formats are tied to those, 
        e.g. BMP, PNG, JPG all work as image formats, and
        C:\Path\To\Image.png,
        http://www.server.com/image.png and \\server\folder\image.png all 
        work as URLs*.</p>
    <p>
        Image-comments don&#39;t really have anything to do with XML comments, but the 
        format is convenient and it should be pretty straight forward to transform them 
        for Sandcastle doc creation.</p>
    <p>
        The extension adds a command in the Tools menu to toggle image-comment display 
        on or off **.</p>
    <h3>
        Uninstallation</h3>
    <p>
        In VS, open the Extension Manager, select ImageComments, then click uninstall. A 
        restart of VS is required.</p>
    <h3>
        Some known issues</h3>
    <ul>
        <li>Absolute paths to images only.</li>
        <li>The caret/selection highlight height on image-comment lines grows as high the 
            image.</li>
        <li>(*) Image loading from HTTP sources usually doesn&#39;t work. But if you twiddle 
            the tag to make it invalid then valid again, it works. It&#39;s probably due to 
            treating the asynchronous image loading process as synchronous in the 
            implementation (which works for fast connections like hard disks).</li>
        <li>(**) You need to scroll/&#39;bump&#39; the editor window to see the effect of the on/off 
            toggle command.</li>
    </ul>
    <h2>
        Development Info</h2>
    <p>
        Requires: Visual Studio 2010 SP1 SDK</p>
    <h3>
        Build instructions</h3>
    <p>
        Providing VS2010 SP1 SDK is installed, you should be able to build by opening 
        the solution and hitting F6, and start debugging with the VS Experimental 
        Instance with F5.</p>
    <h3>
        Program structure</h3>
    <p>
        It&#39;s a very small project and should be fairly self explanatory if you are 
        familiar with Visual Studio editor extensions.
        There are two components to the extension:</p>
        <ul>
            <li>ImageCommentsEditorComponent. Contains 97% of the functionality. </li>
            <li>ImageCommentsPackage. Adds a command to enable/disable functionality; VSIX 
                definition.</li>
    </ul>
    <p>
        For testing information, see <a href="./testing/testing.html">.\Testing\Testing.html</a></p>
    <h3>
        Some known implementation issues</h3>
    <ul>
        <li>Error/exception handling could be improved</li>
        <li>Program/project structure could be improved</li>
        <li>No automated tests and manual testing has been limited.</li>
    </ul>

</body>
</html>
