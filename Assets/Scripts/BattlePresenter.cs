using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;
using Ramen.Data;

public class BattlePresenter : IStartable, ITickable
{
    [Inject]
    private readonly BattleSystem _battleSystem;
    [Inject]
    private readonly CardComboList _cardComboList;
    [Inject]
    private readonly CardList _cardList;
    [Inject]
    private readonly EnemyList _enemyList;
    [Inject]
    private readonly SerifList _serifList;
    [Inject]
    private readonly SerifToCardList _serifToCardList;
    [Inject]
    private readonly IDeckView _deckView;
    [Inject]
    private readonly IHandView _handView;
    [Inject]
    private readonly IDiscardView _discardView;
    [Inject]
    private readonly BattleSettings _battleSettings;
    [Inject]
    private readonly IHeroView _heroView;
    [Inject]
    private readonly IEnemyView _enemyView;

    private readonly List<CardView> _deckCards = new List<CardView>();
    private readonly List<CardView> _handCards = new List<CardView>();
    private readonly List<CardView> _selectedCards = new List<CardView>();
    private readonly List<CardView> _discardCards = new List<CardView>();

    public void Start()
    {
        _battleSystem.Initialize();
        _battleSystem.OnDrawCard = OnDrawCard;
        _battleSystem.OnIsPlayerWin = IsPlayerWin;
        _battleSystem.OnIsEnemyWin = IsEnemyWin;
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
        _heroView.SetHp(_battleSettings.HeroHp);
        _enemyView.SetHp(_battleSettings.EnemyHp);

        Debug.Log("BattlePresenter Start");

        _battleSystem.ChangeState(_battleSystem.SetupState);
    }

    public void Tick()
    {

    }

    private void OnDrawCard()
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

    private void OnCardSelected(CardView card)
    {
        _selectedCards.Add(card);

        Debug.Log("OnCardSelected: " + _selectedCards.Count);

        if (_selectedCards.Count == 3)
        {
            OnPlayerAttack();
        }
    }

    private void OnPlayerAttack()
    {
        // 相手にダメージ
        _enemyView.Damage(3);

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

        _battleSystem.ChangeState(_battleSystem.PlayerAttackState);
    }

    private bool IsPlayerWin()
    {
        return _enemyView.GetHp() <= 0;
    }

    private bool IsEnemyWin()
    {
        return _heroView.GetHp() <= 0;
    }

    private void OnCardDeselected(CardView card)
    {
        _selectedCards.Remove(card);
    }
}
