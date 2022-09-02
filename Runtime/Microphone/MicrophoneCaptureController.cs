#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using ChainBehaviors.IO;
using System.IO;
using AUE;
using AppTools.Audio;
using AppTools;
using AppTools.Microphone;

namespace ChainBehaviors.Microphone
{
    /// <summary>
    /// Provide a way to record the microphone (once it has been activated).
    /// Once the capture is stopped, the file path is passed as event argument.
    /// The recorded file cannot be directly read, it is a custom uncompressed format.
    /// Format is:
    /// [<see cref="AudioSampleFileHeader"/>][Audio sample data from <see cref="AudioClip.GetData"/>]
    /// </summary>
    /// <seealso cref="MicrophoneController"/>
    public class MicrophoneCaptureController : BaseMethod
    {
        [SerializeField]
        private bool _autoCapture = true;

        [SerializeField]
        private MicrophoneControllerReference _microphoneController = null;

        [SerializeField]
        private bool _shouldReleaseResourceWhenCaptureCompleted = true;

        [SerializeField,
         Tooltip("Should complete the capture if the component is destroyed and a recording is in progress?")]
        private bool _shouldCompleteCaptureIfDestroyed = true;

        [SerializeField, 
            Tooltip("Should automatically start the microphone capture (if it is not) when start recording")]
        private bool _shouldStartMicrophoneIfNotWhenCapturing = true;

        [SerializeField]
        private FileStreamCreator _outputStreamDesc = new FileStreamCreator()
        {
            FileMode = FileMode.Create,
            FileAccess = FileAccess.Write,
            FileShare = FileShare.None,
            FileOptions = FileOptions.SequentialScan,
        };

        [SerializeField]
        private BufferSize _captureBufferSize = new BufferSize(4096, BufferSize.EScale.Bytes);

        [SerializeField]
#if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
#endif
        private AUEEvent _onCaptureStarted;

        [SerializeField, Tooltip("The argument is the path to the capture file")]
#if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
#endif
        private AUEEvent<string> _onCaptureFileCompleted;

#if ODIN_INSPECTOR
        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public bool IsRecording => (_streamRecorder?.IsRecording ?? false);
#endif

        private MicrophoneSampleStreamRecorder _streamRecorder = null;
        private FileStream _outputStream;


        private void Start()
        {
            if (_autoCapture)
            {
                StartCapture();
            }
        }

#if ODIN_INSPECTOR
        [Button, FoldoutGroup("Editor Actions"), DisableInEditorMode]
#endif
        public void StartCapture()
        {
            if (_streamRecorder != null || _microphoneController == null || !_microphoneController.IsValueDefined)
            {
                TraceCustomMethodName("Start Capture ~ Cannot start",
                    ("stream recorder state", _streamRecorder),
                    ("microphone controller", _microphoneController));
                return;
            }

            MicrophoneController microphoneController = _microphoneController.Value;
            if (_shouldStartMicrophoneIfNotWhenCapturing && !microphoneController.IsCapturing)
            {
                microphoneController.StartMicrophoneCapture();
            }

            _outputStream = _outputStreamDesc.CreateFileStream();
            _outputStream.Seek(AudioSampleFileHeader.Size, SeekOrigin.Begin); // Skip header for now

            _streamRecorder = new MicrophoneSampleStreamRecorder();
            _streamRecorder.OnMicrophoneError += OnMicrophoneError;
            _streamRecorder.StartRecord(microphoneController.Device, microphoneController.CurrentMicrophoneClip, _outputStream, _captureBufferSize.BytesSizeAsInt);

            microphoneController.OnDevicePreChanged += OnMicrophoneDevicePreChanged;
            microphoneController.OnDevicePostChanged += OnMicrophoneDevicePostChanged;

            TraceCustomMethodName("Start Capture", ("file output path", _outputStream.Name));
            _onCaptureStarted.Invoke();
        }

        private void OnMicrophoneError()
        {
            Debug.Log("Microphone issue detected. Restart the microphone capture and the record...");

            CancelCapture();
            var microphoneController = _microphoneController.Value;
            microphoneController.ResetMicrophoneCapture();
            StartCapture();
        }

        private void OnMicrophoneDevicePreChanged(MicrophoneController microphoneController) => _streamRecorder.IsPaused = true;
        private void OnMicrophoneDevicePostChanged(MicrophoneController microphoneController) => _streamRecorder.IsPaused = false;

#if ODIN_INSPECTOR
        [Button, FoldoutGroup("Editor Actions"), DisableInEditorMode]
#endif
        public void StopCapture()
        {
            TraceCustomMethodName("Stop Capture");

            if (_streamRecorder == null)
            {
                return;
            }

            MicrophoneController microphoneController = _microphoneController.Value;
            microphoneController.OnDevicePreChanged -= OnMicrophoneDevicePreChanged;
            microphoneController.OnDevicePostChanged -= OnMicrophoneDevicePostChanged;

            _streamRecorder.StopRecord();
            var header = new AudioSampleFileHeader()
            {
                Frequency = (int) _streamRecorder.Frequency,
                NumChannels = (byte) _streamRecorder.Channels,
                NumSamples = _streamRecorder.NumSamples,
            };

            // Insert header
            _outputStream.Seek(0, SeekOrigin.Begin);
            header.Write(_outputStream);

            _outputStream.Close();
            _onCaptureFileCompleted?.Invoke(_outputStream.Name);

            if (_shouldReleaseResourceWhenCaptureCompleted)
            {
                ReleaseResources();
            }
        }

        public void CancelCapture()
        {
            TraceCustomMethodName("Cancel Capture");

            MicrophoneController microphoneController = _microphoneController.Value;
            microphoneController.OnDevicePreChanged -= OnMicrophoneDevicePreChanged;
            microphoneController.OnDevicePostChanged -= OnMicrophoneDevicePostChanged;

            _outputStream.Close();
            _outputStream.Dispose();
            _outputStream = null;

            _streamRecorder.StopRecord();
            ReleaseResources();
        }

        private void OnDestroy()
        {
            if (_shouldCompleteCaptureIfDestroyed)
            {
                StopCapture();
            }

            ReleaseResources();
        }

        public void Update()
        {
            _streamRecorder?.Update();
        }

        public void ReleaseResources()
        {
            _streamRecorder?.Dispose();
            _streamRecorder = null;
        }
    }
}