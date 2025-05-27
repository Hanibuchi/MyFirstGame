using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class DeathHandler : MonoBehaviour
{
	[SerializeField] bool m_isDead;
	public bool IsDead => m_isDead;
	List<DropItem> m_dropItems = new();
	List<DropItem> m_additionalDropitems = new();
	float m_dropMoney;
	public static float m_dropMoneyBase = 1.5f;
	float m_dropMoneyScale = 1.0f;
	Attack m_lastDamageTaker;
	public event Action OnDead;

	public static float m_expBase = 1.5f;
	float m_expScale = 1.0f;
	PoolableResourceComponent m_poolableResourceComponent;
	Rigidbody2D m_rigidbody2D;
	RigidbodyConstraints2D m_originalConstraints;
	LevelHandler m_levelHandler;
	// ItemUseHandler m_itemUseHandler;
    [Inject] IResourceManager m_resourceManager;

	private void Awake()
	{
		if (TryGetComponent(out m_poolableResourceComponent))
		{
			m_poolableResourceComponent.ReleaseCallback += OnRelease;
		}
		if (TryGetComponent(out m_rigidbody2D))
			m_originalConstraints = m_rigidbody2D.constraints;
		m_levelHandler = GetComponent<LevelHandler>();
		// m_itemUseHandler = GetComponent<ItemUseHandler>();
	}

	public void Initialize(DeathData deathData)
	{
		m_dropItems = deathData.dropItem;
		m_dropMoneyScale = deathData.dropMoneyScale;
		m_expScale = deathData.expScale;
	}

	public void SetLastDamageTaker(Attack attack)
	{
		m_lastDamageTaker = attack;
	}

	public virtual void Die()
	{
		if (m_isDead)
		{
			Debug.Log("this character is already dead");
			return;
		}

		m_isDead = true;

		if (m_lastDamageTaker != null && m_lastDamageTaker.TryGetComponent(out LevelHandler levelHandler))
		{
			levelHandler.AddExperience(CalculateExperience());
		}

		// if (m_itemUseHandler != null)
		// {
		// 	AddItemsToDrop(m_itemUseHandler.Items);
		// }

		m_dropMoney = CalculateDropMoney();
		Drop();

		HandleDeadBody();

		OnDead?.Invoke();
	}

	protected virtual float CalculateExperience()
	{
		if (m_levelHandler != null)
			return m_expScale * Mathf.Pow(m_expBase, m_levelHandler.Level);
		else
			return m_expScale;
	}

	public void AddItemsToDrop(List<Item> items)
	{
		foreach (Item item in items)
		{
			if (item != null && item.TryGetComponent(out ResourceComponent resourceComponent))
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
				GameObject obj = m_resourceManager.GetItem(dropItem.ItemName);
				if (obj != null && obj.TryGetComponent(out Rigidbody2D rb))
				{
					if (m_rigidbody2D != null)
						rb.velocity = m_rigidbody2D.velocity;
					else
						rb.velocity = Vector2.zero;
				}
			}
		}
	}
	void DropMoney() { }
	public virtual void HandleDeadBody()
	{
		if (m_rigidbody2D != null)
		{
			m_rigidbody2D.freezeRotation = false;
		}
	}
	public string CreateCauseOfDeath()
	{
		if (m_lastDamageTaker != null)
		{
			return $"You were killed by {m_lastDamageTaker.gameObject.name}";
		}
		else
		{
			return "You dead";
		}
	}
	void OnRelease()
	{
		if (m_rigidbody2D != null)
			m_rigidbody2D.constraints = m_originalConstraints;
		m_isDead = false;
		transform.rotation = Quaternion.identity;
	}
}
