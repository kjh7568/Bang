using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Enumerable = System.Linq.Enumerable;

public class CardSystem : MonoBehaviour
{
    [SerializeField]private GameObject[] cardPrefab;
    [SerializeField]private Transform deckParent;

    private List<GameObject> deckObject = new List<GameObject>();
    
    void Start()
    {
        Vector3 startPos = deckParent.position;
        int count = cardPrefab.Length;

        for (int i = 0; i < count; i++)
        {
            int randNum = Random.Range(0, cardPrefab.Length);

            if (deckObject.Contains(cardPrefab[randNum]))
            {
                i--;
                continue;
            }

            deckObject.Add(Instantiate(cardPrefab[randNum], startPos, Quaternion.Euler(-90f, 0f, 0f), deckParent));
            startPos += new Vector3(0, 0.01f, 0);
        }
    }
}
