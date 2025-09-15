using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;
using Ramen.Data;
using System.Linq;
using System;

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
    [Inject]
    private readonly IBattleUiView _battleUiView;

    private readonly List<CardView> _cardViewList = new List<CardView>();

    private BattleCore _battleCore;

    public void Start()
    {
        _battleSystem.Initialize();
        _battleSystem.OnDrawCard = OnDrawCard;
        _battleSystem.OnIsPlayerWin = IsPlayerWin;
        _battleSystem.OnIsEnemyWin = IsEnemyWin;
        _deckView.Initialize();

        _battleCore = new BattleCore(_cardList, _battleSettings, _cardComboList, _serifList, _serifToCardList);
        _battleCore.DealCards();

        for (int i = 0; i < _battleCore.DeckCards.Count; i++)
        {
            _cardViewList.Add(_deckView.CreateCard(_handView.GetTransform()));
        }

        foreach (var cardView in _cardViewList)
        {
            cardView.OnCardSelected = OnCardSelected;
            cardView.OnCardDeselected = OnCardDeselected;
        }

        _deckView.SetDeckCount(_battleCore.DeckCards.Count);
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);
        _heroView.SetHp(_battleSettings.HeroHp);

        _enemyView.SetStatus(_enemyList.GetEnemyByID(1));

        Debug.Log("BattlePresenter Start");

        _battleUiView.OnSkipButtonClicked = OnSkipButtonClicked;

        _battleSystem.OnEnemyAttack = OnEnemyAttack;
        _battleSystem.OnSetup = OnSetup;

        _battleSystem.ChangeState(_battleSystem.SetupState);
    }

    public void Tick()
    {

    }

    private void OnDrawCard()
    {
        _battleCore.DrawCards();

        Debug.Log("OnDrawCard: " + _battleCore.HandCards.Count);

        foreach (var cardView in _cardViewList)
        {
            cardView.SetCardData(null);
            cardView.SetInDiscard();
            cardView.SetWaitState();
        }

        foreach (var card in _battleCore.HandCards)
        {
            var cardView = _cardViewList.FirstOrDefault(x => x.CardData == null);

            if (cardView == null)
            {
                continue;
            }

            cardView.SetCardData(card);
            cardView.SetInHand();
            cardView.SetWaitState();
        }

        _handView.ArrangeCards(_cardViewList);
        _deckView.SetDeckCount(_battleCore.DeckCards.Count);
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);
    }

    private void OnCardSelected(CardView card)
    {
        _battleCore.SelectedCards.Add(card.CardData);

        Debug.Log("OnCardSelected: " + _battleCore.SelectedCards.Count);

        if (_battleCore.SelectedCards.Count == 3)
        {
            OnPlayerAttack();
        }
    }

    private void OnCardDeselected(CardView card)
    {
        _battleCore.SelectedCards.Remove(card.CardData);
    }

    private void OnPlayerAttack()
    {
        var currentSerifToCards = _serifToCardList.SerifToCards.Where(x => x.SelfID == _battleCore.CurrentSerif.SerifID).ToList();

        var attackPower = _battleCore.SelectedCards.Sum(x => x.Power);

        attackPower += _battleCore.GetSerifBonusPower(_battleCore.SelectedCards);

        foreach (var cardFrom in _battleCore.SelectedCards)
        {
            foreach (var cardTo in _battleCore.SelectedCards)
            {
                attackPower += _battleCore.GetComboBonusPower(cardFrom, cardTo);
            }
        }

        _enemyView.Damage(attackPower);

        Debug.Log("攻撃力: " + attackPower);

        // 自分の手札を削除
        foreach (var selectedCard in _battleCore.SelectedCards)
        {
            var cardView = _cardViewList.FirstOrDefault(x => x.CardData == selectedCard);

            if (cardView == null)
            {
                continue;
            }

            cardView.SetCardData(null);
            cardView.SetInDiscard();
            cardView.SetWaitState();
            _battleCore.HandCards.Remove(selectedCard);
        }

        _battleCore.MoveCardsToDiscard();

        _deckView.SetDeckCount(_battleCore.DeckCards.Count);
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);
        _handView.ArrangeCards(_cardViewList);

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

    private void OnSkipButtonClicked()
    {
        if (_battleSystem.CurrentState is BattleCardSelectionState)
        {
            foreach (var cardView in _cardViewList)
            {
                cardView.SetWaitState();
            }

            _battleCore.SelectedCards.Clear();

            _battleSystem.ChangeState(_battleSystem.EnemyAttackState);
        }
    }

    private void OnEnemyAttack()
    {
        _heroView.Damage(_enemyView.GetAttackPower());
    }

    private void OnSetup()
    {
        // セリフを取得
        _battleCore.SetCurrentSerif(_serifList.GetRandomNormalBattleSerif());
        _enemyView.SetSerif(_battleCore.CurrentSerif);
    }
}
