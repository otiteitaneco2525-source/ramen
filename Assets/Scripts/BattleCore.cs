using System.Collections.Generic;
using Ramen.Data;
using System.Linq;

public sealed class BattleCore
{
    private readonly List<Card> _deckCards = new List<Card>();
    private readonly List<Card> _handCards = new List<Card>();
    private readonly List<Card> _discardCards = new List<Card>();
    public List<Card> DeckCards => _deckCards;
    public List<Card> HandCards => _handCards;
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

    /// <summary>
    /// シリフを設定する
    /// </summary>
    /// <param name="serif">シリフ</param>
    public void SetCurrentSerif(Serif serif)
    {
        _currentSerif = serif;
    }

    /// <summary>
    /// デフォルトのカードを設定する
    /// </summary>
    /// <param name="cardIdList">カードIDリスト</param>
    public void DealDefaultCard(List<string> cardIdList)
    {
        cardIdList.ForEach(x => {
            _deckCards.Add(_cardList.GetCardByID(x));
        });
    }

    /// <summary>
    /// 引けるカードがあるかどうかを判定する
    /// </summary>
    /// <returns>引けるカードがあるかどうか</returns>
    public bool IsDrawableCards()
    {
        return IsDrawableCards(_deckCards, _battleSettings.DrawCount);
    }

    /// <summary>
    /// 引けるカードがあるかどうかを判定する
    /// </summary>
    /// <param name="cards">カードリスト</param>
    /// <param name="drawCount">引くカードの枚数</param>
    /// <returns>引けるカードがあるかどうか</returns>
    private bool IsDrawableCards(List<Card> cards, int drawCount)
    {
        return cards.Count >= drawCount;
    }

    /// <summary>
    /// カードを引く
    /// </summary>
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

    /// <summary>
    /// カードを引く
    /// </summary>
    /// <param name="cards">カードリスト</param>
    /// <returns>引いたカードリスト</returns>
    private List<Card> DrawCards(List<Card> cards)
    {
        var result = new List<Card>();

        // ランダムに合計DrawCount枚のカードを引く
        var shuffledCards = cards.OrderBy(x => UnityEngine.Random.value).Take(_battleSettings.DrawCount).ToList();
        result.AddRange(shuffledCards);

        cards.RemoveAll(x => result.Contains(x));

        return result;
    }

    /// <summary>
    /// カードを墓地に移動する
    /// </summary>
    /// <param name="cards">カードリスト</param>
    public void MoveCardsToDiscard(List<Card> cards)
    {
        _deckCards.RemoveAll(x => cards.Contains(x));
        _discardCards.AddRange(cards);
    }

    /// <summary>
    /// セリフのボーナスパワーを取得する
    /// </summary>
    /// <param name="selectedCards">選択したカードリスト</param>
    /// <returns>セリフのボーナスパワー</returns>
    public int GetSerifBonusPower(List<Card> selectedCards)
    {
        return GetSerifBonusPower(selectedCards, _serifToCardList.SerifToCards.Where(x => x.IsForSerifID(_currentSerif.SerifID)).ToList(), _battleSettings.SerifBonusPower, _battleSettings.DrawCount);
    }

    /// <summary>
    /// セリフのボーナスパワーを取得する
    /// </summary>
    /// <param name="selectedCards">選択したカードリスト</param>
    /// <param name="serifToCards">シリフとカードの関係リスト</param>
    /// <param name="serifBonusPower">セリフのボーナスパワー</param>
    /// <param name="drawCount">引くカードの枚数</param>
    /// <returns>セリフのボーナスパワー</returns>
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

    /// <summary>
    /// コンボのボーナスパワーを取得する
    /// </summary>
    /// <param name="cardFrom">カード1</param>
    /// <param name="cardTo">カード2</param>
    /// <returns>コンボのボーナスパワー</returns>
    public int GetComboBonusPower(Card cardFrom, Card cardTo)
    {
        List<CardCombo> combos = new List<CardCombo>();
        combos.AddRange(_cardComboList.CardCombos.Where(x => x.CardID_From == cardFrom.CardID && x.CardID_To == cardTo.CardID));
        combos.AddRange(_cardComboList.CardCombos.Where(x => x.CardID_From == cardTo.CardID && x.CardID_To == cardFrom.CardID));
        int bonusPower = combos.Sum(x => x.Bonus);
        return bonusPower;
    }
}