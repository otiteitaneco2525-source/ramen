using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    [SerializeField] CardObj cardObjPrefab;
    [SerializeField] Hand hand;
    List<CardObj> cards = new List<CardObj>();
    // やること
    //1.カードを作成する（Deckに配置する）
    //2.手札に配置
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
        //スペースキーを押したときに手札にカードを配置
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 5; i++)
            {
                CardObj drawCard = Draw();
                drawCard.transform.SetParent(hand.transform,false);
                drawCard.gameObject.SetActive(true);
                hand.AddCard(drawCard);
                Debug.Log("カードを配置しました: " + drawCard.name);
            }
        }
    }
    //Deckの一番上からカードを引く
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
