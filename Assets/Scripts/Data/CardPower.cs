using System;

namespace Ramen.Data
{
    [Serializable]
    public class CardPower
    {
        /// <summary>
        /// カード属性
        /// </summary>
        public CardAttribute Attribute;

        /// <summary>
        /// パワー値
        /// </summary>
        public int Power;
    }
}