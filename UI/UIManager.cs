using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }
	EquipmentMenuManager equipmentMenuManager;
	public EquipmentMenuManager EquipmentMenuManager => equipmentMenuManager;

	PlayerStatusUI playerStatusUI;
	/// <summary>
	/// プレイヤーのステータスを表示する画面。
	/// </summary>
	public PlayerStatusUI PlayerStatusUI => playerStatusUI;

	Inventory inventory;
	public Inventory Inventory => inventory;

	TitleUI titleUI;
	PauseUI pauseUI;
	SaveMenuUI saveMenuUI;
	AchievementsUI achievementsUI;
	SettingsUI settingsUI;
	StatisticsUI statisticsUI;

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

	public void OnGameStart()
	{
	}
	public void OnReturnToTitle()
	{
	}

	public void SetEquipmentMenuManager(EquipmentMenuManager equipmentMenuManager)
	{
		this.equipmentMenuManager = equipmentMenuManager;
	}
	public void SetPlayerStatusUI(PlayerStatusUI playerStatusUI)
	{
		this.playerStatusUI = playerStatusUI;
	}
	public void SetInventory(Inventory inventory)
	{
		this.inventory = inventory;
	}


	public void Open(UIType ui)
	{
		switch (ui)
		{
			case UIType.PlayerStatusUI: PlayerStatusUI.Open(); break;
			case UIType.EquipmentMenu: EquipmentMenuManager.Open(); break;
			case UIType.InventoryUI: Inventory.Open(); break;

			case UIType.TitleUI:
				if (titleUI == null)
					titleUI = ResourceManager.GetOther(ResourceManager.UIID.TitleUI.ToString()).GetComponent<TitleUI>();
				titleUI.Open();
				break;

			case UIType.PauseUI:
				if (pauseUI == null)
					pauseUI = ResourceManager.GetOther(ResourceManager.UIID.PauseUI.ToString()).GetComponent<PauseUI>();
				pauseUI.Open();
				break;

			case UIType.SaveMenu:
				if (saveMenuUI == null)
					saveMenuUI = ResourceManager.GetOther(ResourceManager.UIID.SaveMenuUI.ToString()).GetComponent<SaveMenuUI>();
				saveMenuUI.Open();
				break;

			case UIType.AchievementsUI:
				if (achievementsUI == null)
					achievementsUI = ResourceManager.GetOther(ResourceManager.UIID.AchievementsUI.ToString()).GetComponent<AchievementsUI>();
				achievementsUI.Open();
				break;

			case UIType.SettingsUI:
				if (settingsUI == null)
					settingsUI = ResourceManager.GetOther(ResourceManager.UIID.SettingsUI.ToString()).GetComponent<SettingsUI>();
				settingsUI.Open();
				break;

			case UIType.StatisticsUI:
				if (statisticsUI == null)
					statisticsUI = ResourceManager.GetOther(ResourceManager.UIID.StatisticsUI.ToString()).GetComponent<StatisticsUI>();
				statisticsUI.Open();
				break;
		}
	}
	public void Close(UIType ui)
	{
		switch (ui)
		{
			case UIType.PlayerStatusUI: PlayerStatusUI?.Close(); break;
			case UIType.EquipmentMenu: EquipmentMenuManager?.Close(); break;
			case UIType.InventoryUI: Inventory?.Close(); break;
			case UIType.SaveMenu: saveMenuUI?.Close(); break;
			case UIType.TitleUI: titleUI?.Close(); break;
			case UIType.PauseUI: pauseUI?.Close(); break;
			case UIType.AchievementsUI: achievementsUI?.Close(); break;
			case UIType.SettingsUI: settingsUI.Close(); break;
			case UIType.StatisticsUI: statisticsUI.Close(); break;
		}
	}
	public enum UIType
	{
		None,
		PlayerStatusUI,
		EquipmentMenu,
		InventoryUI,
		SaveMenu,
		TitleUI,
		PauseUI,
		AchievementsUI,
		SettingsUI,
		StatisticsUI,
	}
}
