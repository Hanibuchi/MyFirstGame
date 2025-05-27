using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitializableResourceManager
{
    void OnAppStart(Action callback);
}
