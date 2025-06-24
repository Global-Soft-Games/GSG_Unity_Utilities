using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GSGUnityUtilities.Runtime
{
    public static class StaticReferenceManager
    {
        public static Dictionary<string, IReference> ReferenceMap { get; private set; } = new Dictionary<string, IReference>();

        public static void Init()
        {
            ReferenceMap = new Dictionary<string, IReference>();
            Debug.Log("StaticReferenceManager Init");
        }

        public static void Release()
        {
            ReferenceMap.Clear();
            Debug.Log("StaticReferenceManager Release");
        }

        // 添加引用
        public static void AddReference<T>(T reference) where T : IReference
        {
            string key = typeof(T).Name;
            Debug.Log($"StaticReferenceManager: AddReference {key}");

            if (!ReferenceMap.ContainsKey(key))
            {
                ReferenceMap.Add(key, reference);
            }
            else
            {
                Debug.LogWarning($"StaticReferenceManager: Key {key} already exists.");
            }
        }

        // 获取引用
        public static T GetReference<T>() where T : class, IReference
        {
            string key = typeof(T).Name;
            Debug.Log($"StaticReferenceManager: GetReference {key}");

            if (ReferenceMap.ContainsKey(key))
            {
                return ReferenceMap[key] as T;
            }
            else
            {
                Debug.LogWarning($"StaticReferenceManager: Key {key} not found.");
                return null;
            }
        }

        // 检查是否包含引用
        public static bool ContainsReference<T>() where T : class, IReference
        {
            string key = typeof(T).Name;
            Debug.Log($"StaticReferenceManager: ContainsReference {key}");

            return ReferenceMap.ContainsKey(key);
        }

        // 移除引用
        public static void RemoveReference<T>(T reference) where T : IReference
        {
            string key = typeof(T).Name;
            Debug.Log($"StaticReferenceManager: RemoveReference {key}");

            if (ReferenceMap.ContainsKey(key))
            {
                ReferenceMap.Remove(key);
            }
            else
            {
                Debug.LogWarning($"StaticReferenceManager: Key {key} not found.");
            }
        }

#if UNITY_EDITOR
        public static void LogReferences()
        {
            Debug.Log("Current References:");
            foreach (var kvp in ReferenceMap)
            {
                Debug.Log($"Type: {kvp.Key}, Instance: {kvp.Value}");
            }
        }
#endif
    }
}
