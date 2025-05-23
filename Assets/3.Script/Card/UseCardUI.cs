using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCardUI : MonoBehaviour
{
    public static UseCardUI Instance;
    //public List<int> cardIndex = new List<int>();
    
    private void Awake()
    {
        Instance = this;
    }

    public void OnCardClicked(int index)
    {
        UIManager.Instance.cardListPanel.SetActive(false);
        
        var broadCaster = FindObjectOfType<Broadcaster>();
        broadCaster.RPC_RequestUseCardList(BasicSpawner.Instance._runner.LocalPlayer, index);
    }
}
