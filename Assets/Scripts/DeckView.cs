using UnityEngine;
using System.Collections.Generic;

public interface IDeckView
{
    CardView AddCard();
    CardView Draw();
}

public sealed class DeckView : MonoBehaviour, IDeckView
{
    [SerializeField] CardView _cardViewPrefab;

    // [Inject]
    // private readonly HandView _handView;
    private List<CardView> _cards = new List<CardView>();
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            CardView cardView = AddCard();
            cardView.gameObject.SetActive(false);
            _cards.Add(cardView);
        }
    }
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         for (int i = 0; i < 5; i++)
    //         {
    //             CardView drawCard = Draw();
    //             drawCard.transform.SetParent(_handView.transform,false);
    //             drawCard.gameObject.SetActive(true);
    //             _handView.AddCard(drawCard);
    //             Debug.Log("Draw: " + drawCard.name);
    //         }
    //     }
    // }

    public CardView Draw()
    {
        CardView cardObj = _cards[0];
        cardObj.gameObject.SetActive(true);
        _cards.RemoveAt(0);
        return cardObj;
    }

    public CardView AddCard()
    {
        return Instantiate(_cardViewPrefab, transform);
    }  
}
