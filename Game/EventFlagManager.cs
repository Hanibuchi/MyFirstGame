using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EventFlagManager : MonoBehaviour
{
    string EventFlagsPath => Path.Combine(GameManager.PlayerDataPath, "EventFlags");
    public void Save()
    {

    }
}
