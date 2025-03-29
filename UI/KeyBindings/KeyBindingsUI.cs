using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class KeyBindingsUI : MonoBehaviour
{
	public int MaxKeyNum { get; private set; } = 4;
	public delegate void RebindUIDelegate(MyInputSystem.ActionType actionName, int index);
	[Serializable]
	class ActionNamesAndKeyBindUIPair
	{
		public MyInputSystem.ActionType ActionType;
		public KeyBindingEntryUI KeyBindUI;
	}
	[SerializeField] List<ActionNamesAndKeyBindUIPair> ActionNamesAndKeyBindUIPairs = new();
	readonly Dictionary<MyInputSystem.ActionType, KeyBindingEntryUI> KeyBindingEntryUIs = new();

	RebindUIDelegate OnStartRebind;
	KeyBindingEntryUI rebindingActionUI;

	// このインスタンスが生成されるタイミングをControllerが制御できないため致し方なくUIのほうからControllerを参照した。MVC的にはよくない。
	private void Awake()
	{
		KeyBindingsController.Instance.SetUI(this);
	}

	/// <summary>
	/// 初期化する。一度しか実行しない
	/// </summary>
	/// <param name="onStartReind"></param>
	public void Init(RebindUIDelegate onStartReind)
	{
		OnStartRebind = onStartReind;

		foreach (var actionKeyBindPair in ActionNamesAndKeyBindUIPairs)
		{
			actionKeyBindPair.KeyBindUI.Init(this, actionKeyBindPair.ActionType
			, MaxKeyNum);
			KeyBindingEntryUIs[actionKeyBindPair.ActionType] = actionKeyBindPair.KeyBindUI;
		}
	}

	public void StartRebind(MyInputSystem.ActionType action, int index)
	{
		OnStartRebind.Invoke(action, index);
		rebindingActionUI = KeyBindingEntryUIs[action];
		// ここでキーを押すよう促すメッセージに切り替える。
	}

	public void EndRebind(string keyPath)
	{
		rebindingActionUI.EndRebind(keyPath);
		CleanUpOperation();
	}


	public void Cancel()
	{
		rebindingActionUI.Cancel();
		CleanUpOperation();
	}

	public void Delete()
	{
		rebindingActionUI.Delete();
		CleanUpOperation();
	}

	public void ResetKeyBind(string keyPath)
	{
		rebindingActionUI.ResetKeyBind(keyPath);
		CleanUpOperation();
	}

	void CleanUpOperation()
	{
		rebindingActionUI = null;
	}
}
