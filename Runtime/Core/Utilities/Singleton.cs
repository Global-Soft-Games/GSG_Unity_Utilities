using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _shuttingDown = false;
    private static bool _initialized = false;

    // 可選配置：是否在場景之間保持持久化
    protected virtual bool IsPersistent => true;

    public static T Instance
    {
        get
        {
            if (_shuttingDown)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null && !_initialized)
                {
                    // 尋找現有實例
                    var instances = FindObjectsOfType<T>();
                    if (instances.Length > 0)
                    {
                        _instance = instances[0];
                        if (instances.Length > 1)
                        {
                            Debug.LogWarning($"[Singleton] Multiple instances of {typeof(T)} found. Using the first one.");
                            for (int i = 1; i < instances.Length; i++)
                                Destroy(instances[i].gameObject);
                        }
                    }

                    // 如果沒有找到實例，創建一個新的
                    if (_instance == null)
                    {
                        var obj = new GameObject($"[Singleton] {typeof(T)}");
                        _instance = obj.AddComponent<T>();
                        
                        if (Application.isPlaying && _instance != null)
                        {
                            var singletonComponent = _instance.GetComponent<Singleton<T>>();
                            if (singletonComponent != null && singletonComponent.IsPersistent)
                            {
                                DontDestroyOnLoad(obj);
                            }
                        }
                    }

                    _initialized = true;
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[Singleton] Multiple instances of {typeof(T)} detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        if (_instance == null)
        {
            _instance = this as T;
            
            if (Application.isPlaying && IsPersistent)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        
        _initialized = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _shuttingDown = true;
            _initialized = false;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _shuttingDown = true;
        _initialized = false;
    }
}
