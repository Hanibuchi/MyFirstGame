using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : UIPageBase, IMessageUI
{
    [SerializeField] TMP_Text m_messageText;
    public void SetMessage(string message)
    {
        m_messageText.text = message;
    }
}
