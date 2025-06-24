using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMemberEquipmentUI
{
    void SetItemUser(ItemUser itemUser);
    void UpdateEquipmentUI(List<IChildItemHolder> itemHolders);
    void RegisterStatus(GameObject game);
}
