using ChainBehaviors.Utils;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace ChainBehaviors.Processes
{
    /// <summary>
    /// Run a list of processes (calling their <see cref="BaseProcess.ExecuteProcess"/>).
    /// The processes must be active to be updated.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleProcessors + "Processor Controller")]
    public class ProcessorsController : BaseMethod
    {
        [SerializeField]
        private bool _autoStart = true;

#if UNITY_EDITOR
        [SerializeField]
        private bool _executeInEditor = false;
#endif

        [SerializeField]
        private BaseProcess[] _processes = null;

        [ShowInInspector, ReadOnly]
        private bool _isPlaying = false;
        private Coroutine _updateProcessesCoroutine = null;
        private bool _hasBeenOverriden = false;

        public bool IsPlaying 
        {
            get => _isPlaying;
            set
            {
                _hasBeenOverriden = true;
                if (value != _isPlaying)
                {
                    _isPlaying = value;
                    TraceCustomMethodName("New Play State", ("state", _isPlaying));
                    if (_isPlaying && _updateProcessesCoroutine == null)
                    {
                        StartProcessorCoroutine();
                    }
                    else if (!_isPlaying && _updateProcessesCoroutine != null)
                    {
                        StopProcessorCoroutine();
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEngine.Application.isPlaying)
            {
                runInEditMode = _executeInEditor && enabled;
                IsPlaying = false;
                IsPlaying = _executeInEditor && enabled;
                _hasBeenOverriden = false;
            }
        }
#endif

        private void Start()
        {
            if (_autoStart && !_hasBeenOverriden)
            {
                IsPlaying = true;
            }
        }

        private void OnEnable()
        {
            // If the processor was playing has been re-enabled,
            // we restart the processor coroutine
            if (_isPlaying)
            {
                StartProcessorCoroutine();
            }
        }

        private void OnDisable() => StopProcessorCoroutine();

        private void StartProcessorCoroutine()
        {
            TraceCustomMethodName("Start Update Processes");
            _updateProcessesCoroutine = StartCoroutine(UpdateProcesses());
        }

        private void StopProcessorCoroutine()
        {
            TraceCustomMethodName("Stop Update Processes");
            if (_updateProcessesCoroutine != null)
            {
                StopCoroutine(_updateProcessesCoroutine);
                _updateProcessesCoroutine = null;
            }
        }

        public void Play() => IsPlaying = true;
        public void Pause() => IsPlaying = false;

        public void UpdateAllProcessesOnce()
        {
            for (int i = 0; i < _processes.Length; ++i)
            {
                if (_processes[i].IsActive)
                {
                    _processes[i].UpdateProcess(Time.deltaTime);
                }
            }
        }

        private IEnumerator UpdateProcesses()
        {
            while (true)
            {
                UpdateAllProcessesOnce();
                yield return null;
            }
        }
    }
}