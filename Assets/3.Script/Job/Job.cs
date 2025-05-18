using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Job", menuName = "Card/Job/Job")]
public class Job : ScriptableObject, IJob
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Sprite cardSprite;

    public string Name => name;
    public string Description => description;
    public Sprite CardSprite => cardSprite;
}