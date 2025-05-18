using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField]private TMP_Text nickNameText;
    
    [SerializeField]private TMP_Text jobNameText;
    [SerializeField]private TMP_Text jobInfoText;
    [SerializeField]private Image jobImage;
    
    [SerializeField]private TMP_Text humanNameText;
    [SerializeField]private TMP_Text humanInfoText;
    [SerializeField]private Image humanImage;
    
    //일단은 호스트만 바뀌게 만듬
    public void Init(Player player)
    {
        nickNameText.text = player.BasicStat.nickName;

        jobNameText.text = player.GameStat.InGameStat.MyJob.Name;
        jobInfoText.text = player.GameStat.InGameStat.MyJob.Description;
        jobImage.sprite = player.GameStat.InGameStat.MyJob.CardSprite;

        humanNameText.text = player.GameStat.InGameStat.MyHuman.Name;
        humanInfoText.text = player.GameStat.InGameStat.MyHuman.Description;
        humanImage.sprite = player.GameStat.InGameStat.MyHuman.CardSprite;
    }
}
