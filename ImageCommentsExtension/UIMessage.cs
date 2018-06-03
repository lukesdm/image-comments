namespace LM.ImageComments.EditorComponent
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// Provides method[s] for sending GUI messages.
    /// </summary>
    internal static class UIMessage
    {
        /// <summary>
        /// Prints message to VS output window
        /// </summary>
        /// <param name="message">Message</param>
        /// <remarks>Adapted from http://stackoverflow.com/questions/1094366/how-do-i-write-to-the-visual-studio-output-window-in-my-custom-tool </remarks>
        public static void Show(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

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
