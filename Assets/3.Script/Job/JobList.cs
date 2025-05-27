using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_JobList", menuName = "Card/Job/_JobList")]
public class JobList : ScriptableObject
{
    public List<Job> jobList;
    
    public Sprite GetJobSpriteByName(string jobName)
    {
        foreach (var data in jobList)
        {
            if (data.Name == jobName)
                return data.CardSprite;
        }

        return null;
    }
}
