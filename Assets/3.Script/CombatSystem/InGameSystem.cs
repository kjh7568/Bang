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

    private Queue<InGameEvent> inGameEventQueue = new Queue<InGameEvent>();
    private Queue<GameObject> bloodPrefabQueue = new Queue<GameObject>();
    [SerializeField] private GameObject bloodPrefab;

    private void Awake()
    {
        Instance = this;

        // for (int i = 0; i < MAX_BLOOD_COUNT; i++)
        // {
        //     var blood = Instantiate(bloodPrefab, bloodPrefabParent);
        //     bloodPrefabQueue.Enqueue(blood);
        //     blood.SetActive(false);
        // }
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
                    // StartCoroutine(SpawnBloodEffect(combatEvent.HitPosition));
                    Events.OnCombatEvent?.Invoke(combatEvent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public IDamageAble GetPlayerOrNull(PlayerRef player)
    {
        var playerDic = BasicSpawner.Instance.spawnedPlayers;

        if (playerDic.ContainsKey(player))
        {
            var damageAble = playerDic[player].GetComponent<Player>().GameStat.InGameStat;
            return damageAble;
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