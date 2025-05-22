using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnPlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject playerUIObject;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            playerUIObject.SetActive(true);
        }
    }
}
