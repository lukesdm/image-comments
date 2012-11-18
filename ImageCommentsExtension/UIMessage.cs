namespace LM.ImageComments.EditorComponent
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    internal static class UIMessage
    {
        // Prints message to VS output window. 
        // LM: Couldn't get general output pane to work, so resorted to Debug pane. TODO: Investigate and fix. Maybe create custom pane ala. Code Contracts extension
        // From http://stackoverflow.com/questions/1094366/how-do-i-write-to-the-visual-studio-output-window-in-my-custom-tool
        public static void Show(string message)
        {
            IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

            //Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane; //LM: GetPane failed when passing in general output pane GUID. [Is there such a thing?]
            Guid debugPaneGuid = VSConstants.GUID_OutWindowDebugPane;
            IVsOutputWindowPane debugPane;
            int gotPane = outWindow.GetPane(ref debugPaneGuid, out debugPane);
            if (gotPane == VSConstants.S_OK)
            {
                debugPane.OutputString("[ImageComments Extension] " + message + "\n");
                debugPane.Activate(); // Brings this pane into view
            }
        }
    }
}
