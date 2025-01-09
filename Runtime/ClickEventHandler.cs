using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ClickEventHandler : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke();
    }
}