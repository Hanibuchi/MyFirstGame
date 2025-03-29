using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// Viewportの最大値を設定する。contentのwidthがMaxViewportWidthより大きい時LowerSectionのLayoutGroupのControlChildSizeのwidthをfalseにしてViewportのwidthをMaxViewportWidthにするスクリプトを作成する。
[RequireComponent(typeof(ScrollRect))]
[RequireComponent(typeof(VerticalLayoutGroup))]
public class FixedViewportSizeAdjuster : MonoBehaviour
{
    [SerializeField]
    float MaxViewportWidth;
    
    // ここに書いてもわからないかもしれないが，contentのFitterの注は無視していい。contentのheightだけviewportのlayoutGroupから設定して，widthは自身のFitterで設定する。こうすることで，heightは常にviewportと同じになり，widthは一定以上だとviewportとは別の大きさになるようにできる。
    public void FixViewportSize()
    {
        ScrollRect scrollRect = GetComponent<ScrollRect>();
        if (scrollRect.content.GetComponent<RectTransform>().rect.width > MaxViewportWidth)
        {
            GetComponent<VerticalLayoutGroup>().childControlWidth = false;
            RectTransform viewportRect = scrollRect.viewport.GetComponent<RectTransform>();
            viewportRect.sizeDelta = new Vector2(MaxViewportWidth, viewportRect.sizeDelta.y);

            RectTransform scrollbarRect = scrollRect.horizontalScrollbar.GetComponent<RectTransform>();
            scrollbarRect.sizeDelta = new Vector2(MaxViewportWidth, scrollbarRect.sizeDelta.y);
        }
        else
        {
            GetComponent<VerticalLayoutGroup>().childControlWidth = true;
        }
    }
}
