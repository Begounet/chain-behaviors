using AppTools.FileManagement;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ChainBehaviors.Utils;

namespace ChainBehaviors.IO
{
    /// <summary>
    /// Save a byte[] buffer to a file (in an async way)
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleIOPath + "Byte Buffer To File")]
    public class ByteBufferToFile : BaseMethod
    {

        [SerializeField]
        private AppFilePath _outputFilePath = new AppFilePath(AppDirectoryPath.EPathSource.DataPath, "output", ".dat");
        public AppFilePath OutputFilePath { get => _outputFilePath; set => _outputFilePath = value; }

        [SerializeField, Tooltip("If file already exists, it will be overwritten")]
        private bool _overwrite = true;

        [SerializeField]
        private bool _cancelOperationIfDestroyed = false;

        private List<CancellationTokenSource> _cancelTknSrcs = new List<CancellationTokenSource>();


        public void Save(byte[] data) => SaveAsync(data).Forget();

        private async UniTaskVoid SaveAsync(byte[] data)
        {
            try
            {
                TraceCustomMethodName("save", ("file path", _outputFilePath.FilePath));

                string outputDirectory = Path.GetDirectoryName(_outputFilePath.FilePath);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                if (_overwrite && File.Exists(_outputFilePath.FilePath))
                {
                    File.Delete(_outputFilePath.FilePath);
                }

                using (FileStream fs = File.Open(_outputFilePath.FilePath, FileMode.OpenOrCreate))
                {
                    fs.Seek(0, SeekOrigin.Begin);

                    CancellationTokenSource cancelTknSrc = new CancellationTokenSource();
                    _cancelTknSrcs.Add(cancelTknSrc);

                    await fs.WriteAsync(data, 0, data.Length, cancelTknSrc.Token);
                    await fs.FlushAsync(cancelTknSrc.Token);
                }
            }
            catch (TaskCanceledException)
            {
                Debug.Log($"File writing into {_outputFilePath.FilePath} cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }

        private void CancelAllSaves()
        {
            _cancelTknSrcs.ForEach((cancelTknSrc) => cancelTknSrc.Cancel());
            _cancelTknSrcs.Clear();
        }

        private void OnDestroy()
        {
            if (_cancelOperationIfDestroyed)
            {
                CancelAllSaves();
            }
        }
    }
}