namespace LM.RichComments.Package
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using LM.RichComments.EditorComponent;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Package containing rich comment command
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidRichCommentsPackagePkgString)]
    public sealed class RichCommentsPackage : Microsoft.VisualStudio.Shell.Package
    {
        #region Package Members

        /// <summary>
        /// Creates and registers rich comment enable/disable toggle command 
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                CommandID menuCommandID = new CommandID(GuidList.guidRichCommentsPackageCmdSet, (int)PkgCmdIDList.cmdidToggleRichComments);
                MenuCommand menuItem = new MenuCommand(
                    (sender, args) => { RichCommentItemManager.ToggleEnabled(); }, 
                    menuCommandID);
                mcs.AddCommand( menuItem );
            }
        }
        #endregion
    }
}
