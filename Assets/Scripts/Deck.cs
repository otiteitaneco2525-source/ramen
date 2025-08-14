using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    [SerializeField] CardObj cardObjPrefab;
    [SerializeField] Hand hand;
    List<CardObj> cards = new List<CardObj>();
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            CardObj cardObj = Spawn();
            cardObj.gameObject.SetActive(false);
            cards.Add(cardObj);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 5; i++)
            {
                CardObj drawCard = Draw();
                drawCard.transform.SetParent(hand.transform,false);
                drawCard.gameObject.SetActive(true);
                hand.AddCard(drawCard);
                Debug.Log("Draw: " + drawCard.name);
            }
        }
    }

    private CardObj Draw()
    {
        CardObj cardObj = cards[0];
        cards.RemoveAt(0);
        return cardObj;
    }
    CardObj Spawn()
    {
        return Instantiate(cardObjPrefab, transform);
    }  
}
