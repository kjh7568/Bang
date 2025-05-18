using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_HumanList", menuName = "Card/Human/_HumanList")]
public class HumanList : ScriptableObject
{
    public List<HumanData> humanList;
}
