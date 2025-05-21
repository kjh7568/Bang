using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public GameObject cardListPanel;
    public GameObject waitingPanel;

    public TMP_Text waitingUserTurnText;

    //public List<Image> handCardImageList;
    
    private void Awake()
    {
        Instance = this;
    }
}
