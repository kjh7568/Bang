using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanData : ScriptableObject, IHuman
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Sprite cardSprite;
    
    public string Name => name;
    public string Description => description;
    public Sprite CardSprite => cardSprite;

    public virtual void UseAbility()
    {
    }
}