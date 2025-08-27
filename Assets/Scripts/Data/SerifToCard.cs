using System;

namespace Ramen.Data
{
    /// <summary>
    /// セリフとカードの関連情報を格納するクラス
    /// </summary>
    [Serializable]
    public class SerifToCard
    {
        /// <summary>
        /// セリフID
        /// </summary>
        public string SelfID;
        
        /// <summary>
        /// カードID
        /// </summary>
        public string CardID;
        
        /// <summary>
        /// カード名
        /// </summary>
        public string CardName;
        
        /// <summary>
        /// オプション（None, OtherThanなど）
        /// </summary>
        public SerifToCardType Option;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public SerifToCard()
        {
        }

        /// <summary>
        /// パラメータ付きコンストラクタ
        /// </summary>
        /// <param name="selfID">セリフID</param>
        /// <param name="cardID">カードID</param>
        /// <param name="cardName">カード名</param>
        /// <param name="option">オプション</param>
        public SerifToCard(string selfID, string cardID, string cardName, SerifToCardType option)
        {
            SelfID = selfID;
            CardID = cardID;
            CardName = cardName;
            Option = option;
        }

        /// <summary>
        /// セリフとカードの関連情報を文字列として返す
        /// </summary>
        /// <returns>関連情報の文字列表現</returns>
        public override string ToString()
        {
            return $"セリフID: {SelfID} → カード: {CardName}({CardID}) [{Option}]";
        }

        /// <summary>
        /// 特定のセリフIDと一致するかどうか
        /// </summary>
        /// <param name="serifID">チェックするセリフID</param>
        /// <returns>一致する場合true</returns>
        public bool IsForSerifID(string serifID)
        {
            return SelfID == serifID;
        }

        /// <summary>
        /// 特定のカードIDと一致するかどうか
        /// </summary>
        /// <param name="cardID">チェックするカードID</param>
        /// <returns>一致する場合true</returns>
        public bool IsForCardID(string cardID)
        {
            return CardID == cardID;
        }

        /// <summary>
        /// 特定のオプションと一致するかどうか
        /// </summary>
        /// <param name="option">チェックするオプション</param>
        /// <returns>一致する場合true</returns>
        public bool HasOption(SerifToCardType option)
        {
            return Option == option;
        }

        /// <summary>
        /// OtherThanオプションかどうか
        /// </summary>
        /// <returns>OtherThanの場合true</returns>
        public bool IsOtherThan()
        {
            return Option == SerifToCardType.OtherThan;
        }

        /// <summary>
        /// Noneオプションかどうか
        /// </summary>
        /// <returns>Noneの場合true</returns>
        public bool IsNone()
        {
            return Option == SerifToCardType.None;
        }
    }
}
