using AppTools;
using AUE;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using ChainBehaviors.Utils;

namespace ChainBehaviors
{
    /// <summary>
    /// Call an event after X seconds.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Delay")]
    public class Delay : BaseMethod
    {
        private class InternalOp
        {
            private CancellationTokenSource _cancelTknSrc = null;

            public void Cancel()
            {
                _cancelTknSrc?.Cancel();
                _cancelTknSrc = null;
            }

            public async UniTask ExecuteAsync(float delay)
            {
                _cancelTknSrc = new CancellationTokenSource();
                await UniTaskEx.WaitForSeconds(delay);
                _cancelTknSrc = null;
            }
        }

        [SerializeField]
        private float _delay = 1.0f;

        [SerializeField, FormerlySerializedAs("_cancelIfExecutedAgain")]
        private bool _restartIfExecutedAgain = true;

        [SerializeField]
        private AUEEvent _onDelayCompleted = null;


        private List<InternalOp> _operations = new List<InternalOp>();

        public void Execute() => Execute(_delay);

        public void Execute(float delay)
        {
            Trace(delay);

            if (_restartIfExecutedAgain)
            {
                CancelAllOperations();
            }

            InternalOp op = new InternalOp();
            _operations.Add(op);
            _ = op.ExecuteAsync(delay)
                .ContinueWith(() => _onDelayCompleted.Invoke())
                .ContinueWith(() => _operations.Remove(op));
        }

        private void OnDisable() => CancelAllOperations();
        private void OnDestroy() => CancelAllOperations();

        public void CancelAllOperations()
        {
            TraceCustomMethodName("Cancel delay operations");

            _operations.ForEach((op) => op.Cancel());
            _operations.Clear();
        }
    }
}