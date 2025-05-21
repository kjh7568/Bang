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
        if (cardIndex.Contains(index))
            cardIndex.Remove(index);
        else
            cardIndex.Add(index);

        Debug.Log("현재 선택된 카드 인덱스: " + string.Join(",", cardIndex));
    }
}
