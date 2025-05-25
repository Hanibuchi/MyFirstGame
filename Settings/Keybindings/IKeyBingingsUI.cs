using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKeyBindingsUI
{
	void Init(RebindUIDelegate onStartReind);
	void EndRebind(string key);
	void Cancel();
	void Delete();
	void ResetKeyBind(string keyPath);
}
