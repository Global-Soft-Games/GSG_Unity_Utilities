using System.Collections.Generic;
using UnityEngine;

public interface IReference
{
    // 在这里可以定义一些通用的接口方法或属性
}

public class ReferenceManager : Singleton<ReferenceManager>
{
    private Dictionary<string, IReference> _references = new Dictionary<string, IReference>();

    // 添加引用
    public void AddReference<T>(T reference) where T : IReference
    {
        string key = typeof(T).Name;
        Debug.Log($"ReferenceManager: AddReference {key}");

        if (!_references.ContainsKey(key))
        {
            _references.Add(key, reference);
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

        if (_references.ContainsKey(key))
        {
            return _references[key] as T;
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

        return _references.ContainsKey(key);
    }

    // 移除引用
    public void RemoveReference<T>(T reference) where T : class, IReference
    {
        string key = typeof(T).Name;
        Debug.Log($"ReferenceManager: RemoveReference {key}");
        
        if (_references.ContainsKey(key))
        {
            _references.Remove(key);
        }
        else
        {
            Debug.LogWarning($"ReferenceManager: Key {key} not found.");
        }
    }
}
