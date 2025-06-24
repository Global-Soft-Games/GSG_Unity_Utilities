using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField]
    Text m_MessageText;

    public void ShowPopup(string message)
    {
        m_MessageText.text = message;
        gameObject.SetActive(true);
    }

    public void ShowPopup(Exception exception)
    {
        m_MessageText.text = exception.Message;
        gameObject.SetActive(true);
    }

    public void ShowPopup(string message, Exception exception)
    {
        m_MessageText.text = message;
        gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
