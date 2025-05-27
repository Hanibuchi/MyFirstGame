using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIStackManager
{
    private Stack<IUIPage> pageStack = new();


    public void PushPage(IUIPage newPage)
    {
        if (pageStack.Count > 0)
            pageStack.Peek().Hide();

        pageStack.Push(newPage);
        newPage.Show();
    }

    public void PopPage()
    {
        if (pageStack.Count == 0) return;
        if (pageStack.Peek().IsRoot) return;

        pageStack.Pop().Hide();

        if (pageStack.Count > 0)
            pageStack.Peek().Show();
    }

    /// <summary>
    /// すべてのページをHideImdし，Stackをリセットする。
    /// </summary>
    public void CloseAll()
    {
        while (pageStack.Count != 0)
        {
            pageStack.Pop().HideImd();
        }
    }

    public IUIPage CurrentPage => pageStack.Count > 0 ? pageStack.Peek() : null;
}
