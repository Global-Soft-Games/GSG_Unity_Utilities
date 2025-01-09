using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class MainToggleController : MonoBehaviour
{
    [SerializeField]
    private Toggle mainToggle;

    [SerializeField]
    private List<Toggle> controlledToggles = new List<Toggle>();

    private bool isUpdating = false;

    public UnityEvent OnAnyToggleValueChanged = new UnityEvent();

    private void Start()
    {
        if (mainToggle != null)
        {
            mainToggle.onValueChanged.AddListener(OnMainToggleValueChanged);
        }
        else
        {
            Debug.LogError("Main toggle is not assigned!");
        }

        foreach (var toggle in controlledToggles)
        {
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(OnControlledToggleValueChanged);
            }
        }
    }

    private void OnMainToggleValueChanged(bool value)
    {
        if (isUpdating) return;

        isUpdating = true;
        foreach (var toggle in controlledToggles)
        {
            if (toggle != null)
            {
                toggle.isOn = value;
            }
        }
        isUpdating = false;
    }

    private void OnControlledToggleValueChanged(bool value)
    {
        if (isUpdating) return;

        isUpdating = true;
        UpdateMainToggleState();
        isUpdating = false;

        OnAnyToggleValueChanged.Invoke();
    }

    private void UpdateMainToggleState()
    {
        bool allOn = true;
        bool allOff = true;

        foreach (var toggle in controlledToggles)
        {
            if (toggle != null)
            {
                if (toggle.isOn)
                {
                    allOff = false;
                }
                else
                {
                    allOn = false;
                }
            }
        }

        if (allOn)
        {
            mainToggle.isOn = true;
        }
        else if (allOff)
        {
            mainToggle.isOn = false;
        }
        else
        {
            mainToggle.SetIsOnWithoutNotify(false);
        }
    }

    public void AddControlledToggle(Toggle toggle)
    {
        if (!controlledToggles.Contains(toggle))
        {
            controlledToggles.Add(toggle);
            toggle.onValueChanged.AddListener(OnControlledToggleValueChanged);
            UpdateMainToggleState();
        }
    }

    public void RemoveControlledToggle(Toggle toggle)
    {
        if (controlledToggles.Remove(toggle))
        {
            toggle.onValueChanged.RemoveListener(OnControlledToggleValueChanged);
            UpdateMainToggleState();
        }
    }
}