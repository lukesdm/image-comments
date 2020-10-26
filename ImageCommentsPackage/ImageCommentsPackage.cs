namespace LM.ImageComments.Package
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.VisualStudio.Shell;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Package containing image comment command
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidImageCommentsPackagePkgString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class ImageCommentsPackage : AsyncPackage
    {
        /// <summary>
        /// Creates and registers image comment enable/disable toggle command 
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await ImageCommentsCommand.InitializeAsync(this);
        }
    }
}
