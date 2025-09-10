using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ramen.Data;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Linq;

public class RemenTest
{
    private readonly List<Card> _deckCards = new List<Card>();
    private readonly List<Card> _handCards = new List<Card>();

    [Test]
    public void RemenScriptableObjectTest()
    {
        BattleSettings battleSettings = LoadAsset<BattleSettings>("Assets/ScriptableObjects/BattleSettings.asset");
        CardComboList cardComboList = LoadAsset<CardComboList>("Assets/ScriptableObjects/CardComboList.asset");
        CardList cardList = LoadAsset<CardList>("Assets/ScriptableObjects/CardList.asset");
        EnemyList enemyList = LoadAsset<EnemyList>("Assets/ScriptableObjects/EnemyList.asset");
        SerifList serifList = LoadAsset<SerifList>("Assets/ScriptableObjects/SerifList.asset");
        SerifToCardList serifToCardList = LoadAsset<SerifToCardList>("Assets/ScriptableObjects/SerifToCardList.asset");
        
        Assert.IsNotNull(battleSettings);
        Assert.IsNotNull(cardComboList);
        Assert.IsNotNull(cardList);
        Assert.IsNotNull(enemyList);
        Assert.IsNotNull(serifList);
        Assert.IsNotNull(serifToCardList);


        BattleCore battleCore = new BattleCore();
        _deckCards.AddRange(battleCore.DealCards(cardList));
        Assert.IsNotNull(_deckCards);
        Assert.IsNotEmpty(_deckCards);

        Assert.AreEqual(_deckCards.Count, 6);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.Dashi).Count(), 2);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 2);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.Seasoning).Count(), 2);

        // カードを引く1回目
        if (battleCore.IsDrawableCards(_deckCards, battleSettings.DrawCount))
        {
            _handCards.AddRange(battleCore.DrawCards(_deckCards));
        }

        Assert.AreEqual(_deckCards.Count, 3);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.Dashi).Count(), 1);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 1);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.Seasoning).Count(), 1);

        Assert.IsNotNull(_handCards);
        Assert.IsNotEmpty(_handCards);
        Assert.AreEqual(_handCards.Count, 3);
        Assert.AreEqual(_handCards.Where(x => x.CardType == CardType.Dashi).Count(), 1);
        Assert.AreEqual(_handCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 1);
        Assert.AreEqual(_handCards.Where(x => x.CardType == CardType.Seasoning).Count(), 1);


        // カードを引く2回目
        if (battleCore.IsDrawableCards(_deckCards, battleSettings.DrawCount))
        {
            _handCards.AddRange(battleCore.DrawCards(_deckCards));
        }
        Assert.AreEqual(_deckCards.Count, 0);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.Dashi).Count(), 0);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 0);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.Seasoning).Count(), 0);

        Assert.IsNotNull(_handCards);
        Assert.IsNotEmpty(_handCards);
        Assert.AreEqual(_handCards.Count, 6);
        Assert.AreEqual(_handCards.Where(x => x.CardType == CardType.Dashi).Count(), 2);
        Assert.AreEqual(_handCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 2);
        Assert.AreEqual(_handCards.Where(x => x.CardType == CardType.Seasoning).Count(), 2);
        

        // カードを引く3回目
        Assert.IsFalse(battleCore.IsDrawableCards(_deckCards, battleSettings.DrawCount));

        if (battleCore.IsDrawableCards(_deckCards, battleSettings.DrawCount))
        {
            _handCards.AddRange(battleCore.DrawCards(_deckCards));
        }

        Assert.AreEqual(_deckCards.Count, 0);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.Dashi).Count(), 0);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 0);
        Assert.AreEqual(_deckCards.Where(x => x.CardType == CardType.Seasoning).Count(), 0);

        // コンボの検証
        foreach (var cardFrom in cardList.Cards)
        {
            foreach (var cardTo in cardList.Cards)
            {                
                List<CardCombo> combos = new List<CardCombo>();
                combos.AddRange(cardComboList.CardCombos.Where(x => x.CardID_From == cardFrom.CardID && x.CardID_To == cardTo.CardID));
                combos.AddRange(cardComboList.CardCombos.Where(x => x.CardID_From == cardTo.CardID && x.CardID_To == cardFrom.CardID));
                var bonusPowerExpected = combos.Sum(x => x.Bonus);

                var bonusPower = battleCore.GetComboBonusPower(cardFrom, cardTo, cardComboList);
                
                Debug.Log($"コンボ: {cardFrom.Name}({cardFrom.CardID}) + {cardTo.Name}({cardTo.CardID}) = {bonusPower} | {bonusPowerExpected} | {combos.Count}");
                Assert.AreEqual(bonusPowerExpected, bonusPower);
            }
        }


        // var combos = cardComboList.GetCombosByCardId("1");
        // foreach (var cmd in cardComboList.CardCombos)
        // {
        //     int bonusPower = 0;
        //     if (cmd.Option == CardComboType.None)
        //     {
        //         var selectedCards = cardList.Cards.Where(x => x.CardID == cmd.CardID_From || x.CardID == cmd.CardID_To).ToList();
        //         bonusPower = battleCore.GetComboBonusPower(selectedCards, cardComboList);
        //     }
        //     else if (cmd.Option == CardComboType.OtherThan)
        //     {
        //         var selectedCards = cardList.Cards.Where(x => x.CardID == cmd.CardID_From || x.CardID == cmd.CardID_To).ToList();
        //         bonusPower = battleCore.GetComboBonusPower(selectedCards, cardComboList);
        //     }


        //     Debug.Log($"コンボ: {cmd.CardID_From_Name}({cmd.CardID_From}) + {cmd.CardID_To_Name}({cmd.CardID_To}) = {bonusPower} ({cmd.Option})");
        //     Assert.AreEqual(cmd.Bonus, bonusPower);
        // }


        // リソースを解放
        Addressables.Release(cardList);
    }

    private T LoadAsset<T>(string assetPath) where T : Object
    {
        return Addressables.LoadAssetAsync<T>(assetPath).WaitForCompletion();
    }
}
