using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHumanCardController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private HumanList humanList;
    [SerializeField] private Player player; // 직접 Player 참조

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            Debug.LogError("Player 참조가 비어있습니다.");
            return;
        }

        string humanName = player.GameStat.InGameStat.MyHuman.Name;

        foreach (var humanData in humanList.humanList)
        {
            if (humanData.Name == humanName)
            {
                spriteRenderer.sprite = humanData.CardSprite;
                break;
            }
        }
    }
}
