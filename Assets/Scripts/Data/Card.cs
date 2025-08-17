using System;

namespace Ramen.Data
{
    /// <summary>
    /// カードの基本情報を格納するクラス
    /// </summary>
    [Serializable]
    public class Card
    {
        /// <summary>
        /// カードID（例：1 A, 2 A, 8 B, 13 C）
        /// </summary>
        public string CardID;
        
        /// <summary>
        /// カードタイプ（A, B, C）
        /// </summary>
        public string CardType;
        
        /// <summary>
        /// カードタイプ名（出汁、香味野菜、調味料など）
        /// </summary>
        public string CardTypeName;
        
        /// <summary>
        /// カード名（とり、ぶた、ショウガ、牛乳、醤油など）
        /// </summary>
        public string Name;
        
        /// <summary>
        /// パワー値（1-12の範囲）
        /// </summary>
        public int Power;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public Card()
        {
        }

        /// <summary>
        /// パラメータ付きコンストラクタ
        /// </summary>
        /// <param name="cardID">カードID</param>
        /// <param name="cardType">カードタイプ</param>
        /// <param name="cardTypeName">カードタイプ名</param>
        /// <param name="name">カード名</param>
        /// <param name="power">パワー値</param>
        public Card(string cardID, string cardType, string cardTypeName, string name, int power)
        {
            CardID = cardID;
            CardType = cardType;
            CardTypeName = cardTypeName;
            Name = name;
            Power = power;
        }

        /// <summary>
        /// カード情報を文字列として返す
        /// </summary>
        /// <returns>カード情報の文字列表現</returns>
        public override string ToString()
        {
            return $"CardID: {CardID}, Type: {CardType} ({CardTypeName}), Name: {Name}, Power: {Power}";
        }
    }
}
