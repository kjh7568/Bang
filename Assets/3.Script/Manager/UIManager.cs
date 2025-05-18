using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public List<Image> handCardImageList;
    
    private void Awake()
    {
        Instance = this;
    }
}
