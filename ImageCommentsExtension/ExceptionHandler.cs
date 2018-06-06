namespace LM.ImageComments.EditorComponent
{
    using Microsoft.VisualStudio.Shell;
    using System;

    internal static class ExceptionHandler
    {
        public static void Notify(Exception ex, bool showMessage)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string message = string.Format("{0}: {1}", DateTime.Now, ex.ToString());
            Console.WriteLine(message);
            if (showMessage)
            {
                UIMessage.Show(message);
            }
            // LM: Code for modeless message box. Removed as frequent messages make it really annoying
            ////if (showMessageBox)
            ////{
            ////    new Thread(o => {MessageBox.Show(o.ToString());}).Start(ex);
            ////}
        }
    }
}