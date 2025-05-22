using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCard : MonoBehaviour
{
    
    public void OnCardClicked(int index)
    {
        Debug.Log("선택된 카드 Index :: " + index);
    }
}
