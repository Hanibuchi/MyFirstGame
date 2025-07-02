using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PartyMember : MonoBehaviour
{
    public bool IsLeader { get; private set; }
    MemberEquipmentUI m_memberEquipmentUI;
    public Party Party { get; private set; }

    LevelHandler m_levelHandler;
    [SerializeField] SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        m_levelHandler = GetComponent<LevelHandler>();
        if (TryGetComponent(out DeathHandler deathHandler))
        {
            deathHandler.OnDead += OnDead;
        }
        if (_spriteRenderer == null)
            Debug.LogWarning("spriteRenderer is null");
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


    public MemberEquipmentUI GetMemberUI()
    {
        if (m_memberEquipmentUI == null)
        {
            this.m_memberEquipmentUI = ResourceManager.Instance.GetOther(ResourceManager.UIID.MemberEquipmentUI.ToString()).GetComponent<MemberEquipmentUI>();

            var itemUser = GetComponent<ItemUser>();
            m_memberEquipmentUI.SetItemUser(itemUser);
            m_memberEquipmentUI.RegisterStatus(gameObject);
        }
        return m_memberEquipmentUI;
    }
    void DestroyMemberUI()
    {
        if (m_memberEquipmentUI == null)
            return;

        ResourceManager.Instance.ReleaseOther(ResourceManager.UIID.MemberEquipmentUI.ToString(), m_memberEquipmentUI.gameObject);
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

    public Sprite GetImage()
    {
        if (_spriteRenderer != null)
        {
            return _spriteRenderer.sprite;
        }
        return null;
    }
    public event Action<Sprite> OnNPCImageChanged;

    Sprite previousSprite;
    void Update()
    {
        if (_spriteRenderer != null && _spriteRenderer.sprite != previousSprite)
        {
            OnNPCImageChanged?.Invoke(_spriteRenderer.sprite);
            previousSprite = _spriteRenderer.sprite;
        }
    }
}
