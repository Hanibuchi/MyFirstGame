using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementEntryUI : UI
{
	[SerializeField] string achievementName;
	[SerializeField] Rarity rarity;
	[SerializeField] bool isAchieved;
	[SerializeField] bool isRewardEnabled;
	[SerializeField] Image image;
	[SerializeField] Color disableColor = new(0.5f, 0.5f, 0.5f, 1.0f);
	public void Set(string name, AchievementData achievementData)
	{
		achievementName = name;
		rarity = achievementData.Rarity;
		isAchieved = achievementData.isAchieved;
		isRewardEnabled = achievementData.isRewardEnabled;
		if (achievementData.isAchieved)
		{
			image.color = new(1.0f, 1.0f, 1.0f, 1.0f);
		}
		else
		{
			image.color = disableColor;
		}
		image.sprite = achievementData.Sprite;
	}
	public override void OnRelease()
	{
		base.OnRelease();
		transform.SetParent(null);
	}
}
