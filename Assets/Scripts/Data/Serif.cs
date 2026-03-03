using System;

namespace Ramen.Data
{
    /// <summary>
    /// セリフ情報を格納するクラス
    /// </summary>
    [Serializable]
    public class Serif
    {
        /// <summary>
        /// セリフID
        /// </summary>
        public string SerifID;
        
        /// <summary>
        /// セリフ名（実際のセリフ内容）
        /// </summary>
        public string SerifName;
        
        /// <summary>
        /// 通常バトルでの出現率（%）
        /// </summary>
        public int NormalBattleRate;
        
        /// <summary>
        /// ボスバトルでの出現率（%）
        /// </summary>
        public int BossBattleRate;

        public CardAttribute CardAttribute;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public Serif()
        {
        }

        /// <summary>
        /// パラメータ付きコンストラクタ
        /// </summary>
        /// <param name="serifID">セリフID</param>
        /// <param name="serifName">セリフ名</param>
        /// <param name="normalBattleRate">通常バトル出現率</param>
        /// <param name="bossBattleRate">ボスバトル出現率</param>
        public Serif(string serifID, string serifName, int normalBattleRate, int bossBattleRate)
        {
            SerifID = serifID;
            SerifName = serifName;
            NormalBattleRate = normalBattleRate;
            BossBattleRate = bossBattleRate;
        }

        /// <summary>
        /// セリフ情報を文字列として返す
        /// </summary>
        /// <returns>セリフ情報の文字列表現</returns>
        public override string ToString()
        {
            return $"ID: {SerifID} - {SerifName} (通常: {NormalBattleRate}%, ボス: {BossBattleRate}%)";
        }

        /// <summary>
        /// 通常バトルで出現するかどうかを判定
        /// </summary>
        /// <param name="randomValue">0-100のランダム値</param>
        /// <returns>出現する場合true</returns>
        public bool ShouldAppearInNormalBattle(int randomValue)
        {
            return randomValue <= NormalBattleRate;
        }

        /// <summary>
        /// ボスバトルで出現するかどうかを判定
        /// </summary>
        /// <param name="randomValue">0-100のランダム値</param>
        /// <returns>出現する場合true</returns>
        public bool ShouldAppearInBossBattle(int randomValue)
        {
            return randomValue <= BossBattleRate;
        }

        /// <summary>
        /// 通常バトルとボスバトルの出現率が同じかどうか
        /// </summary>
        /// <returns>同じ場合true</returns>
        public bool HasSameRate()
        {
            return NormalBattleRate == BossBattleRate;
        }

        /// <summary>
        /// 通常バトルの方が出現率が高いかどうか
        /// </summary>
        /// <returns>通常バトルの方が高い場合true</returns>
        public bool IsHigherInNormalBattle()
        {
            return NormalBattleRate > BossBattleRate;
        }

        /// <summary>
        /// ボスバトルの方が出現率が高いかどうか
        /// </summary>
        /// <returns>ボスバトルの方が高い場合true</returns>
        public bool IsHigherInBossBattle()
        {
            return BossBattleRate > NormalBattleRate;
        }
    }
}
