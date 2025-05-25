using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : MonoBehaviour
{
    public bool IsLeader { get; private set; }
    GameObject m_memberEquipmentUI;
    public Party Party { get; private set; }

    LevelHandler m_levelHandler;

    private void Awake()
    {
        m_levelHandler = GetComponent<LevelHandler>();
        if (TryGetComponent(out DeathHandler deathHandler))
        {
            deathHandler.OnDead += OnDead;
        }
    }

    public void OnJoinParty(Party party)
    {
        Party = party;
        transform.SetParent(party.transform);
    }
    public void OnLeaveParty()
    {
        Party = null;
        transform.SetParent(null);
        DestroyMemberUI();
    }


    public void OnBecomeLeader()
    {
        IsLeader = true;
        if (Party.IsPlayerParty)
        {
            if (GetComponent<PlayerController>() == null)
            {
                gameObject.AddComponent<PlayerController>();
            }
            UIManager.Instance.GetPlayerStatusUI()?.RegisterStatus(gameObject);
            GameManager.Instance.SetPlayer(gameObject);
        }
    }
    public void OnResignLeader()
    {
        IsLeader = false;
        if (TryGetComponent(out PlayerController playerController))
            Destroy(playerController);
    }


    public GameObject GetMemberUI()
    {
        if (m_memberEquipmentUI == null)
        {
            m_memberEquipmentUI = ResourceManager.GetOther(ResourceManager.UIID.MemberEquipmentUI.ToString());

            // アイテム所持するコンポネントを分離したらこれも分離する。
            var memberEquipmentUI = m_memberEquipmentUI.GetComponent<MemberEquipmentUI>();
            IItemParent itemParent = GetComponent<IItemParent>();
            memberEquipmentUI.SetItemParent(itemParent);
            memberEquipmentUI.RegisterStatus(gameObject);
        }
        return m_memberEquipmentUI;
    }
    void DestroyMemberUI()
    {
        if (m_memberEquipmentUI == null)
            return;

        ResourceManager.ReleaseOther(ResourceManager.UIID.MemberEquipmentUI.ToString(), m_memberEquipmentUI.gameObject);
        m_memberEquipmentUI = null;
    }

    void OnDead()
    {
        if (IsLeader)
            Party?.GameOver();
        else
            Party?.RemoveMember(this);
    }
    public void Surrender()
    {
        gameObject.layer = LayerMask.NameToLayer(GameManager.Layer.Ally.ToString());
        Party.RemoveMember(this);
    }

    public float GetHiringCost()
    {
        return m_levelHandler.BaseLevel * 10 + 100;
    }
}
