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
    public void CheckTotalCardPowerTest()
    {
        BattleSettings battleSettings = LoadAsset<BattleSettings>("Assets/ScriptableObjects/BattleSettings.asset");
        CardList cardList = LoadAsset<CardList>("Assets/ScriptableObjects/CardList.asset");
        SerifList serifList = LoadAsset<SerifList>("Assets/ScriptableObjects/SerifList.asset");

        List<string> cardNameList = new List<string> { "とり", "玉ねぎ", "醤油" };

        List<Card> selectedCardList = cardList.Cards.Where(x => cardNameList.Contains(x.Name)).ToList();

        BattleCore battleCore = new BattleCore(cardList, battleSettings);

        battleCore.SetCurrentSerif(serifList.Serifs.Where(x => x.SerifID == "5").First());

        // セリフのCardAttributeと一致するselectedCardListのPowerListのPowerの合計値を取得する
        int cardAttributePower = selectedCardList.Where(x => x.PowerList.Any(y => y.Attribute == battleCore.CurrentSerif.CardAttribute)).Sum(y => y.PowerList.Where(z => z.Attribute == battleCore.CurrentSerif.CardAttribute).Sum(z => z.Power));

        Debug.Log("cardAttributePower: " + cardAttributePower);

        // int orderPower = battleCore.GetOrderPower(selectedCardList);

        // foreach (CardPower cardPower in battleCore.CardAttributePowers)
        // {
        //     Debug.Log(cardPower.Attribute + ": " + cardPower.Power + " (" + cardPower.Count + ")");
        // }


        int plusCount = 0;
        int minusCount = 0;

        foreach (Card card in selectedCardList)
        {
            foreach (string plusCardName in card.PlusCardNameList)
            {
                if (selectedCardList.Select(y => y.Name).Contains(plusCardName))
                {
                    plusCount++;
                }
            }
            foreach (string minusCardName in card.MinusCardNameList)
            {
                if (selectedCardList.Select(y => y.Name).Contains(minusCardName))
                {
                    minusCount++;
                }
            }
        }



        Debug.Log("plusCount: " + plusCount);
        Debug.Log("minusCount: " + minusCount);

        // orderPower += plusCount * 10;
        // orderPower -= minusCount * 10;

        // Debug.Log(orderPower);


        // Assert.AreEqual(battleCore.CurrentSerif.SerifName, "肉や骨のコクがしっかり出たのが食べたい！");



        
    }


    [Test]
    public void CheckCardPowerTest()
    {
        BattleSettings battleSettings = LoadAsset<BattleSettings>("Assets/ScriptableObjects/BattleSettings.asset");
        CardList cardList = LoadAsset<CardList>("Assets/ScriptableObjects/CardList.asset");
        EnemyList enemyList = LoadAsset<EnemyList>("Assets/ScriptableObjects/EnemyList.asset");
        SerifList serifList = LoadAsset<SerifList>("Assets/ScriptableObjects/SerifList.asset");
        SerifToCardList serifToCardList = LoadAsset<SerifToCardList>("Assets/ScriptableObjects/SerifToCardList.asset");

        // CardListを最初から参照し、カード名とパワーが一致するか確認する
        foreach (var card in cardList.Cards)
        {
            Assert.IsNotNull(card.Name);

            switch (card.Name)
            {
                /* 下記のカード名ごとにパワーが一致するか確認する
                    とり
                    ぶた
                    さかな
                    カレー
                    貝
                    牛肉
                    昆布
                    ショウガ
                    ねぎ
                    にんにく
                    玉ねぎ
                    レモン
                    牛乳
                    醤油
                    塩
                    味噌
                    トマト
                    赤唐辛子

                */
                case "とり":
                    Assert.AreEqual(card.Power, 10);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 4);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 0);

                    break;
                case "ぶた":
                    Assert.AreEqual(card.Power, 8);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 3);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 1);
                    break;
                case "さかな":
                    Assert.AreEqual(card.Power, 8);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 0);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 3);
                    break;
                case "カレー":
                    Assert.AreEqual(card.Power, 8);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 3);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 0);
                    break;
                case "貝":
                    Assert.AreEqual(card.Power, 10);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 1);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 3);
                    break;
                case "牛肉":
                    Assert.AreEqual(card.Power, 12);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 15);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 3);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 3);
                    break;
                case "昆布":
                    Assert.AreEqual(card.Power, 10);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 2);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 3);
                    break;
                case "ショウガ":
                    Assert.AreEqual(card.Power, 2);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 1);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 0);
                    break;
                case "ねぎ":
                    Assert.AreEqual(card.Power, 1);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 2);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 0);
                    break;
                case "にんにく":
                    Assert.AreEqual(card.Power, 2);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 0);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 0);
                    break;
                case "玉ねぎ":
                    Assert.AreEqual(card.Power, 2);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 0);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 0);
                    break;
                case "レモン":
                    Assert.AreEqual(card.Power, 4);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 1);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 2);
                    break;
                case "牛乳":
                    Assert.AreEqual(card.Power, 12);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, -15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 15);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 1);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 16);
                    break;
                case "醤油":
                    Assert.AreEqual(card.Power, 6);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 0);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 0);
                    break;
                case "塩":
                    Assert.AreEqual(card.Power, 8);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 0);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 1);
                    break;
                case "味噌":
                    Assert.AreEqual(card.Power, 8);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 0);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 3);
                    break;
                case "トマト":
                    Assert.AreEqual(card.Power, 10);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 15);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 1);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 4);
                    break;
                case "赤唐辛子":
                    Assert.AreEqual(card.Power, 6);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Light).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rich).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Seafood).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Animal).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Stimulation).First().Power, 15);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Odor).First().Power, 0);
                    Assert.AreEqual(card.PowerList.Where(x => x.Attribute == CardAttribute.Rare).First().Power, 0);

                    Assert.AreEqual(card.PlusCardNameList.Count(), 1);
                    Assert.AreEqual(card.MinusCardNameList.Count(), 2);
                    break;
                default:
                    Assert.Fail($"カード名: {card.Name} が見つかりません");
                    break;
            }
        }
    }

    private T LoadAsset<T>(string assetPath) where T : Object
    {
        return Addressables.LoadAssetAsync<T>(assetPath).WaitForCompletion();
    }
}
