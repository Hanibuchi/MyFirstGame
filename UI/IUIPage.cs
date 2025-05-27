using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIPage
{
    bool IsRoot { get; }
    bool IsPermanent { get; }
    void Init();
    void Show();
    void Hide();
    void HideImd();
}
