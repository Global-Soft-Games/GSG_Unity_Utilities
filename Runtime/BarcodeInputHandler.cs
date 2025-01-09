using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JOSAM.Utility
{
    [RequireComponent(typeof(InputField))]
    public class BarcodeInputHandler : MonoBehaviour
    {
        private InputField barcodeInputField;  // 用於顯示條碼的輸入框

        private void Awake()
        {
            barcodeInputField = GetComponent<InputField>();
        }

        private void Start()
        {
            if (barcodeInputField == null)
            {
                Debug.LogError("請在檢查器中指定 InputField!");
                return;
            }

            // 添加輸入框的值改變監聽器
            barcodeInputField.onEndEdit.AddListener(OnBarcodeInputComplete);
        }

        private void OnBarcodeInputComplete(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Debug.Log($"接收到條碼: {value}");
            }
        }

        // 手動設置條碼值的方法（可用於外部調用）
        public void SetBarcodeValue(string barcode)
        {
            if (barcodeInputField != null)
            {
                barcodeInputField.text = barcode;
            }
        }

        private void OnDestroy()
        {
            if (barcodeInputField != null)
            {
                barcodeInputField.onEndEdit.RemoveListener(OnBarcodeInputComplete);
            }
        }
    }
}