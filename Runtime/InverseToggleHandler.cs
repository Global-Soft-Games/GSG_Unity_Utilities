using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InverseToggleHandler : MonoBehaviour
{
    public UnityEvent<bool> OnInverseToggle;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        if (toggle == null)
        {
            Debug.LogError("沒有找到 Toggle 組件！");
            return;
        }

        toggle.onValueChanged.AddListener(HandleToggleValueChanged);
    }

    private void HandleToggleValueChanged(bool isOn)
    {
        bool inverseValue = !isOn;
        OnInverseToggle.Invoke(inverseValue);
    }

    // 公開方法，用於獲取當前的反轉值
    public bool GetInverseValue()
    {
        return !toggle.isOn;
    }
}