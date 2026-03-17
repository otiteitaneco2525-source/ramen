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
    private Serif _currentSerif = null;

    public Serif CurrentSerif => _currentSerif;

    public BattleCore(CardList cardList, BattleSettings battleSettings)
    {
        _cardList = cardList;
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
    private bool IsDrawableCards()
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
}