using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelSynchronizer : MonoBehaviour
{
    public static UIPanelSynchronizer Instance;
    
    public GameObject selectCardPanel;

    private void Awake()
    {
        Instance = this;
    }
}
