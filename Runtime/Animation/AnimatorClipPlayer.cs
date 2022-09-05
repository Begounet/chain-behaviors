using Sirenix.OdinInspector;
using AppTools;
using UnityEngine;
using UnityEngine.Assertions;
using ChainBehaviors.Utils;

namespace ChainBehaviors
{
    /// <summary>
    /// Allow to play a specific clip on an animator
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleAnimationPath + "Animator Clip Player")]
    public class AnimatorClipPlayer : BaseMethod
    {
        public enum ESource
        {
            Script,
            Constant
        }

        [SerializeField]
        private Animator _animator = null;

        [SerializeField]
        private ESource _sourceMode = ESource.Script;

        [SerializeField, ShowIf("_sourceMode", ESource.Constant)]
        private AnimatorParameterName _stateName = null;


        public void Play(string stateName)
        {
            Trace(stateName);
            Assert.AreEqual(_sourceMode, ESource.Script);
            PlayInternal(Animator.StringToHash(stateName));
        }

        public void Play()
        {
            Trace();
            Assert.AreEqual(_sourceMode, ESource.Constant);
            PlayInternal(_stateName.Id);
        }

        private void PlayInternal(int stateId)
        {
            _animator.Play(stateId);
        }
    }
}