namespace LM.ImageComments.EditorComponent
{
    using System;

    internal static class ExceptionHandler
    {
        public static void Notify(Exception ex, bool showMessage)
        {
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

        // Classifies exception as critical (and unable to be handled) or not
        // From http://bit.ly/QeAHTk
        public static bool IsCritical(Exception ex) 
        {
          if (ex is OutOfMemoryException) return true;
          if (ex is AppDomainUnloadedException) return true;
          if (ex is BadImageFormatException) return true;
          if (ex is CannotUnloadAppDomainException) return true;
          if (ex is InvalidProgramException) return true;
          if (ex is System.Threading.ThreadAbortException) 
              return true;
          return false;
        }
    }
}