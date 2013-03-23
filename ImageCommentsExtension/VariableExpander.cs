using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using LM.ImageComments.EditorComponent;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace LM.ImageComments
{
    /// <summary>
    /// This class provides variable substitution for strings, e.g. replacing '$(ProjectDir)' with 'C:\MyProject\'
    /// </summary>
    class VariableExpander
    {
        private readonly Regex _variableMatcher;
        private const string VARIABLE_PATTERN = @"\$\(\S+?\)";

        private const string PROJECTDIR_PATTERN = "$(ProjectDir)";
        private const string SOLUTIONDIR_PATTERN = "$(SolutionDir)";

        private string _projectDirectory;
        private string _solutionDirectory;

        private IWpfTextView _view;

        public VariableExpander(IWpfTextView view)
        {
            if (view == null)
            {
                 throw new ArgumentNullException("view");
            }
            _view = view;
            _variableMatcher = new Regex(VARIABLE_PATTERN, RegexOptions.Compiled);

            try
            {
                populateVariableValues();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Notify(ex, true);
            }
        }
        
        /// <summary>
        /// Processes URL by replacing $(Variables) with their values
        /// </summary>
        /// <param name="urlString">Input URL string</param>
        /// <returns>Processed URL string</returns>
        public string ProcessText(string urlString)
        {
            string processedUrl = _variableMatcher.Replace(urlString, match =>
                {
                    string variableName = match.Value;
                    if (string.Compare(variableName, PROJECTDIR_PATTERN, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        return _projectDirectory;
                    }
                    else if (string.Compare(variableName, SOLUTIONDIR_PATTERN, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        return _solutionDirectory;
                    }
                    else
                    {
                        // Could throw an exception here, but it's possible the path contains $(...).
                        // TODO: Variable name escaping
                        return variableName;
                    }
                });
            return processedUrl;
        }

        
        /// <summary>
        /// Populates variable values from project item associated with TextView.
        /// </summary>
        /// <remarks>Based on code from http://stackoverflow.com/a/2493865
        /// Guarantees variables will not be null, but they may be empty
        /// </remarks>
        private void populateVariableValues()
        {
            _projectDirectory = "";
            _solutionDirectory = "";
            
            ITextDocument document;
            _view.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof (ITextDocument), out document);
            var dte2 = (DTE2) Package.GetGlobalService(typeof (SDTE));
            ProjectItem projectItem = dte2.Solution.FindProjectItem(document.FilePath);
                
            string projectPath = projectItem.ContainingProject.FileName;
            if (projectPath != "") // projectPath will be empty if file isn't part of a project.
            {
                _projectDirectory = Path.GetDirectoryName(projectPath) + @"\";
            }

            string solutionPath = dte2.Solution.FileName;
            if (solutionPath != "") // solutionPath will be empty if project isn't part of a saved solution
            {
                _solutionDirectory = Path.GetDirectoryName(solutionPath) + @"\";
            }
        }
    }
}
