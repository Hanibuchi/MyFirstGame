using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MyGame;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBindingsController : MonoBehaviour
{
	string KeyBindingsPath => Path.Combine(ApplicationManager.ConfigDirectoryPath, "KeyBindings");
	public static KeyBindingsController Instance { get; private set; }
	 KeyBindingsUI KeyBindingsUI;

	private void Awake()
	{
		Instance = this;
	}

	public void SetUI(KeyBindingsUI keyBindingsUI)
	{
		KeyBindingsUI = keyBindingsUI;
		KeyBindingsUI.Init(StartRebind);
	}

	/// <summary>
	/// 初期化する。一度しか実行しない
	/// </summary>
	public void OnAppStart()
	{
	}

	public void StartRebind(MyInputSystem.ActionType action, int index)
	{
		var inputAction = MyInputSystem.GameInputs.FindAction(action.ToString());
		if (inputAction == null)
		{
			Debug.LogWarning("cannot find this action");
			return;
		}
		MyInputSystem.Instance.StartRebind(inputAction, index, EndRebind, Cancel, Delete, ResetKeyBind);
	}

	void EndRebind(InputAction inputAction, int index)
	{
		KeyBindingsUI.EndRebind(inputAction.bindings[index].effectivePath);
	}

	void Cancel(InputAction inputAction, int index)
	{
		KeyBindingsUI.Cancel();
	}

	void Delete(InputAction inputAction, int index)
	{
		KeyBindingsUI.Delete();
	}

	void ResetKeyBind(InputAction inputAction, int index)
	{
		KeyBindingsUI.ResetKeyBind(inputAction.bindings[index].effectivePath);
	}
}
