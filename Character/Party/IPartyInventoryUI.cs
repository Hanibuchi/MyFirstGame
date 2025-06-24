using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPartyInventoryUI
{
    void SetParty(Party party);
    void UpdateInventoryUI(List<IChildItemHolder> itemHolders);
}
