using System.Collections.Generic;
using UnityEngine;

namespace Ramen.Data
{
    /// <summary>
    /// カードリストを管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "CardList", menuName = "Ramen/Card List")]
    public class CardList : ScriptableObject
    {
        [Header("カードデータ")]
        [SerializeField] private List<Card> cards = new List<Card>();

        /// <summary>
        /// 全カードのリスト
        /// </summary>
        public List<Card> Cards => cards;

        /// <summary>
        /// カードを追加
        /// </summary>
        /// <param name="card">追加するカード</param>
        public void AddCard(Card card)
        {
            if (card != null)
            {
                cards.Add(card);
            }
        }

        /// <summary>
        /// 全カードをクリア
        /// </summary>
        public void ClearAllCards()
        {
            cards.Clear();
        }

        /// <summary>
        /// カードの総数を取得
        /// </summary>
        /// <returns>カードの総数</returns>
        public int GetCardCount()
        {
            return cards.Count;
        }

        /// <summary>
        /// カードIDでカードを検索
        /// </summary>
        /// <param name="cardID">検索するカードID</param>
        /// <returns>見つかったカード、見つからない場合はnull</returns>
        public Card GetCardByID(string cardID)
        {
            return cards.Find(card => card.CardID == cardID);
        }

        /// <summary>
        /// カードタイプでカードを検索
        /// </summary>
        /// <param name="cardType">検索するカードタイプ（A, B, C）</param>
        /// <returns>該当するカードのリスト</returns>
        public List<Card> GetCardsByType(string cardType)
        {
            return cards.FindAll(card => card.CardType == cardType);
        }
    }
}