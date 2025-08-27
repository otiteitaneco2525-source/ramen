using System.Collections.Generic;
using UnityEngine;

namespace Ramen.Data
{
    /// <summary>
    /// セリフとカードの関連リストを管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "SerifToCardList", menuName = "Ramen/Serif To Card List")]
    public class SerifToCardList : ScriptableObject
    {
        [Header("セリフとカードの関連データ")]
        [SerializeField] private List<SerifToCard> serifToCards = new List<SerifToCard>();

        /// <summary>
        /// 全セリフとカードの関連リスト
        /// </summary>
        public List<SerifToCard> SerifToCards => serifToCards;

        /// <summary>
        /// セリフとカードの関連を追加
        /// </summary>
        /// <param name="serifToCard">追加する関連</param>
        public void AddSerifToCard(SerifToCard serifToCard)
        {
            if (serifToCard != null)
            {
                serifToCards.Add(serifToCard);
            }
        }

        /// <summary>
        /// 全関連をクリア
        /// </summary>
        public void ClearAllSerifToCards()
        {
            serifToCards.Clear();
        }

        /// <summary>
        /// 関連の総数を取得
        /// </summary>
        /// <returns>関連の総数</returns>
        public int GetSerifToCardCount()
        {
            return serifToCards.Count;
        }

        /// <summary>
        /// 特定のセリフIDに関連するカードを取得
        /// </summary>
        /// <param name="serifID">検索するセリフID</param>
        /// <returns>該当する関連のリスト</returns>
        public List<SerifToCard> GetCardsBySerifID(string serifID)
        {
            return serifToCards.FindAll(stc => stc.IsForSerifID(serifID));
        }

        /// <summary>
        /// 特定のカードIDに関連するセリフを取得
        /// </summary>
        /// <param name="cardID">検索するカードID</param>
        /// <returns>該当する関連のリスト</returns>
        public List<SerifToCard> GetSerifsByCardID(string cardID)
        {
            return serifToCards.FindAll(stc => stc.IsForCardID(cardID));
        }

        /// <summary>
        /// 特定のオプションを持つ関連を取得
        /// </summary>
        /// <param name="option">検索するオプション</param>
        /// <returns>該当する関連のリスト</returns>
        public List<SerifToCard> GetSerifToCardsByOption(SerifToCardType option)
        {
            return serifToCards.FindAll(stc => stc.HasOption(option));
        }

        /// <summary>
        /// OtherThanオプションを持つ関連を取得
        /// </summary>
        /// <returns>OtherThanオプションの関連リスト</returns>
        public List<SerifToCard> GetOtherThanSerifToCards()
        {
            return serifToCards.FindAll(stc => stc.IsOtherThan());
        }

        /// <summary>
        /// Noneオプションを持つ関連を取得
        /// </summary>
        /// <returns>Noneオプションの関連リスト</returns>
        public List<SerifToCard> GetNoneSerifToCards()
        {
            return serifToCards.FindAll(stc => stc.IsNone());
        }

        /// <summary>
        /// 特定のセリフIDとカードIDの組み合わせを検索
        /// </summary>
        /// <param name="serifID">セリフID</param>
        /// <param name="cardID">カードID</param>
        /// <returns>該当する関連、見つからない場合はnull</returns>
        public SerifToCard GetSerifToCard(string serifID, string cardID)
        {
            return serifToCards.Find(stc => stc.IsForSerifID(serifID) && stc.IsForCardID(cardID));
        }

        /// <summary>
        /// セリフID別の統計情報を取得
        /// </summary>
        /// <returns>セリフID別の関連数の辞書</returns>
        public Dictionary<string, int> GetSerifIDStatistics()
        {
            var stats = new Dictionary<string, int>();
            
            foreach (var stc in serifToCards)
            {
                if (stats.ContainsKey(stc.SelfID))
                {
                    stats[stc.SelfID]++;
                }
                else
                {
                    stats[stc.SelfID] = 1;
                }
            }
            
            return stats;
        }

        /// <summary>
        /// カードID別の統計情報を取得
        /// </summary>
        /// <returns>カードID別の関連数の辞書</returns>
        public Dictionary<string, int> GetCardIDStatistics()
        {
            var stats = new Dictionary<string, int>();
            
            foreach (var stc in serifToCards)
            {
                if (stats.ContainsKey(stc.CardID))
                {
                    stats[stc.CardID]++;
                }
                else
                {
                    stats[stc.CardID] = 1;
                }
            }
            
            return stats;
        }

        /// <summary>
        /// オプション別の統計情報を取得
        /// </summary>
        /// <returns>オプション別の関連数の辞書</returns>
        public Dictionary<string, int> GetOptionStatistics()
        {
            var stats = new Dictionary<string, int>();
            
            foreach (var stc in serifToCards)
            {
                if (stats.ContainsKey(stc.Option.ToString()))
                {
                    stats[stc.Option.ToString()]++;
                }
                else
                {
                    stats[stc.Option.ToString()] = 1;
                }
            }
            
            return stats;
        }

        /// <summary>
        /// 特定のセリフIDに関連するカード名のリストを取得
        /// </summary>
        /// <param name="serifID">セリフID</param>
        /// <returns>カード名のリスト</returns>
        public List<string> GetCardNamesBySerifID(string serifID)
        {
            var cards = GetCardsBySerifID(serifID);
            var cardNames = new List<string>();
            
            foreach (var card in cards)
            {
                cardNames.Add(card.CardName);
            }
            
            return cardNames;
        }

        /// <summary>
        /// 特定のセリフIDに関連するカードIDのリストを取得
        /// </summary>
        /// <param name="serifID">セリフID</param>
        /// <returns>カードIDのリスト</returns>
        public List<string> GetCardIDsBySerifID(string serifID)
        {
            var cards = GetCardsBySerifID(serifID);
            var cardIDs = new List<string>();
            
            foreach (var card in cards)
            {
                cardIDs.Add(card.CardID);
            }
            
            return cardIDs;
        }
    }
}
