using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;
using Ramen.Data;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;

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

        _battleSystem.OnSetup = OnSetup;
        _battleSystem.OnPlayerAttack = OnPlayerAttack;

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
            cardView.Visible = false;
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
            cardView.Visible = true;
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
            _battleSystem.ChangeState(_battleSystem.PlayerAttackState);
        }
    }

    private void OnCardDeselected(CardView card)
    {
        _battleCore.SelectedCards.Remove(card.CardData);
    }

    private async UniTask OnPlayerAttack()
    {
        foreach (var selectedCard in _battleCore.SelectedCards)
        {
            var cardView = _cardViewList.FirstOrDefault(x => x.CardData == selectedCard);

            if (cardView == null)
            {
                continue;
            }

            cardView.SetIdelState();
        }

        // await UniTask.Delay(500);

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

        if (attackPower <= 0)
        {
            attackPower = 0;
        }

        Vector3 startPos = _heroView.GetTransform().position;
        Vector3 endPos = _heroView.GetTransform().position + new Vector3(-1.5f, 0, 0);
        await LMotion.Create(startPos, endPos, 0.15f).WithEase(Ease.Linear).BindToPosition(_heroView.GetTransform());

        await UniTask.Delay(250);

        _enemyView.Damage(attackPower);

        await LMotion.Shake.Create(_enemyView.GetTransform().position, new Vector3(0.5f, 0.5f, 0f), 0.15f)
            .WithFrequency(4)
            .WithDampingRatio(0f)
            .WithRandomSeed(180)
            .BindToPosition(_enemyView.GetTransform());

        await LMotion.Create(endPos, startPos, 0.15f).WithEase(Ease.Linear).BindToPosition(_heroView.GetTransform());

        await UniTask.Delay(500);

        _battleSystem.ChangeState(_battleSystem.EnemyAttackState);

        startPos = _enemyView.GetTransform().position;
        endPos = _enemyView.GetTransform().position + new Vector3(1.5f, 0, 0);
        await LMotion.Create(startPos, endPos, 0.15f).WithEase(Ease.Linear).BindToPosition(_enemyView.GetTransform());

        await UniTask.Delay(250);

        await LMotion.Create(endPos, startPos, 0.15f).WithEase(Ease.Linear).BindToPosition(_enemyView.GetTransform());

        _heroView.Damage(_enemyView.GetAttackPower());

        await LMotion.Shake.Create(_heroView.GetTransform().position, new Vector3(0.5f, 0.5f, 0f), 0.15f)
            .WithFrequency(4)
            .WithDampingRatio(0f)
            .WithRandomSeed(180)
            .BindToPosition(_heroView.GetTransform());

        // Debug.Log("攻撃力: " + attackPower);

        // 自分の手札を削除
        foreach (var selectedCard in _battleCore.SelectedCards)
        {
            var cardView = _cardViewList.FirstOrDefault(x => x.CardData == selectedCard);

            if (cardView == null)
            {
                continue;
            }

            cardView.SetCardData(null);
            cardView.Visible = false;
            cardView.SetWaitState();
            _battleCore.HandCards.Remove(selectedCard);
        }

        _battleCore.MoveCardsToDiscard();

        _deckView.SetDeckCount(_battleCore.DeckCards.Count);
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);
        _handView.ArrangeCards(_cardViewList);

        _battleSystem.ChangeState(_battleSystem.SetupState);
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

    private void OnSetup()
    {
        // セリフを取得
        _battleCore.SetCurrentSerif(_serifList.GetRandomNormalBattleSerif());
        _enemyView.SetSerif(_battleCore.CurrentSerif);
    }
}
