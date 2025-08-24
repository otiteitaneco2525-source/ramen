using UnityEngine;
using System.Collections.Generic;
using VContainer.Unity;
using TMPro;

public interface IDeckView : IInitializable
{
    CardView CreateCard();
    CardView Draw(List<CardView> cards);
    void SetDeckCount(int count);
}

public sealed class DeckView : MonoBehaviour, IDeckView
{
    [SerializeField] CardView _cardViewPrefab;
    [SerializeField] TextMeshProUGUI _deckCountText;

    public void Initialize()
    {
    }

    public CardView Draw(List<CardView> cards)
    {
        CardView cardView = cards[0];
        cardView.gameObject.SetActive(true);
        cards.RemoveAt(0);
        return cardView;
    }

    public void SetDeckCount(int count)
    {
        _deckCountText.text = count.ToString();
    }

    public CardView CreateCard()
    {
        CardView cardView = Instantiate(_cardViewPrefab, transform);
        cardView.gameObject.SetActive(false);
        return cardView;
    }  
}
