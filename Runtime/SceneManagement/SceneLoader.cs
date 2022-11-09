using AUE;
using ChainBehaviors.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainBehaviors.SceneManagement
{
    [AddComponentMenu(CBConstants.ModuleSceneManagement + "Scene Loader")]
    public class SceneLoader : BaseMethod
    {
        [SerializeField]
        private string _sceneName;
        public string SceneName { get => _sceneName; set => _sceneName = value; }

        [SerializeField]
        private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
        public LoadSceneMode LoadSceneMode { get => _loadSceneMode; set => _loadSceneMode = value; }

        [SerializeField]
        private AUEEvent<string> _onSceneLoaded;


        public void Execute()
        {
            ExecuteAsync().Forget();
        }

        public async UniTask ExecuteAsync()
        {
            Trace(("Scene", _sceneName), ("Load Scene Mode", _loadSceneMode));
            await SceneManager.LoadSceneAsync(_sceneName, _loadSceneMode);
            _onSceneLoaded.Invoke(_sceneName);
        }
    }
}