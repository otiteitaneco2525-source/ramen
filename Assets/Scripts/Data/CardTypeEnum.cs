namespace Ramen.Data
{
    /// <summary>
    /// カードタイプを定義する列挙型
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// 出汁タイプ
        /// </summary>
        Dashi = 0,
        
        /// <summary>
        /// 香味野菜タイプ
        /// </summary>
        AromaticVegetables = 1,
        
        /// <summary>
        /// 調味料タイプ
        /// </summary>
        Seasoning = 2
    }

    /// <summary>
    /// カードタイプの拡張メソッド
    /// </summary>
    public static class CardTypeExtensions
    {
        /// <summary>
        /// カードタイプの文字列表現を取得
        /// </summary>
        /// <param name="cardType">カードタイプ</param>
        /// <returns>文字列表現（A, B, C）</returns>
        public static string ToLetter(this CardType cardType)
        {
            return cardType switch
            {
                CardType.Dashi => "A",
                CardType.AromaticVegetables => "B",
                CardType.Seasoning => "C",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// カードタイプの日本語名を取得
        /// </summary>
        /// <param name="cardType">カードタイプ</param>
        /// <returns>日本語名</returns>
        public static string ToJapaneseName(this CardType cardType)
        {
            return cardType switch
            {
                CardType.Dashi => "出汁",
                CardType.AromaticVegetables => "香味野菜",
                CardType.Seasoning => "調味料",
                _ => "不明"
            };
        }

        /// <summary>
        /// 文字列からカードタイプを取得
        /// </summary>
        /// <param name="letter">文字列（A, B, C）</param>
        /// <returns>カードタイプ</returns>
        public static CardType FromLetter(string letter)
        {
            return letter?.ToUpper() switch
            {
                "A" => CardType.Dashi,
                "B" => CardType.AromaticVegetables,
                "C" => CardType.Seasoning,
                _ => CardType.Dashi // デフォルト値
            };
        }
    }
}
