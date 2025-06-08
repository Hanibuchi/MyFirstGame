using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public class Test_Item : MonoBehaviour
{
    [SerializeField] Item item;
    [SerializeField] MobManager mob;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] Damage damage;
    public void Test_Fire()
    {
        var shot = CreateShot();
        Debug.Log($"Test_Item, Test_Fire, shot: {shot}");
        item.FirstFire(shot);
    }

    public Shot CreateShot()
    {
        Shot shot = new();
        shot.SetCore(mob, item.gameObject, GameManager.Utility.GetMousePos(), targetLayer, damage);
        return shot;
    }
}
