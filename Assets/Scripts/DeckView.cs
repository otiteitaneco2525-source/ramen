using UnityEngine;
using System.Collections.Generic;
using VContainer.Unity;
using TMPro;

public interface IDeckView : IInitializable
{
    CardView AddCard();
    CardView Draw();
}

public sealed class DeckView : MonoBehaviour, IDeckView
{
    [SerializeField] CardView _cardViewPrefab;
    [SerializeField] TextMeshProUGUI _deckCountText;
    private List<CardView> _cards = new List<CardView>();

    public void Initialize()
    {
        for (int i = 0; i < 10; i++)
        {
            CardView cardView = AddCard();
            cardView.gameObject.SetActive(false);
            _cards.Add(cardView);
        }
    }

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
