using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ramen.Data;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class RemenTest
{
    [Test]
    public void RemenScriptableObjectTest()
    {
        try
        {
            // AddressableでCardList.assetを非同期読み込み
            CardList cardList = Addressables.LoadAssetAsync<CardList>("Assets/ScriptableObjects/CardList.asset").WaitForCompletion();
            

            

            // 読み込み結果を確認
            Assert.IsNotNull(cardList, "CardList.assetの読み込みに失敗しました");
            
            // ここでCardListを使用したテストを実行
            Debug.Log($"CardList読み込み完了: {cardList.name}");

            Assert.AreEqual(14, cardList.Cards.Count);
            
            // リソースを解放
            Addressables.Release(cardList);
        }
        catch (System.Exception ex)
        {
            Assert.Fail($"CardList読み込み中にエラーが発生しました: {ex.Message}");
        }
    }
}
