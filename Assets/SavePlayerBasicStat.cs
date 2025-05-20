using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayerBasicStat : MonoBehaviour
{
    
    public string Email;
    public string Password;
    public string Nickname;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    
}
