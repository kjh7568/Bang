using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class InGameSystem : MonoBehaviour
{
    public static InGameSystem Instance;

    private const int MAX_EVENT_PROCESS_COUNT = 10;
    private const int MAX_BLOOD_COUNT = 30;

    [SerializeField] private Transform bloodPrefabParent;
    private WaitForSeconds bloodWait = new WaitForSeconds(2f);
    
    public class Callbacks
    {
        //CombatEvent가 발생하면의 의미로 쓸거임
        public Action<InGameEvent> OnCombatEvent;
    }

    public readonly Callbacks Events = new Callbacks();

    private Dictionary<PlayerRef, IDamageAble> playerDic = new Dictionary<PlayerRef, IDamageAble>();
    private Queue<InGameEvent> inGameEventQueue = new Queue<InGameEvent>();
    private Queue<GameObject> bloodPrefabQueue = new Queue<GameObject>();
    [SerializeField] private GameObject bloodPrefab;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < MAX_BLOOD_COUNT; i++)
        {
            var blood = Instantiate(bloodPrefab, bloodPrefabParent);
            bloodPrefabQueue.Enqueue(blood);
            blood.SetActive(false);
        }
    }

    private void Update()
    {
        int processCount = 0;
        while (inGameEventQueue.Count > 0 && processCount < MAX_EVENT_PROCESS_COUNT)
        {
            var inGameEvent = inGameEventQueue.Dequeue();

            switch (inGameEvent.Type)
            {
                case InGameEvent.EventType.Combat:
                    var combatEvent = inGameEvent as CombatEvent;
                    inGameEvent.Receiver.TakeDamage(combatEvent);
                    StartCoroutine(SpawnBloodEffect(combatEvent.HitPosition));
                    Events.OnCombatEvent?.Invoke(combatEvent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void RegisterMonster(IDamageAble monster)
    {
        if (playerDic.TryAdd(monster.MainCollider, monster) == false)
        {
            Debug.LogWarning($"{monster.GameObject.name}가 등록되어 있습니다." +
                             $"{playerDic[monster.MainCollider]}를 대체합니다");
            playerDic[monster.MainCollider] = monster;
        }
    }

    public IDamageAble GetPlayerOrNull(PlayerRef player)
    {
        if (playerDic.ContainsKey(player))
        {
            return playerDic[player];
        }

        return null;
    }

    public void AddInGameEvent(InGameEvent inGameEvent)
    {
        inGameEventQueue.Enqueue(inGameEvent);
    }

    private IEnumerator SpawnBloodEffect(Vector3 hitPosition)
    {
        var blood = bloodPrefabQueue.Dequeue();
        blood.transform.position = hitPosition;
        blood.SetActive(true);
        
        yield return bloodWait;
        
        blood.SetActive(false);
        bloodPrefabQueue.Enqueue(blood);
    }
}