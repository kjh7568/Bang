using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCard : MonoBehaviour
{
    public static UseCard Instance;
    public List<int> cardIndex = new List<int>();
    
    private void Awake()
    {
        Instance = this;
    }

    public void OnCardClicked(int index)
    {
        for (int i = 0; i < cardIndex.Count; i++)
        {
            if (cardIndex[i] == index)
            {
                cardIndex.Remove(index);
            }
            else
            {
                cardIndex.Add(index);
            }
        }
        
        Debug.Log("선택된 카드 Index :: " + index);
    }
}
