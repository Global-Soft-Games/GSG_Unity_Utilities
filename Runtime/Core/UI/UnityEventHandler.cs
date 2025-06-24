using UnityEngine;
using UnityEngine.Events;

public class UnityEventHandler : MonoBehaviour
{
    public UnityEvent OnAwakeEvent;
    public UnityEvent OnEnableEvent;
    public UnityEvent OnStartEvent;
    public UnityEvent OnDisableEvent;
    public UnityEvent OnDestroyEvent;

    private void Awake()
    {
        Debug.Log($"{gameObject.name}: Awake 被調用");
        OnAwakeEvent.Invoke();
    }

    private void OnEnable()
    {
        Debug.Log($"{gameObject.name}: OnEnable 被調用");
        OnEnableEvent.Invoke();
    }

    private void Start()
    {
        Debug.Log($"{gameObject.name}: Start 被調用");
        OnStartEvent.Invoke();
    }

    private void OnDisable()
    {
        Debug.Log($"{gameObject.name}: OnDisable 被調用");
        OnDisableEvent.Invoke();
    }

    private void OnDestroy()
    {
        Debug.Log($"{gameObject.name}: OnDestroy 被調用");
        OnDestroyEvent.Invoke();
    }
}