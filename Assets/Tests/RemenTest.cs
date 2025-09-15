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


        BattleCore battleCore = new BattleCore(cardList, battleSettings, cardComboList, serifList, serifToCardList);
        battleCore.DealCards();
        Assert.IsNotNull(battleCore.DeckCards);
        Assert.IsNotEmpty(battleCore.DeckCards);

        Assert.AreEqual(battleCore.DeckCards.Count, 6);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.Dashi).Count(), 2);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 2);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.Seasoning).Count(), 2);

        // カードを引く1回目
        if (battleCore.IsDrawableCards())
        {
            battleCore.DrawCards();
        }

        Assert.AreEqual(battleCore.DeckCards.Count, 3);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.Dashi).Count(), 1);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 1);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.Seasoning).Count(), 1);

        Assert.IsNotNull(battleCore.HandCards);
        Assert.IsNotEmpty(battleCore.HandCards);
        Assert.AreEqual(battleCore.HandCards.Count, 3);
        Assert.AreEqual(battleCore.HandCards.Where(x => x.CardType == CardType.Dashi).Count(), 1);
        Assert.AreEqual(battleCore.HandCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 1);
        Assert.AreEqual(battleCore.HandCards.Where(x => x.CardType == CardType.Seasoning).Count(), 1);


        // カードを引く2回目
        if (battleCore.IsDrawableCards())
        {
            battleCore.DrawCards();
        }
        Assert.AreEqual(battleCore.DeckCards.Count, 0);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.Dashi).Count(), 0);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 0);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.Seasoning).Count(), 0);

        Assert.IsNotNull(battleCore.HandCards);
        Assert.IsNotEmpty(battleCore.HandCards);
        Assert.AreEqual(battleCore.HandCards.Count, 6);
        Assert.AreEqual(battleCore.HandCards.Where(x => x.CardType == CardType.Dashi).Count(), 2);
        Assert.AreEqual(battleCore.HandCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 2);
        Assert.AreEqual(battleCore.HandCards.Where(x => x.CardType == CardType.Seasoning).Count(), 2);
        

        // カードを引く3回目
        Assert.IsFalse(battleCore.IsDrawableCards());

        if (battleCore.IsDrawableCards())
        {
            battleCore.DrawCards();
        }

        Assert.AreEqual(battleCore.DeckCards.Count, 0);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.Dashi).Count(), 0);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.AromaticVegetables).Count(), 0);
        Assert.AreEqual(battleCore.DeckCards.Where(x => x.CardType == CardType.Seasoning).Count(), 0);

        // コンボの検証
        foreach (var cardFrom in cardList.Cards)
        {
            foreach (var cardTo in cardList.Cards)
            {                
                List<CardCombo> combos = new List<CardCombo>();
                combos.AddRange(cardComboList.CardCombos.Where(x => x.CardID_From == cardFrom.CardID && x.CardID_To == cardTo.CardID));
                combos.AddRange(cardComboList.CardCombos.Where(x => x.CardID_From == cardTo.CardID && x.CardID_To == cardFrom.CardID));
                var bonusPowerExpected = combos.Sum(x => x.Bonus);

                var bonusPower = battleCore.GetComboBonusPower(cardFrom, cardTo);
                
                Debug.Log($"コンボ: {cardFrom.Name}({cardFrom.CardID}) + {cardTo.Name}({cardTo.CardID}) = {bonusPower} | {bonusPowerExpected} | {combos.Count}");
                Assert.AreEqual(bonusPowerExpected, bonusPower);
            }
        }

        battleCore.SetCurrentSerif(serifList.GetSerifByID("1"));
        Assert.AreEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "12", "1", "3" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("2"));
        Assert.AreEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "2", "4", "6" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("3"));
        Assert.AreEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "18", "4" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("4"));
        Assert.AreEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "1", "3", "5" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("5"));
        Assert.AreEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "3", "5", "7" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("6"));
        Assert.AreEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "6", "2", "1" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("7"));
        Assert.AreEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "13", "17", "6" }, cardList)));

        battleCore.SetCurrentSerif(serifList.GetSerifByID("1"));
        Assert.AreNotSame(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "1", "2", "3" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("2"));
        Assert.AreNotSame(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "1", "2", "3" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("3"));
        Assert.AreNotSame(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "1", "2", "3" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("4"));
        Assert.AreNotSame(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "1", "2", "3" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("5"));
        Assert.AreNotEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "1", "2", "3" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("6"));
        Assert.AreNotEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "1", "2", "3" }, cardList)));
        battleCore.SetCurrentSerif(serifList.GetSerifByID("7"));
        Assert.AreNotEqual(battleSettings.SerifBonusPower, battleCore.GetSerifBonusPower(GetCardList(new string[] { "1", "2", "3" }, cardList)));
        
        // リソースを解放
        Addressables.Release(cardList);
    }


    private List<Card> GetCardList(string[] cardIDs, CardList cardList)
    {
        return cardList.Cards.Where(x => cardIDs.Contains(x.CardID)).ToList();
    }

    private T LoadAsset<T>(string assetPath) where T : Object
    {
        return Addressables.LoadAssetAsync<T>(assetPath).WaitForCompletion();
    }
}
