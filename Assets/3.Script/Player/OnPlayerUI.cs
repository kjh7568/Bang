using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnPlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject playerUIObject;

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            playerUIObject.SetActive(true);
        }
        else
        {
            playerUIObject.SetActive(false);
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
