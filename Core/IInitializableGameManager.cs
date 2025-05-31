using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitializableGameManager
{
    void NewGame(InitGameData initGameData);
    void LoadGame(string saveSlotName);
    void OnReturnToTitle();
    void Save();
}
