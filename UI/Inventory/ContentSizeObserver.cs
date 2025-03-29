using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class ContentSizeObserver : MonoBehaviour
    {
        // このスクリプトがアタッチされているcontentのサイズが変更される度，LowerSectionの大きさを調節する。
        private void OnRectTransformDimensionsChange()
        {
            if (transform.parent.parent.TryGetComponent<FixedViewportSizeAdjuster>(out var granpaAjuster))
            {
                granpaAjuster.FixViewportSize();
            }
            else
            {
                Debug.Log("Viewport position may be changed.");
            }
        }
    }
}
