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
        SoundManager.Instance.PlaySound(SoundType.Button);
        SignInPanel.SetActive(false);
        SignUpPanel.SetActive(true);
    }
    
    public void OnSignUp2Button()
    {
        SoundManager.Instance.PlaySound(SoundType.Button);

        SignInPanel.SetActive(true);
        SignUpPanel.SetActive(false);
    }
    
}
