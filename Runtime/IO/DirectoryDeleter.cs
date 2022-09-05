using AppTools.FileManagement;
using System;
using System.IO;
using UnityEngine;
using ChainBehaviors.Utils;

namespace ChainBehaviors.IO
{
    /// <summary>
    /// Delete a specific directory.
    /// The directory can be set from the component or pass as an argument.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleIOPath + "Directory Deleter")]
    public class DirectoryDeleter : BaseMethod
    {
        [SerializeReference, SerializedInterface]
        private IDirectoryPathBuilder _targetDirectory;

        [SerializeField]
        private bool _insideOnly = false;

        public void Delete()
        {
            if (_targetDirectory == null)
            {
                throw new InvalidOperationException("Target directory must be set when no path is specified");
            }

            Delete(_targetDirectory.BuildDirectoryPath());
        }

        public void Delete(string directoryPath)
        {
            Trace(("directory path", directoryPath));
            if (Directory.Exists(directoryPath))
            {
                if (_insideOnly)
                {
                    DeleteDirectoryInside(directoryPath);
                }
                else
                {
                    Directory.Delete(directoryPath, recursive: true);
                }
            }
        }

        private void DeleteDirectoryInside(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            string[] dirs = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
