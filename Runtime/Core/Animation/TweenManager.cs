using UnityEngine;
using System.Collections.Generic;

namespace GSGUnityUtilities.Runtime
{
    public class TweenManager : MonoBehaviour
    {
        private static TweenManager instance;
        private List<Tween> activeTweens = new List<Tween>();
        private List<Tween> tweensToRemove = new List<Tween>();

        public static TweenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("TweenManager");
                    instance = go.AddComponent<TweenManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public void AddTween(Tween tween)
        {
            if (!activeTweens.Contains(tween))
            {
                activeTweens.Add(tween);
            }
        }

        public void RemoveTween(Tween tween)
        {
            tweensToRemove.Add(tween);
        }

        private void Update()
        {
            // 更新所有活動的 tween
            for (int i = 0; i < activeTweens.Count; i++)
            {
                activeTweens[i].Update();
            }

            // 移除已完成的 tween
            if (tweensToRemove.Count > 0)
            {
                foreach (var tween in tweensToRemove)
                {
                    activeTweens.Remove(tween);
                }
                tweensToRemove.Clear();
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                activeTweens.Clear();
                tweensToRemove.Clear();
            }
        }

        public static void DestroyInstance()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }
        }
    }
}