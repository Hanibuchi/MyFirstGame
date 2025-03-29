using MyGame;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackItemData", menuName = "ItemData/AttackItem", order = 0)]
public class AttackItemData : ItemData
{
	public ResourceManager.ProjectileID ProjectileID;
}