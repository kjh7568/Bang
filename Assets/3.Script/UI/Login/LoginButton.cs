using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginButton : MonoBehaviour
{
    [SerializeField]private GameObject SignInPanel;
    [SerializeField]private GameObject SignUpPanel;
    
    public void OnStartButton()
    {
        SceneManager.LoadScene(1);
    }
    public void OnSignUpButton()
    {
        SignInPanel.SetActive(false);
        SignUpPanel.SetActive(true);
    }
    
    public void OnSignUp2Button()
    {
        SignInPanel.SetActive(true);
        SignUpPanel.SetActive(false);
    }
    
}
