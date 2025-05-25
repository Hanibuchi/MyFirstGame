using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }
	UIStackManager m_uistackManager = new();

	Dictionary<string, IUIPage> m_uiPages = new();


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

	public void OnGameStart()
	{
	}
	public void OnReturnToTitle()
	{
	}


	public void Show(UIPageType ui)
	{
		if (!m_uiPages.TryGetValue(ui.ToString(), out var page))
		{
			var obj = ResourceManager.GetOther(ui.ToString());
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
				return;
			}
		}
		if (!page.IsPermanent) // IsPermanentがfalseのUIの処理はUIStackManagerに任せる。
			m_uistackManager.PushPage(page);
		else
			page.Show();
	}

	public void Hide(UIPageType ui)
	{
		if (!m_uiPages.TryGetValue(ui.ToString(), out var page))
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
		m_uistackManager.CurrentPage?.OnBack();
		m_uistackManager.PopPage();
	}

	public void CloseAll()
	{
		m_uistackManager.CloseAll();
	}

	public IPlayerStatusUI GetPlayerStatusUI()
	{
		if (m_uiPages.TryGetValue(UIPageType.PlayerStatusUI.ToString(), out var page))
		{
			return page as IPlayerStatusUI;
		}
		else
		{
			Debug.LogWarning($"UIPage {UIPageType.PlayerStatusUI.ToString()} does not have IPlayerStatusUI component");
			return null;
		}
	}

	public IEquipmentUI GetEquipmentUI()
	{
		if (m_uiPages.TryGetValue(UIPageType.EquipmentUI.ToString(), out var page))
		{
			return page as IEquipmentUI;
		}
		else
		{
			Debug.LogWarning($"UIPage {UIPageType.EquipmentUI.ToString()} does not have IEquipmentUI component");
			return null;
		}
	}

	public InventoryUI GetInventoryUI()
	{
		if (m_uiPages.TryGetValue(UIPageType.InventoryUI.ToString(), out var page))
		{
			return page as InventoryUI;
		}
		else
		{
			Debug.LogWarning($"UIPage {UIPageType.InventoryUI.ToString()} does not have IItemParentUI component");
			return null;
		}
	}

	public INewGameUI GetNewGameUI()
	{
		if (m_uiPages.TryGetValue(UIPageType.NewGameUI.ToString(), out var page))
		{
			return page as INewGameUI;
		}
		else
		{
			Debug.LogWarning($"UIPage {UIPageType.NewGameUI.ToString()} does not have INewGameUI component");
			return null;
		}
	}
	public IDeleteCautionUI GetDeleteCautionUI()
	{
		if (m_uiPages.TryGetValue(UIPageType.DeleteCautionUI.ToString(), out var page))
		{
			return page as IDeleteCautionUI;
		}
		else
		{
			Debug.LogWarning($"UIPage {UIPageType.DeleteCautionUI.ToString()} does not have IDeleteCautionUI component");
			return null;
		}
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
}
