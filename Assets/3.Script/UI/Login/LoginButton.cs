using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour
{
    [SerializeField]private GameObject SignInPanel;
    [SerializeField]private GameObject SignUpPanel;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_InputField nickname;
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

        email.text = "";
        password.text = "";
        nickname.text = "";
        nickname.text = "";
        
        SignInPanel.SetActive(true);
        SignUpPanel.SetActive(false);
    }
    
}
