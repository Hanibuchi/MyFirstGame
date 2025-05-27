using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using Zenject;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }
	UIStackManager m_uistackManager = new();

	/// <summary>
	/// 直接ここから取得してはいけない。GetUIPageを介す。
	/// </summary>
	Dictionary<string, IUIPage> m_uiPages = new();
    [Inject] IResourceManager m_resourceManager;

	IUIPage GetUIPage(UIPageType ui)
	{
		if (!m_uiPages.TryGetValue(ui.ToString(), out var page))
		{
			var obj = m_resourceManager.GetOther(ui.ToString());
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
		MyInputSystem.GameInputs.UI.Cancel.canceled += OnCancelButtonPushed;
	}


	public void Show(UIPageType ui)
	{
		var page = GetUIPage(ui);
		if (!page.IsPermanent) // IsPermanentがfalseのUIの処理はUIStackManagerに任せる。
			m_uistackManager.PushPage(page);
		else
			page.Show();
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
	protected virtual void OnCancelButtonPushed(InputAction.CallbackContext callback)
	{
		Back();
	}
	public void Back()
	{
		m_uistackManager.PopPage();
	}

	public void CloseAll()
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
	KeyBindingsUI,
	NewGameUI,
	LanguageUI,
	DeleteCautionUI,
	MessageUI,
}
