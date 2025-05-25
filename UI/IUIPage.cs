using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIPage
{
    bool IsPermanent { get; }
    void Init();
    void Show();
    void Hide();
    void HideImd();
    void OnBack();
}
