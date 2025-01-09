using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class ToggleGroupStateHandler : MonoBehaviour
{
    [System.Serializable]
    public class ToggleStateEvent : UnityEvent<int> { }

    public ToggleGroup toggleGroup;
    public ToggleStateEvent OnStateChanged;

    private List<Toggle> toggles;
    private int currentState = -1;

    private void Start()
    {
        if (toggleGroup == null)
        {
            Debug.LogError("Toggle Group is not assigned!");
            return;
        }

        toggles = new List<Toggle>(toggleGroup.GetComponentsInChildren<Toggle>());
        for (int i = 0; i < toggles.Count; i++)
        {
            int index = i; // 捕获循环变量
            toggles[i].onValueChanged.AddListener((isOn) => {
                if (isOn) UpdateState(index);
            });
        }

        // 初始化状态
        UpdateState();
    }

    private void UpdateState(int newState = -1)
    {
        if (newState == -1)
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].isOn)
                {
                    newState = i;
                    break;
                }
            }
        }

        if (currentState != newState)
        {
            currentState = newState;
            OnStateChanged.Invoke(currentState);
        }
    }

    public int GetCurrentState()
    {
        return currentState;
    }
}