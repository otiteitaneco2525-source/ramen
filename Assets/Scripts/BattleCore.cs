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

    private List<CardPower> _cardAttributePowers;

    public List<CardPower> CardAttributePowers => _cardAttributePowers;

    public BattleCore(CardList cardList, BattleSettings battleSettings)
    {
        _cardList = cardList;
        _battleSettings = battleSettings;
        _cardAttributePowers = new List<CardPower>();
        _cardAttributePowers.Add(new CardPower(CardAttribute.Light));
        _cardAttributePowers.Add(new CardPower(CardAttribute.Rich));
        _cardAttributePowers.Add(new CardPower(CardAttribute.Seafood));
        _cardAttributePowers.Add(new CardPower(CardAttribute.Animal));
        _cardAttributePowers.Add(new CardPower(CardAttribute.Stimulation));
        _cardAttributePowers.Add(new CardPower(CardAttribute.Odor));
        _cardAttributePowers.Add(new CardPower(CardAttribute.Rare));
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
    /// カードを１枚だけデッキ（必要なら墓地をシャッフルしてデッキ化）から手札に引く
    /// </summary>
    /// <returns>引いたカード。引けない場合は null</returns>
    public Card DrawCard()
    {
        if (!IsDrawableCards(_deckCards, 1))
        {
            _deckCards.AddRange(_discardCards.ToArray());
            _discardCards.Clear();
        }

        if (_deckCards.Count == 0)
        {
            return null;
        }

        var drawn = _deckCards.OrderBy(x => UnityEngine.Random.value).First();
        _deckCards.Remove(drawn);
        _handCards.Add(drawn);
        return drawn;
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
        _handCards.RemoveAll(x => cards.Contains(x));
        _discardCards.AddRange(cards);
    }

    /// <summary>
    /// 攻撃力を計算する
    /// </summary>
    /// <param name="selectedCards">選択したカードリスト</param>
    /// <returns>攻撃力</returns>
    public int GetAttackPower(List<Card> selectedCards)
    {
        // カードの右上にある数値を合計する
        return selectedCards.Sum(x => x.Power);
    }

    /// <summary>
    /// 質問評価を計算する
    /// </summary>
    /// <param name="selectedCards">選択したカードリスト</param>
    /// <returns>質問評価値</returns>
    public int GetQuestionEvaluation(List<Card> selectedCards)
    {
        return selectedCards.Where(x => x.PowerList.Any(y => y.Attribute == _currentSerif.CardAttribute)).Sum(y => y.PowerList.Where(z => z.Attribute == _currentSerif.CardAttribute).Sum(z => z.Power));
    }

    /// <summary>
    /// 相性評価を計算する
    /// </summary>
    /// <param name="selectedCards">選択したカードリスト</param>
    /// <returns>相性評価値</returns>
    public int GetCompatibilityEvaluation(List<Card> selectedCards)
    {
        int power = 0;
        int plusCount = 0;
        int minusCount = 0;

        foreach (Card card in selectedCards)
        {
            foreach (string plusCardName in card.PlusCardNameList)
            {
                if (selectedCards.Select(y => y.Name).Contains(plusCardName))
                {
                    plusCount++;
                }
            }
            foreach (string minusCardName in card.MinusCardNameList)
            {
                if (selectedCards.Select(y => y.Name).Contains(minusCardName))
                {
                    minusCount++;
                }
            }
        }
        power += plusCount * 10;
        power -= minusCount * 10;
        return power;
    }
}