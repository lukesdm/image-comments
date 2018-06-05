namespace LM.ImageComments.Package
{
    using System.ComponentModel.Design;
    using LM.ImageComments.EditorComponent;
    using Microsoft.VisualStudio.Shell;
    using Task = System.Threading.Tasks.Task;

    internal sealed class ImageCommentsCommand
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var mcs = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;

            if (null != mcs)
            {
                var menuCommandID = new CommandID(GuidList.guidImageCommentsPackageCmdSet, (int)PkgCmdIDList.cmdidToggleImageComments);

                var menuItem = new MenuCommand((s, e) => Execute(package), menuCommandID);

                mcs.AddCommand(menuItem);
            }
        }

        private static void Execute(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ImageAdornmentManager.ToggleEnabled();
        }
    }
}
