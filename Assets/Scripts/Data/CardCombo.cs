using System;

namespace Ramen.Data
{
    /// <summary>
    /// カードの組み合わせ情報を格納するクラス
    /// </summary>
    [Serializable]
    public class CardCombo
    {
        /// <summary>
        /// 組み合わせ元のカードID
        /// </summary>
        public string CardID_From;
        
        /// <summary>
        /// 組み合わせ元のカード名
        /// </summary>
        public string CardID_From_Name;
        
        /// <summary>
        /// 組み合わせ先のカードID
        /// </summary>
        public string CardID_To;
        
        /// <summary>
        /// 組み合わせ先のカード名
        /// </summary>
        public string CardID_To_Name;
        
        /// <summary>
        /// ボーナス値（正の値はボーナス、負の値はペナルティ）
        /// </summary>
        public int Bonus;
        
        /// <summary>
        /// オプション（None, OtherThanなど）
        /// </summary>
        public CardComboType Option;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public CardCombo()
        {
        }

        /// <summary>
        /// パラメータ付きコンストラクタ
        /// </summary>
        /// <param name="cardID_From">組み合わせ元のカードID</param>
        /// <param name="cardID_From_Name">組み合わせ元のカード名</param>
        /// <param name="cardID_To">組み合わせ先のカードID</param>
        /// <param name="cardID_To_Name">組み合わせ先のカード名</param>
        /// <param name="bonus">ボーナス値</param>
        /// <param name="option">オプション</param>
        public CardCombo(string cardID_From, string cardID_From_Name, string cardID_To, string cardID_To_Name, int bonus, string option)
        {
            CardID_From = cardID_From;
            CardID_From_Name = cardID_From_Name;
            CardID_To = cardID_To;
            CardID_To_Name = cardID_To_Name;
            Bonus = bonus;
            Option = (CardComboType)Enum.Parse(typeof(CardComboType), option);
        }

        /// <summary>
        /// 組み合わせ情報を文字列として返す
        /// </summary>
        /// <returns>組み合わせ情報の文字列表現</returns>
        public override string ToString()
        {
            string bonusText = Bonus >= 0 ? $"+{Bonus}" : Bonus.ToString();
            return $"{CardID_From_Name}({CardID_From}) + {CardID_To_Name}({CardID_To}) = {bonusText} ({Option})";
        }

        /// <summary>
        /// ボーナスが正の値かどうか
        /// </summary>
        /// <returns>ボーナスが正の値の場合true</returns>
        public bool IsPositiveBonus()
        {
            return Bonus > 0;
        }

        /// <summary>
        /// ボーナスが負の値かどうか
        /// </summary>
        /// <returns>ボーナスが負の値の場合true</returns>
        public bool IsNegativeBonus()
        {
            return Bonus < 0;
        }

        /// <summary>
        /// 特定のカードIDとの組み合わせかどうか
        /// </summary>
        /// <param name="cardID">チェックするカードID</param>
        /// <returns>組み合わせに含まれる場合true</returns>
        public bool ContainsCardIdFrom(string cardID)
        {
            return CardID_From == cardID;
        }

        /// <summary>
        /// 特定のカードIDとの組み合わせかどうか
        /// </summary>
        /// <param name="cardIdFrom">チェックするカードID</param>
        /// <returns>組み合わせに含まれる場合true</returns>
        public bool ContainsCardIdFromTo(string cardIdFrom, string cardIdTo)
        {
            return CardID_From == cardIdFrom && CardID_To == cardIdTo;
        }
    }
}
