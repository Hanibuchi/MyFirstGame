using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MyGame;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBindingsController : IKeyBindingsController
{
	string KeyBindingsPath => Path.Combine(ApplicationManager.ConfigDirectoryPath, "KeyBindings");
	[SerializeField] IKeyBindingsUI m_keyBindingsUI;
	public void SetKeybindingsUI(IKeyBindingsUI keyBindingsUI)
	{
		m_keyBindingsUI = keyBindingsUI;
		m_keyBindingsUI.Init(StartRebind);
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
		m_keyBindingsUI.EndRebind(inputAction.bindings[index].effectivePath);
	}

	void Cancel(InputAction inputAction, int index)
	{
		m_keyBindingsUI.Cancel();
	}

	void Delete(InputAction inputAction, int index)
	{
		m_keyBindingsUI.Delete();
	}

	void ResetKeyBind(InputAction inputAction, int index)
	{
		m_keyBindingsUI.ResetKeyBind(inputAction.bindings[index].effectivePath);
	}
}
