using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _shuttingDown = false;

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
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();

                        // Ensure the singleton persists across scenes
                        DontDestroyOnLoad(obj);
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void OnDestroy()
    {
        _shuttingDown = true;
    }

    protected virtual void OnApplicationQuit()
    {
        _shuttingDown = true;
    }
}
