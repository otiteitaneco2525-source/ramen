using UnityEngine;
using System.Collections.Generic;
using Ramen.Data;
using System;
using System.Linq;

public class BattleCore
{
    // private readonly List<ICardView> _deckCards = new List<ICardView>();
    // private readonly List<ICardView> _handCards = new List<ICardView>();
    // private readonly List<ICardView> _selectedCards = new List<ICardView>();
    // private readonly List<ICardView> _discardCards = new List<ICardView>();
    // private readonly Dictionary<CardType, List<Card>> _cardsByType = new Dictionary<CardType, List<Card>>();

    public List<Card> DealCards(CardList cardList)
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

    public bool IsDrawableCards(List<Card> cards, int drawCount)
    {
        return cards.Count >= drawCount;
    }

    public List<Card> DrawCards(List<Card> cards)
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

    /// <summary>
    /// 選択されたカードが条件に一致するかチェック
    /// </summary>
    /// <param name="selectedCards">選択されたカード</param>
    /// <param name="serifToCards">セリフに対応するカード条件</param>
    /// <returns>すべての条件に一致する場合true</returns>
    public bool CheckCardSelectionValidity(List<Card> selectedCards, List<SerifToCard> serifToCards)
    {
        // 選択されたカードのIDリスト
        var selectedCardIds = selectedCards.Select(card => card.CardID).ToList();

        List<bool> conditionMets = new List<bool>();

        // 各条件をチェック
        foreach (var serifToCard in serifToCards)
        {
            bool conditionMet = false;
            
            if (serifToCard.Option == SerifToCardType.None)
            {
                // Noneの場合：選択されたカードに指定されたカードIDが含まれているかチェック
                conditionMet = selectedCardIds.Contains(serifToCard.CardID);
            }
            else if (serifToCard.Option == SerifToCardType.OtherThan)
            {
                // OtherThanの場合：選択されたカードに指定されたカードIDが含まれていないかチェック
                conditionMet = !selectedCardIds.Contains(serifToCard.CardID);
            }

            conditionMets.Add(conditionMet);
        }

        return conditionMets.Count(x => x) == conditionMets.Count;
    }

    public int GetComboBonusPower(Card cardFrom, Card cardTo, CardComboList cardComboList)
    {
        List<CardCombo> combos = new List<CardCombo>();
        combos.AddRange(cardComboList.CardCombos.Where(x => x.CardID_From == cardFrom.CardID && x.CardID_To == cardTo.CardID));
        combos.AddRange(cardComboList.CardCombos.Where(x => x.CardID_From == cardTo.CardID && x.CardID_To == cardFrom.CardID));
        int bonusPower = combos.Sum(x => x.Bonus);
        return bonusPower;
    }
}