using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectEventHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public UnityEvent<GameObject> OnSelectEvent;

    public UnityEvent<GameObject> OnDeselectEvent;

    // 當物件被選取時呼叫這個方法
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log(gameObject.name + " 被選取了");        

        // 通知選取事件
        if (OnSelectEvent != null)
        {
            OnSelectEvent.Invoke(gameObject);
        }

        if (EventSystem.current.firstSelectedGameObject != gameObject)
        {
            EventSystem.current.firstSelectedGameObject = gameObject;
        }
    }

    // 當物件被取消選取時呼叫這個方法
    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log(gameObject.name + " 被取消選取了");

        // 通知取消選取事件
        if (OnDeselectEvent != null)
        {
            OnDeselectEvent.Invoke(gameObject);
        }

        if (EventSystem.current.firstSelectedGameObject == gameObject)
        {
            EventSystem.current.firstSelectedGameObject = null;
        }
    }
}
