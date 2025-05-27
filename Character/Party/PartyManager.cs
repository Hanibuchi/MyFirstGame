using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class PartyManager : MonoBehaviour
{
    static public PartyManager Instance { get; private set; }
    [Inject] IResourceManager m_resourceManager;
    [SerializeField] PlayerParty m_playerParty;
    public PlayerParty PlayerParty => m_playerParty;

    List<Party> m_partyList = new();
    public List<Party> PartyList => m_partyList;
    public void OnGameStart()
    {
        Instance = this;
        PlayerParty.OnGameStart();
    }

    public void Save()
    {
        PlayerParty.Save();
    }

    public Party GetParty()
    {
        GameObject partyObj = m_resourceManager.GetOther(ResourceManager.OtherID.Party.ToString());
        partyObj.transform.SetParent(transform);
        Party party = partyObj.GetComponent<Party>();
        party.Init();
        m_partyList.Add(party);
        return party;
    }

    public void ReleaseParty(Party party)
    {
        if (m_partyList.Contains(party))
        {
            m_partyList.Remove(party);
        }
        else
        {
            Debug.LogError("Party not found in party list");
            return;
        }
        m_resourceManager.ReleaseOther(ResourceManager.OtherID.Party.ToString(), party.gameObject);
    }
}
