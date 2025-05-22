using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public GameObject cardListPanel;
    public GameObject waitingPanel;
    public GameObject playerPanel;
    
    public TMP_Text waitingUserTurnText;

    
    public List<GameObject> enemyList = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        SetPlayerPanel();
    }

    public void SetPlayerPanel()
    {
        int index = 0;

        foreach (var playerPair in BasicSpawner.Instance.spawnedPlayers)
        {
            var player = playerPair.Value.GetComponent<Player>();

            // 본인을 제외하고
            if (player.Object.InputAuthority != BasicSpawner.Instance._runner.LocalPlayer)
            {
                if (index < enemyList.Count)
                {
                    GameObject enemySlot = enemyList[index];

                    // 이름 설정
                    TMP_Text nameText = enemySlot.GetComponentInChildren<TMP_Text>();
                    if (nameText != null)
                        nameText.text = player.BasicStat.nickName;

                    // 버튼 설정
                    Button button = enemySlot.GetComponent<Button>();
                    if (button != null)
                    {
                        Debug.Log(player.BasicStat.nickName);
                        
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => OnEnemySlotClicked(player.BasicStat.nickName));
                    }

                    index++;
                }
            }
        }

        // // 남은 슬롯은 비워주기
        // for (int i = index; i < enemyList.Count; i++)
        // {
        //     TMP_Text nameText = enemyList[i].GetComponentInChildren<TMP_Text>();
        //     if (nameText != null)
        //         nameText.text = "";
        //
        //     Button button = enemyList[i].GetComponent<Button>();
        //     if (button != null)
        //         button.onClick.RemoveAllListeners();
        // }
    }

    public void OnEnemySlotClicked(string playerName)
    {
        Debug.Log("클릭된 플레이어 이름: " + playerName);
    }

    
    // public GameObject[] characterPrefabs; // 프리팹 4개
    // //public Transform spawnParent;         // CharacterSelectionPanel
    // private List<GameObject> spawned = new List<GameObject>();
    //
    // void Start() {
    //     float spacing = 5f;
    //     for (int i = 0; i < characterPrefabs.Length; i++) {
    //         GameObject character = Instantiate(characterPrefabs[i], playerPanel.transform);
    //         character.transform.localScale = new Vector3(150, 150, 150);
    //         character.transform.localPosition = new Vector3(i * spacing, 0, 0);
    //         character.transform.localRotation = Quaternion.Euler(0, 180, 0); 
    //         spawned.Add(character);
    //     }
    // }
    //
    // void Update() {
    //     if (Input.GetMouseButtonDown(0)) {
    //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //         if (Physics.Raycast(ray, out RaycastHit hit)) {
    //             GameObject selected = hit.collider.gameObject;
    //             Debug.Log("선택한 캐릭터: " + selected.name);
    //             // 선택 처리 로직 추가
    //         }
    //     }
    // }
}
