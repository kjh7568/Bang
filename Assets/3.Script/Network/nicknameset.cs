using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class nicknameset : MonoBehaviour
{
    [SerializeField] private TMP_Text nickname;
    
    public void SetNickname(string name)
    {
        nickname.text = name;
    }
    
}
