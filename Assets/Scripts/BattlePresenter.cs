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

    private readonly List<CardView> _deckCards = new List<CardView>();
    private readonly List<CardView> _handCards = new List<CardView>();
    private readonly List<CardView> _selectedCards = new List<CardView>();
    private readonly List<CardView> _discardCards = new List<CardView>();

    // CardTypeごとのカードデータ配列
    private readonly Dictionary<CardType, List<Card>> _cardsByType = new Dictionary<CardType, List<Card>>();

    private Serif _serif;

    public void Start()
    {
        _battleSystem.Initialize();
        _battleSystem.OnDrawCard = OnDrawCard;
        _battleSystem.OnIsPlayerWin = IsPlayerWin;
        _battleSystem.OnIsEnemyWin = IsEnemyWin;
        _deckView.Initialize();

        // CardTypeごとに最大2つかつランダムで2つカードデータを取得して配列に格納
        CardType[] cardTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToArray();
        foreach (var cardType in cardTypes)
        {
            var cardList = _cardList.GetCardsByType(cardType);
            if (cardList != null && cardList.Count > 0)
            {
                // 最大2つまで取得し、ランダムに選択
                int count = Mathf.Min(2, cardList.Count);
                var shuffledCardList = cardList.OrderBy(x => UnityEngine.Random.value).Take(count).ToList();
                _cardsByType[cardType] = shuffledCardList;
                
                Debug.Log($"{cardType}タイプのカードを{count}枚ランダムに取得しました");
            }
            else
            {
                _cardsByType[cardType] = new List<Card>();
                Debug.LogWarning($"{cardType}タイプのカードが見つかりませんでした");
            }
        }

        for (int i = 0; i < 6; i++)
        {
            _deckCards.Add(_deckView.CreateCard());
        }

        foreach (var card in _deckCards)
        {
            card.OnCardSelected = OnCardSelected;
            card.OnCardDeselected = OnCardDeselected;
        }

        _deckView.SetDeckCount(_deckCards.Count);
        _discardView.SetDiscardCount(_discardCards.Count);
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

        // CardTypeごとに1枚ずつ合計3枚のカードを引く
        CardType[] cardTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToArray();
        foreach (var cardType in cardTypes)
        {
            // デッキにカードがない場合は処理を終了
            if (_deckCards.Count == 0) break;

            // _cardsByTypeから選択されたカードタイプのカードを取得
            if (_cardsByType.ContainsKey(cardType) && _cardsByType[cardType].Count > 0)
            {
                // ランダムにカードを選択
                var availableCards = _cardsByType[cardType];
                Card selectedCard = availableCards[UnityEngine.Random.Range(0, availableCards.Count)];
                
                // デッキからカードビューを取得
                CardView cardView = _deckView.Draw(_deckCards);
                
                // カードデータを設定
                cardView.SetCardData(selectedCard);
                
                // 手札に追加
                _handCards.Add(cardView);
                _handView.AddCard(cardView);
                cardView.SetInHand();
                cardView.SetWaitState();
                
                Debug.Log($"カードを引きました: {selectedCard.Name} ({cardType})");
            }
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




        // 自分の手札を削除
        var selectedCardsToProcess = _selectedCards.ToArray();
        foreach (var selectedCard in selectedCardsToProcess)
        {
            _handCards.Remove(selectedCard);
            _handView.RemoveCard(selectedCard);
            _discardCards.Add(selectedCard);
            selectedCard.SetInDiscard();
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

    private void OnSkipButtonClicked()
    {
        if (_battleSystem.CurrentState is BattleCardSelectionState)
        {
            foreach (var card in _selectedCards)
            {
                card.SetWaitState();
            }

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
        _serif = _serifList.GetRandomNormalBattleSerif();
        _enemyView.SetSerif(_serif);
    }
}
