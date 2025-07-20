using System;
using System.Data.Common;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class ResourceComponent : MonoBehaviour, IResourceComponent, ISerializableComponent
{
	[SerializeField][JsonProperty] ResourceType m_type;
	public ResourceType Type => m_type;
	[SerializeField][JsonProperty] string m_id;
	public string ID => m_id;
	public event Action GetCallback;

	public void OnGet(ResourceType type, string id)
	{
		m_type = type;
		m_id = id;
		GetCallback?.Invoke();
	}

	private void Start()
	{
		GetCallback?.Invoke(); // Awake()にて各コンポネントがGetCallbackにメソッドを登録するため初回は登録されていないのにOnGetでInvokeしてしまう。そこで初回のみStart()にてInvokeする。
	}
}