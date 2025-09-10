using System.Collections.Generic;
using UnityEngine;

namespace Ramen.Data
{
    /// <summary>
    /// カードコンボリストを管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "CardComboList", menuName = "Ramen/Card Combo List")]
    public class CardComboList : ScriptableObject
    {
        [Header("カードコンボデータ")]
        [SerializeField] private List<CardCombo> cardCombos = new List<CardCombo>();

        /// <summary>
        /// 全カードコンボのリスト
        /// </summary>
        public List<CardCombo> CardCombos => cardCombos;

        /// <summary>
        /// カードコンボを追加
        /// </summary>
        /// <param name="cardCombo">追加するカードコンボ</param>
        public void AddCardCombo(CardCombo cardCombo)
        {
            if (cardCombo != null)
            {
                cardCombos.Add(cardCombo);
            }
        }

        /// <summary>
        /// 全カードコンボをクリア
        /// </summary>
        public void ClearAllCardCombos()
        {
            cardCombos.Clear();
        }

        /// <summary>
        /// カードコンボの総数を取得
        /// </summary>
        /// <returns>カードコンボの総数</returns>
        public int GetCardComboCount()
        {
            return cardCombos.Count;
        }

        /// <summary>
        /// 特定のカードIDに関連するコンボを取得
        /// </summary>
        /// <param name="cardID">検索するカードID</param>
        /// <returns>該当するカードコンボのリスト</returns>
        public List<CardCombo> GetCombosByCardId(string cardID)
        {
            return cardCombos.FindAll(combo => combo.ContainsCardIdFrom(cardID));
        }

        /// <summary>
        /// 特定のカードIDに関連するコンボを取得
        /// </summary>
        /// <param name="cardID">検索するカードID</param>
        /// <returns>該当するカードコンボのリスト</returns>
        public List<CardCombo> GetCombosByCardIdFromTo(string cardIdFrom, string cardID)
        {
            return cardCombos.FindAll(combo => combo.ContainsCardIdFromTo(cardIdFrom, cardID));
        }

        /// <summary>
        /// 正のボーナスを持つコンボを取得
        /// </summary>
        /// <returns>正のボーナスを持つカードコンボのリスト</returns>
        public List<CardCombo> GetPositiveBonusCombos()
        {
            return cardCombos.FindAll(combo => combo.IsPositiveBonus());
        }

        /// <summary>
        /// 負のボーナス（ペナルティ）を持つコンボを取得
        /// </summary>
        /// <returns>負のボーナスを持つカードコンボのリスト</returns>
        public List<CardCombo> GetNegativeBonusCombos()
        {
            return cardCombos.FindAll(combo => combo.IsNegativeBonus());
        }

        /// <summary>
        /// 特定のオプションを持つコンボを取得
        /// </summary>
        /// <param name="option">検索するオプション</param>
        /// <returns>該当するカードコンボのリスト</returns>
        public List<CardCombo> GetCombosByOption(CardComboType option)
        {
            return cardCombos.FindAll(combo => combo.Option == option);
        }

        /// <summary>
        /// 2つのカードIDの組み合わせを検索
        /// </summary>
        /// <param name="cardID1">1つ目のカードID</param>
        /// <param name="cardID2">2つ目のカードID</param>
        /// <returns>該当するカードコンボ、見つからない場合はnull</returns>
        public CardCombo GetComboByCardIDs(string cardID1, string cardID2)
        {
            return cardCombos.Find(combo => 
                (combo.CardID_From == cardID1 && combo.CardID_To == cardID2) ||
                (combo.CardID_From == cardID2 && combo.CardID_To == cardID1));
        }

        /// <summary>
        /// ボーナス値の範囲でコンボを検索
        /// </summary>
        /// <param name="minBonus">最小ボーナス値</param>
        /// <param name="maxBonus">最大ボーナス値</param>
        /// <returns>該当するカードコンボのリスト</returns>
        public List<CardCombo> GetCombosByBonusRange(int minBonus, int maxBonus)
        {
            return cardCombos.FindAll(combo => combo.Bonus >= minBonus && combo.Bonus <= maxBonus);
        }
    }
}
