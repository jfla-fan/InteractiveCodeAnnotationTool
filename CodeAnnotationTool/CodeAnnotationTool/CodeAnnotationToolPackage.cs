﻿using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CodeAnnotationTool.NotionProvider;
using EnvDTE;
using Task = System.Threading.Tasks.Task;

namespace CodeAnnotationTool
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(CodeAnnotationToolPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ToolWindow1))]
    public sealed class CodeAnnotationToolPackage : AsyncPackage
    {
        /// <summary>
        /// CodeAnnotationToolPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "01ee88bb-6152-4548-a9f3-042a5f7fc430";

        public const string FileStorageName = "notions.json";

        #region Package Members
        
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            {
                DTE dteService = (DTE)GetGlobalService(typeof(DTE));
                string solutionDirPath = Path.GetDirectoryName(dteService.Solution.FullName);

                Debug.WriteLine($"Solution dir path inside lambda: {solutionDirPath}");

                ((IServiceContainer)this).AddService(typeof(CachedNotionProvider),
                    new CachedNotionProvider(Path.Combine(solutionDirPath, FileStorageName)),
                    true);
            }

            await ToolWindow1Command.InitializeAsync(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CachedNotionProvider notionProvider = (CachedNotionProvider) GetService(typeof(CachedNotionProvider));
                notionProvider?.Dispose();
            }
        }

        #endregion
    }
}
