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

        public int Count;

        public int PlusPower;

        public int MinusPower;


        public CardPower(CardAttribute cardAttribute)
        {
            Attribute = cardAttribute;
            Power = 0;
            Count = 0;
        }  
    }
}