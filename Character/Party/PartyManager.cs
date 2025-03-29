using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    static public PartyManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public Party Add()
    {
        var party = gameObject.AddComponent<Party>();
        party.Init();
        return party;
    }

    public void Delete(Party party)
    {
        Destroy(party);
    }
}
