using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusSlotsFrame : MonoBehaviour
{
    /// <summary>
    /// ステータスキャンバスのアイテムスロットを更新する。中に入れたオブジェクトを複製して子オブジェクトにする。
    /// </summary>
    /// <param name="equipmentSlotsObject"></param>
    public void UpdateItemSlots(GameObject equipmentSlotsObject)
    {
        if (transform.childCount > 0)
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);

        GameObject copy = Instantiate(equipmentSlotsObject);
        copy.transform.SetParent(transform);
    }
}
