using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using Zenject;

public class UIManager : MonoBehaviour, IInitializableUIManager
{
	public static UIManager Instance { get; private set; }
	UIStackManager m_uistackManager = new();

	/// <summary>
	/// 直接ここから取得してはいけない。GetUIPageを介す。
	/// </summary>
	Dictionary<string, IUIPage> m_uiPages = new();

	IUIPage GetUIPage(UIPageType ui)
	{
		if (!m_uiPages.TryGetValue(ui.ToString(), out var page))
		{
			var obj = ResourceManager.Instance.GetOther(ui.ToString());
			if (obj.TryGetComponent(out page))
			{
				page.Init();
				page.HideImd();
				if (page is MonoBehaviour mono)
				{
					DontDestroyOnLoad(mono.gameObject);
				}
				m_uiPages.Add(ui.ToString(), page);
			}
			else
			{
				Debug.LogWarning($"UIPage {ui.ToString()} does not have IUIPage component");
				return null;
			}
		}
		return page;
	}


	/// <summary>
	/// アプリ開始時呼び出される
	/// </summary>
	public void OnAppStart()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}


	public void Show(UIPageType ui)
	{
		var page = GetUIPage(ui);
		if (page.IsPermanent) // IsPermanentがfalseのUIの処理はUIStackManagerに任せる。
			page.Show();
		else
			m_uistackManager.PushPage(page);
	}

	public void Hide(UIPageType ui)
	{
		var page = GetUIPage(ui);
		if (page == null)
		{
			Debug.LogWarning($"UIPage {ui.ToString()} does not exist");
			return;
		}

		if (page.IsPermanent)
		{
			page.Hide();
		}
		else
			Debug.LogWarning($"UIPage {ui.ToString()} is not permanent. Cannot hide it directly. Please use Back() method.");
	}
	public bool escapeButtonMask;
	public virtual void OnEscapeButtonPushed()
	{
		Debug.Log("escape button pushed");
		if (escapeButtonMask)
		{
			Debug.Log("escapeButton masked");
			return;
		}
		if (m_uistackManager.CurrentPage == null)
			Show(UIPageType.PauseUI);
		else
			Back();
	}
	public void Back()
	{
		m_uistackManager.PopPage();
	}

	public void CloseAllStack()
	{
		m_uistackManager.CloseAll();
	}

	public IPlayerStatusUI GetPlayerStatusUI()
	{
		var page = GetUIPage(UIPageType.PlayerStatusUI);
		if (page != null)
			return page as IPlayerStatusUI;
		else
			return null;
	}

	public IEquipmentUI GetEquipmentUI()
	{
		var page = GetUIPage(UIPageType.EquipmentUI);
		if (page != null)
			return page as EquipmentUI;
		else
			return null;
	}

	public InventoryUI GetInventoryUI()
	{
		var page = GetUIPage(UIPageType.InventoryUI);
		if (page != null)
			return page as InventoryUI;
		else
			return null;
	}

	public INewGameUI GetNewGameUI()
	{
		var page = GetUIPage(UIPageType.NewGameUI);
		if (page != null)
			return page as NewGameUI;
		else
			return null;
	}
	public IDeleteCautionUI GetDeleteCautionUI()
	{
		var page = GetUIPage(UIPageType.DeleteCautionUI);
		if (page != null)
			return page as DeleteCautionUI;
		else
			return null;
	}
	public IMessageUI GetMessageUI()
	{
		var page = GetUIPage(UIPageType.MessageUI);
		if (page != null)
			return page as IMessageUI;
		else
			return null;
	}
}


public enum UIPageType
{
	None,
	PlayerStatusUI,
	EquipmentUI,
	InventoryUI,
	SaveMenuUI,
	TitleUI,
	PauseUI,
	AchievementsUI,
	SettingsUI,
	StatisticsUI,
	NewGameUI,
	LanguageUI,
	DeleteCautionUI,
	MessageUI,
}
