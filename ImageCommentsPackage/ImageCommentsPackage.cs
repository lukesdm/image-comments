namespace LM.ImageComments.Package
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
    using System.Runtime.InteropServices;
    using LM.ImageComments.EditorComponent;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Package containing image comment command
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidImageCommentsPackagePkgString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class ImageCommentsPackage : Microsoft.VisualStudio.Shell.Package
    {
        #region Package Members

        /// <summary>
        /// Creates and registers image comment enable/disable toggle command 
        /// </summary>
        protected override void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                CommandID menuCommandID = new CommandID(GuidList.guidImageCommentsPackageCmdSet, (int)PkgCmdIDList.cmdidToggleImageComments);
                MenuCommand menuItem = new MenuCommand(
                    (sender, args) => { ImageAdornmentManager.ToggleEnabled(); }, 
                    menuCommandID);
                mcs.AddCommand( menuItem );
            }
        }
        #endregion
    }
}
