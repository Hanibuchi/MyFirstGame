using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] TMP_Text messageText;
    [SerializeField] Button okButton;
    event Action callback;
    private void Awake()
    {
        okButton.onClick.AddListener(Close);
    }
    public void Open(string message, Action callback)
    {
        messageText.text = message;
        this.callback = callback;
    }
    public void Open(string message)
    {
        messageText.text = message;
    }
    void Close()
    {
        ResourceManager.ReleaseOther(ResourceManager.UIID.MessageUI.ToString(), gameObject);
        callback?.Invoke();
    }
}
