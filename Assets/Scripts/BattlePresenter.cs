using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;

public class BattlePresenter : IStartable, ITickable
{
    [Inject]
    private readonly BattleSystem _battleSystem;
    [Inject]
    private readonly IDeckView _deckView;
    [Inject]
    private readonly IHandView _handView;
    [Inject]
    private readonly IDiscardView _discardView;
    [Inject]
    private readonly BattleSettings _battleSettings;

    private readonly List<CardView> _deckCards = new List<CardView>();
    private readonly List<CardView> _handCards = new List<CardView>();
    private readonly List<CardView> _selectedCards = new List<CardView>();
    private readonly List<CardView> _discardCards = new List<CardView>();

    public void Start()
    {
        _battleSystem.Initialize();
        _deckView.Initialize();

        for (int i = 0; i < 10; i++)
        {
            _deckCards.Add(_deckView.AddCard());
        }

        foreach (var card in _deckCards)
        {
            card.OnCardSelected += OnCardSelected;
            card.OnCardDeselected += OnCardDeselected;
        }

        _deckView.SetDeckCount(_deckCards.Count);
        _discardView.SetDiscardCount(_discardCards.Count);

        Debug.Log("BattlePresenter Start");
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_deckCards.Count < _battleSettings.DrawCount)
            {
                var cardsToMove = _discardCards.ToArray();
                foreach (var card in cardsToMove)
                {
                    _deckCards.Add(card);
                    _discardCards.Remove(card);
                    card.SetWaitState();
                }
                _discardView.SetDiscardCount(_discardCards.Count);
            }

            for (int i = 0; i < _battleSettings.DrawCount; i++)
            {
                CardView cardView = _deckView.Draw(_deckCards);
                _handCards.Add(cardView);
                _handView.AddCard(cardView);
                cardView.SetWaitState();
            }
            _handView.ArrangeCards(_handCards);
            _deckView.SetDeckCount(_deckCards.Count);
        }
    }

    private void OnCardSelected(CardView card)
    {
        _selectedCards.Add(card);

        Debug.Log("OnCardSelected: " + _selectedCards.Count);

        if (_selectedCards.Count == 3)
        {
            // 相手にダメージ

            // 自分の手札を削除
            var selectedCardsToProcess = _selectedCards.ToArray();
            foreach (var selectedCard in selectedCardsToProcess)
            {
                _handCards.Remove(selectedCard);
                _handView.RemoveCard(selectedCard);
                _discardCards.Add(selectedCard);
            }
            _selectedCards.Clear();
            _discardView.SetDiscardCount(_discardCards.Count);
            _handView.ArrangeCards(_handCards);
        }
    }

    private void OnCardDeselected(CardView card)
    {
        _selectedCards.Remove(card);
    }
}
