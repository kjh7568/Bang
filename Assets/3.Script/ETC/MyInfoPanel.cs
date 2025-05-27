using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyInfoPanel : MonoBehaviour
{
    [Header("SO setting")]
    [SerializeField] private HumanList HumanDatas;
    [SerializeField] private JobList JobDatas;
    
    [Header("moving panel setting")]
    public Button openPanelButton;
    public RectTransform MyInfoPanelTransform;
    public float moveSpeed = 5f;

    private Vector2 startPosition;
    private Vector2 hiddenPosition;
    private Vector2 targetPosition;
    private bool shouldMove = false;
    private bool isOpen = false;

    [Header("panel context setting")]
    public TMP_Text nameTxt;
    
    public TMP_Text jobTxt;
    public TMP_Text jobInfoTxt;
    public Image jobSprite;

    public TMP_Text humanTxt;
    public TMP_Text humanInfoTxt;
    public Image humanSprite;
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        
        openPanelButton.onClick.AddListener(OpenMyPanel);

        startPosition = MyInfoPanelTransform.anchoredPosition;
        hiddenPosition = startPosition + new Vector2(-800f, 0); 

        targetPosition = hiddenPosition;

        UpdateMyInfo();
    }

    void Update()
    {
        if (shouldMove)
        {
            MyInfoPanelTransform.anchoredPosition = Vector2.Lerp(
                MyInfoPanelTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * moveSpeed
            );

            if (Vector2.Distance(MyInfoPanelTransform.anchoredPosition, targetPosition) < 0.1f)
            {
                MyInfoPanelTransform.anchoredPosition = targetPosition;
                shouldMove = false;
            }
        }
    }

    void OpenMyPanel()
    {
        if (isOpen)
        {
            targetPosition = startPosition; 
        }
        else
        {
            targetPosition = hiddenPosition; 
        }

        isOpen = !isOpen;
        shouldMove = true;
    }

    void UpdateMyInfo()
    {
        Debug.Log($"선택된 직업: {Player.LocalPlayer.InGameStat.MyJob}");
        Debug.Log($"선택된 인물: {Player.LocalPlayer.InGameStat.MyHuman}");
        
        var job = Player.LocalPlayer.InGameStat.MyJob;
        var human = Player.LocalPlayer.InGameStat.MyHuman;

        nameTxt.text = Player.LocalPlayer.BasicStat.nickName;
 
        jobTxt.text = Player.LocalPlayer.InGameStat.MyJob.Name;
        jobSprite.sprite = JobDatas.GetJobSpriteByName(job.Name);
        jobInfoTxt.text = Player.LocalPlayer.InGameStat.MyJob.Description;
        
        humanTxt.text = Player.LocalPlayer.InGameStat.MyHuman.Name;
        humanSprite.sprite = HumanDatas.GetHumanSpriteByName(human.Name);
        humanInfoTxt.text = Player.LocalPlayer.InGameStat.MyHuman.Description;
    }
}