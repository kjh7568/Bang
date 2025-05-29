using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPanel : MonoBehaviour
{
    [SerializeField]private GameObject currentPanel;
    [SerializeField]private GameObject previousPanel;

    public void OnOKButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중지
#else
        Application.Quit(); // 빌드된 실행 파일 종료
#endif
    }
    
    public void OnBackButton()
    {
        SoundManager.Instance.PlaySound(SoundType.Button);

        currentPanel.SetActive(false);
        previousPanel.SetActive(true);
    }
}
