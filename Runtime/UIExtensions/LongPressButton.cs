using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class LongPressButton : Button
{
    [System.Serializable]
    public class ButtonLongPressEvent : UnityEvent { }

    public ButtonLongPressEvent onLongPress = new ButtonLongPressEvent();
    public float longPressDuration = 0.5f; // 開始視為長按的時間
    public float longPressInterval = 0.1f; // 長按時事件觸發的間隔

    private float pressTimer = 0f;
    private float lastEventTime = 0f;
    private bool isPressed = false;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isPressed = true;
        pressTimer = 0f;
        lastEventTime = 0f;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        ResetButton();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        ResetButton();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ResetButton();
    }

    protected virtual void Update()
    {
        if (isPressed)
        {
            pressTimer += Time.deltaTime;

            if (pressTimer >= longPressDuration)
            {
                if (Time.time - lastEventTime >= longPressInterval)
                {
                    onLongPress.Invoke();
                    lastEventTime = Time.time;
                }
            }
        }
    }

    private void ResetButton()
    {
        isPressed = false;
        pressTimer = 0f;
        lastEventTime = 0f;
    }
}