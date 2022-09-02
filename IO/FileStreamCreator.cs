using AppTools;
using AppTools.FileManagement;
using System.IO;
using UnityEngine;

namespace ChainBehaviors.IO
{
    /// <summary>
    /// Create a FileStream based on settings.
    /// </summary>
    [System.Serializable]
    public class FileStreamCreator : IStream
    {
        [SerializeReference, SerializedInterface]
        private IFilePathBuilder _filePathBuilder;
        public IFilePathBuilder FilePathBuilder { get => _filePathBuilder; set => _filePathBuilder = value; }

        [SerializeField]
        private FileMode _fileMode = FileMode.Create;
        public FileMode FileMode { get => _fileMode; set => _fileMode = value; }

        [SerializeField]
        private FileAccess _fileAccess = FileAccess.ReadWrite;
        public FileAccess FileAccess { get => _fileAccess; set => _fileAccess = value; }

        [SerializeField]
        private FileShare _fileShare = FileShare.ReadWrite;
        public FileShare FileShare { get => _fileShare; set => _fileShare = value; }

        [SerializeField]
        private BufferSize _bufferSize = new BufferSize(4096, BufferSize.EScale.Bytes);
        public BufferSize BufferSize { get => _bufferSize; set => _bufferSize = value; }

        [SerializeField]
        private FileOptions _fileOptions = FileOptions.None;
        public FileOptions FileOptions { get => _fileOptions; set => _fileOptions = value; }


        public Stream CreateStream(long length = 0) => CreateFileStream(length);

        public FileStream CreateFileStream(long length = 0)
        {
            if (_filePathBuilder == null)
            {
                throw new System.InvalidOperationException("File Path Builder must not be null");
            }

            string path = _filePathBuilder.BuildFilePath();
            var fs = new FileStream(path, _fileMode, _fileAccess, _fileShare, _bufferSize.BytesSizeAsInt, _fileOptions);
            if (length > 0)
            {
                fs.SetLength(length);
            }
            return fs;
        }
    }
}
