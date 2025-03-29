using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClass2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestClass.Instance.SpawnItem();
        TestClass.Instance.SpawnMob();
        TestClass.Instance.SpawnMob();
    }
}
