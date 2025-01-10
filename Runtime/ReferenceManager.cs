using System.Collections.Generic;
using UnityEngine;

public interface IReference
{
    public void Init();
    public void Release();
}

public interface IReferenceManager : IReference
{
    Dictionary<string, IReference> ReferenceMap { get; }
    void AddReference<T>(T reference) where T : IReference;
    void RemoveReference<T>(T reference) where T : IReference;
    T GetReference<T>() where T : class, IReference;
    bool ContainsReference<T>() where T : class, IReference;
}

public class ReferenceManager : Singleton<ReferenceManager>, IReferenceManager
{
    public Dictionary<string, IReference> ReferenceMap { get; private set; } = new Dictionary<string, IReference>();

    public void Init()
    {
        ReferenceMap = new Dictionary<string, IReference>()
        {
            { typeof(IReferenceManager).Name, this },
        };

        Debug.Log("ReferenceManager Init");
    }

    public void Release()
    {
        ReferenceMap.Clear();
        Debug.Log("ReferenceManager Release");
    }

    // 添加引用
    public void AddReference<T>(T reference) where T : IReference
    {
        string key = typeof(T).Name;
        Debug.Log($"ReferenceManager: AddReference {key}");

        if (!ReferenceMap.ContainsKey(key))
        {
            ReferenceMap.Add(key, reference);
        }
        else
        {
            Debug.LogWarning($"ReferenceManager: Key {key} already exists.");
        }
    }

    // 获取引用
    public T GetReference<T>() where T : class, IReference
    {
        string key = typeof(T).Name;
        Debug.Log($"ReferenceManager: GetReference {key}");

        if (ReferenceMap.ContainsKey(key))
        {
            return ReferenceMap[key] as T;
        }
        else
        {
            Debug.LogWarning($"ReferenceManager: Key {key} not found.");
            return null;
        }
    }

    // 检查是否包含引用
    public bool ContainsReference<T>() where T : class, IReference
    {
        string key = typeof(T).Name;
        Debug.Log($"ReferenceManager: ContainsReference {key}");

        return ReferenceMap.ContainsKey(key);
    }

    // 移除引用
    public void RemoveReference<T>(T reference) where T : IReference
    {
        string key = typeof(T).Name;
        Debug.Log($"ReferenceManager: RemoveReference {key}");

        if (ReferenceMap.ContainsKey(key))
        {
            ReferenceMap.Remove(key);
        }
        else
        {
            Debug.LogWarning($"ReferenceManager: Key {key} not found.");
        }
    }
}
