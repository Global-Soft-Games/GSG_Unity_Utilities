using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GSGUnityUtilities.Runtime
{
    [System.Serializable]
    public class SceneReference : IReference
    {
        [SerializeField] private Object sceneAsset;
        [SerializeField] private string scenePath;
        
        public string ScenePath => scenePath;
        public string SceneName => System.IO.Path.GetFileNameWithoutExtension(scenePath);
        
        public async Task LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Additive)
        {
            if (string.IsNullOrEmpty(scenePath)) return;

            await Task.Delay(100);

            Debug.Log($"Loading scene {SceneName} with mode {mode}");

            // 如果場景已經載入，則直接返回
            if (SceneManager.GetSceneByName(SceneName).isLoaded) 
            {
                Debug.Log($"Scene {SceneName} is already loaded");
                return;
            }
            
            var operation = SceneManager.LoadSceneAsync(SceneName, mode);

            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }

        public async Task UnloadSceneAsync()
        {
            if (string.IsNullOrEmpty(scenePath)) return;

            // 如果場景未載入，則直接返回
            if (!SceneManager.GetSceneByName(SceneName).isLoaded) 
            {
                Debug.Log($"Scene {SceneName} is not loaded");
                return;
            }
            
            var operation = SceneManager.UnloadSceneAsync(SceneName);
            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }

        public void Init()
        {
            Debug.Log($"Scene Reference Init: {SceneName}");
        }

        public void Release()
        {
            Debug.Log($"Scene Reference Release: {SceneName}");
        }
    }
} 