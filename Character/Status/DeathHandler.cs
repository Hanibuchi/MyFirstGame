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
	Attack m_lastDamageTaker;
	public event Action OnDead;
	Rigidbody2D m_rigidbody2D;
	RigidbodyConstraints2D m_originalConstraints;

	private void Awake()
	{
		if (TryGetComponent(out PoolableResourceComponent poolableResourceComponent))
		{
			poolableResourceComponent.ReleaseCallback += OnRelease;
		}

		if (TryGetComponent(out m_rigidbody2D))
			m_originalConstraints = m_rigidbody2D.constraints;
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

		if (m_lastDamageTaker != null && m_lastDamageTaker.TryGetComponent(out LevelHandler levelHandler) && TryGetComponent(out DropHandler dropHandler))
		{
			levelHandler.AddExperience(dropHandler.CalculateExperience());
		}


		HandleDeadBody();

		OnDead?.Invoke();
	}
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
		m_isDead = false;
		if (m_rigidbody2D != null)
			m_rigidbody2D.constraints = m_originalConstraints;
		transform.rotation = Quaternion.identity;
	}
}
