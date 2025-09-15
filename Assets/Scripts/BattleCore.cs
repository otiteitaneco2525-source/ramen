using UnityEngine;
using System.Collections.Generic;
using Ramen.Data;
using System;
using System.Linq;

public sealed class BattleCore
{
    private readonly List<Card> _deckCards = new List<Card>();
    private readonly List<Card> _handCards = new List<Card>();
    private readonly List<Card> _selectedCards = new List<Card>();
    private readonly List<Card> _discardCards = new List<Card>();
    public List<Card> DeckCards => _deckCards;
    public List<Card> HandCards => _handCards;
    public List<Card> SelectedCards => _selectedCards;
    public List<Card> DiscardCards => _discardCards;

    private readonly CardList _cardList;
    private readonly BattleSettings _battleSettings;
    private readonly CardComboList _cardComboList;
    private readonly SerifList _serifList;
    private readonly SerifToCardList _serifToCardList;
    private Serif _currentSerif = null;

    public Serif CurrentSerif => _currentSerif;

    public BattleCore(CardList cardList, BattleSettings battleSettings, CardComboList cardComboList, SerifList serifList, SerifToCardList serifToCardList)
    {
        _cardList = cardList;
        _cardComboList = cardComboList;
        _serifList = serifList;
        _serifToCardList = serifToCardList;
        _battleSettings = battleSettings;
    }

    public void SetCurrentSerif(Serif serif)
    {
        _currentSerif = serif;
    }

    public Serif GetRandomNormalBattleSerif()
    {
        return _serifList.GetRandomNormalBattleSerif();
    }

    public void DealCards()
    {
        _deckCards.AddRange(DealCards(_cardList));
    }

    private List<Card> DealCards(CardList cardList)
    {
        var result = new List<Card>();

        // CardTypeごとに最大2つかつランダムで2つカードデータを取得して配列に格納
        CardType[] cardTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToArray();
        foreach (var cardType in cardTypes)
        {
            var list = cardList.GetCardsByType(cardType);
            if (list == null || list.Count == 0)
            {
                continue;
            }
            
            // 最大2つまで取得し、ランダムに選択
            int count = Mathf.Min(2, list.Count);
            var shuffledCardList = list.OrderBy(x => UnityEngine.Random.value).Take(count).ToList();
            result.AddRange(shuffledCardList);
        }

        return result;
    }

    public bool IsDrawableCards()
    {
        return IsDrawableCards(_deckCards, _battleSettings.DrawCount);
    }

    private bool IsDrawableCards(List<Card> cards, int drawCount)
    {
        return cards.Count >= drawCount;
    }

    public void DrawCards()
    {
        if (!IsDrawableCards())
        {
            _deckCards.AddRange(_discardCards.ToArray());
            _discardCards.Clear();
        }

        if (_deckCards.Count == 0)
        {
            return;
        }

        _handCards.AddRange(DrawCards(_deckCards));
    }

    private List<Card> DrawCards(List<Card> cards)
    {
        var result = new List<Card>();

        // CardTypeごとに1枚ずつ合計3枚のカードを引く
        CardType[] cardTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToArray();

        foreach (var cardType in cardTypes)
        {
            // ランダムにカードを選択
            var availableCards = cards.Where(x => x.CardType == cardType).ToList();
            Card selectedCard = availableCards[UnityEngine.Random.Range(0, availableCards.Count)];
            result.Add(selectedCard);
        }

        cards.RemoveAll(x => result.Contains(x));

        return result;
    }

    public void MoveCardsToDiscard()
    {
        _discardCards.AddRange(_selectedCards);
        _selectedCards.Clear();
    }

    public void SetDiscardCards(List<Card> cards)
    {
        _discardCards.AddRange(cards);
    }

    public int GetSerifBonusPower(List<Card> selectedCards)
    {
        return GetSerifBonusPower(selectedCards, _serifToCardList.SerifToCards.Where(x => x.IsForSerifID(_currentSerif.SerifID)).ToList(), _battleSettings.SerifBonusPower, _battleSettings.DrawCount);
    }

    private int GetSerifBonusPower(List<Card> selectedCards, List<SerifToCard> serifToCards, int serifBonusPower, int drawCount)
    {
        List<bool> conditionMets = new List<bool>();

        int count = serifToCards.Count() > drawCount ? drawCount : serifToCards.Count();

        foreach (var card in selectedCards)
        {
            conditionMets.Add(serifToCards.Where(x => x.IsForCardID(card.CardID)).Count() > 0);
        }

        return conditionMets.Count(x => x) >= count ? serifBonusPower : 0;
    }

    public int GetComboBonusPower(Card cardFrom, Card cardTo)
    {
        List<CardCombo> combos = new List<CardCombo>();
        combos.AddRange(_cardComboList.CardCombos.Where(x => x.CardID_From == cardFrom.CardID && x.CardID_To == cardTo.CardID));
        combos.AddRange(_cardComboList.CardCombos.Where(x => x.CardID_From == cardTo.CardID && x.CardID_To == cardFrom.CardID));
        int bonusPower = combos.Sum(x => x.Bonus);
        return bonusPower;
    }
}