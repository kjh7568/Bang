using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_HumanList", menuName = "Card/Human/_HumanList")]
public class HumanList : ScriptableObject
{
    public List<HumanData> humanList;
    
    public Sprite GetHumanSpriteByName(string humanName)
    {
        foreach (var data in humanList)
        {
            if (data.Name == humanName)
                return data.CardSprite;
        }

        return null;
    }
}
