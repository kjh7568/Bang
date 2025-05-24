using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableYesButtonWhenNoMissed : MonoBehaviour
{
    [SerializeField] private GameObject yesButton;
    [SerializeField] private bool RPC_SearchMissed;
    
    private void DisableYesButton()
    {
        if (RPC_SearchMissed == false)
        {
            yesButton.SetActive(false);
        }
    }
}
