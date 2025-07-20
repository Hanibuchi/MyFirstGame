using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropHandler : MonoBehaviour
{
    List<DropItem> m_dropItems = new();
    List<DropItem> m_additionalDropitems = new();
    public static float m_dropMoneyBase = 1.5f;
    float m_dropMoneyScale = 1.0f;


    public static float m_expBase = 1.5f;
    float m_expScale = 1.0f;


    LevelHandler m_levelHandler;
    ItemUser m_itemUseHandler;
    Rigidbody2D m_rigidbody2d;

    void Awake()
    {
        m_levelHandler = GetComponent<LevelHandler>();
        m_itemUseHandler = GetComponent<ItemUser>();
        m_rigidbody2d = GetComponent<Rigidbody2D>();
        if (TryGetComponent(out DeathHandler deathHandler))
            deathHandler.OnDead += OnDead;
        if (TryGetComponent(out PoolableResourceComponent poolableResourceComponent))
            poolableResourceComponent.ReleaseCallback += OnReset;
    }
    public void Initialize(DropData dropData)
    {
        m_additionalDropitems.Clear();
        m_dropItems = dropData.dropItem;
        m_dropMoneyScale = dropData.dropMoneyScale;
        m_expScale = dropData.expScale;
    }

    public virtual float CalculateExperience()
    {
        if (m_levelHandler != null)
            return m_expScale * Mathf.Pow(m_expBase, m_levelHandler.Level);
        else
            return m_expScale;
    }

    public void AddItemsToDrop(List<IItem> items)
    {
        foreach (var item in items)
        {
            if (item is MonoBehaviour mono && mono.TryGetComponent(out ResourceComponent resourceComponent))
            {
                m_additionalDropitems.Add(new DropItem(1.0f, resourceComponent.ID));
            }
        }
    }
    public virtual float CalculateDropMoney()
    {
        if (m_levelHandler != null)
            return m_dropMoneyScale * Mathf.Pow(m_dropMoneyBase, m_levelHandler.Level);
        else
            return m_dropMoneyScale;
    }
    public void Drop()
    {
        DropSub(m_dropItems);
        DropSub(m_additionalDropitems);
        DropMoney();
    }
    void DropSub(List<DropItem> dropItems)
    {
        foreach (DropItem dropItem in dropItems)
        {
            if (Random.Randoms[RandomName.DropItem.ToString()].Value() <= dropItem.DropRate)
            {
                GameObject obj = ResourceManager.Instance.GetItem(dropItem.ItemName);
                if (obj != null && obj.TryGetComponent(out Rigidbody2D rb))
                {
                    if (m_rigidbody2d != null)
                        rb.velocity = m_rigidbody2d.velocity;
                    else
                        rb.velocity = Vector2.zero;
                }
            }
        }
    }
    void DropMoney() { CalculateDropMoney(); }

    void OnDead()
    {
        if (m_itemUseHandler != null)
        {
            AddItemsToDrop(m_itemUseHandler.ItemHolder.Items.Select(itemHolder => itemHolder.GetItem()).ToList());
        }

        Drop();
    }
    void OnReset()
    {
        m_additionalDropitems.Clear();
    }
}
