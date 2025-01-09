using UnityEngine;
using UnityEngine.Events;

public class ObjectCreationHandler : MonoBehaviour
{
    [System.Serializable]
    public class ObjectCreatedEvent : UnityEvent<GameObject> { }

    public GameObject objectPrefab;
    public Transform parent;

    public ObjectCreatedEvent OnObjectCreated;
    public UnityEvent OnCreationFailed;

    public void HandleObjectCreation()
    {
        if (objectPrefab == null)
        {
            OnCreationFailed.Invoke();
            return;
        }

        GameObject createdObject = Instantiate(objectPrefab, parent);
        OnObjectCreated.Invoke(createdObject);
    }
}