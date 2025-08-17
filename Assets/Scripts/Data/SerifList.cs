using System.Collections.Generic;
using UnityEngine;

namespace Ramen.Data
{
    /// <summary>
    /// セリフリストを管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "SerifList", menuName = "Ramen/Serif List")]
    public class SerifList : ScriptableObject
    {
        [Header("セリフデータ")]
        [SerializeField] private List<Serif> serifs = new List<Serif>();

        /// <summary>
        /// 全セリフのリスト
        /// </summary>
        public List<Serif> Serifs => serifs;

        /// <summary>
        /// セリフを追加
        /// </summary>
        /// <param name="serif">追加するセリフ</param>
        public void AddSerif(Serif serif)
        {
            if (serif != null)
            {
                serifs.Add(serif);
            }
        }

        /// <summary>
        /// 全セリフをクリア
        /// </summary>
        public void ClearAllSerifs()
        {
            serifs.Clear();
        }

        /// <summary>
        /// セリフの総数を取得
        /// </summary>
        /// <returns>セリフの総数</returns>
        public int GetSerifCount()
        {
            return serifs.Count;
        }

        /// <summary>
        /// セリフIDでセリフを検索
        /// </summary>
        /// <param name="serifID">検索するセリフID</param>
        /// <returns>見つかったセリフ、見つからない場合はnull</returns>
        public Serif GetSerifByID(string serifID)
        {
            return serifs.Find(serif => serif.SerifID == serifID);
        }

        /// <summary>
        /// セリフ名でセリフを検索
        /// </summary>
        /// <param name="serifName">検索するセリフ名</param>
        /// <returns>該当するセリフのリスト</returns>
        public List<Serif> GetSerifsByName(string serifName)
        {
            return serifs.FindAll(serif => serif.SerifName.Contains(serifName));
        }

        /// <summary>
        /// 通常バトル用のセリフを取得（出現率が0より大きいもの）
        /// </summary>
        /// <returns>通常バトル用のセリフリスト</returns>
        public List<Serif> GetNormalBattleSerifs()
        {
            return serifs.FindAll(serif => serif.NormalBattleRate > 0);
        }

        /// <summary>
        /// ボスバトル用のセリフを取得（出現率が0より大きいもの）
        /// </summary>
        /// <returns>ボスバトル用のセリフリスト</returns>
        public List<Serif> GetBossBattleSerifs()
        {
            return serifs.FindAll(serif => serif.BossBattleRate > 0);
        }

        /// <summary>
        /// 出現率の範囲でセリフを検索
        /// </summary>
        /// <param name="minRate">最小出現率</param>
        /// <param name="maxRate">最大出現率</param>
        /// <param name="isNormalBattle">通常バトルかどうか</param>
        /// <returns>該当するセリフのリスト</returns>
        public List<Serif> GetSerifsByRateRange(int minRate, int maxRate, bool isNormalBattle)
        {
            if (isNormalBattle)
            {
                return serifs.FindAll(serif => serif.NormalBattleRate >= minRate && serif.NormalBattleRate <= maxRate);
            }
            else
            {
                return serifs.FindAll(serif => serif.BossBattleRate >= minRate && serif.BossBattleRate <= maxRate);
            }
        }

        /// <summary>
        /// 通常バトルとボスバトルで同じ出現率のセリフを取得
        /// </summary>
        /// <returns>同じ出現率のセリフリスト</returns>
        public List<Serif> GetSerifsWithSameRate()
        {
            return serifs.FindAll(serif => serif.HasSameRate());
        }

        /// <summary>
        /// 通常バトルの方が出現率が高いセリフを取得
        /// </summary>
        /// <returns>通常バトルの方が高いセリフリスト</returns>
        public List<Serif> GetSerifsHigherInNormalBattle()
        {
            return serifs.FindAll(serif => serif.IsHigherInNormalBattle());
        }

        /// <summary>
        /// ボスバトルの方が出現率が高いセリフを取得
        /// </summary>
        /// <returns>ボスバトルの方が高いセリフリスト</returns>
        public List<Serif> GetSerifsHigherInBossBattle()
        {
            return serifs.FindAll(serif => serif.IsHigherInBossBattle());
        }

        /// <summary>
        /// ランダムにセリフを選択（通常バトル用）
        /// </summary>
        /// <returns>選択されたセリフ、該当するものがない場合はnull</returns>
        public Serif GetRandomNormalBattleSerif()
        {
            var normalSerifs = GetNormalBattleSerifs();
            if (normalSerifs.Count == 0) return null;

            int totalRate = 0;
            foreach (var serif in normalSerifs)
            {
                totalRate += serif.NormalBattleRate;
            }

            int randomValue = Random.Range(1, totalRate + 1);
            int currentRate = 0;

            foreach (var serif in normalSerifs)
            {
                currentRate += serif.NormalBattleRate;
                if (randomValue <= currentRate)
                {
                    return serif;
                }
            }

            return normalSerifs[0]; // フォールバック
        }

        /// <summary>
        /// ランダムにセリフを選択（ボスバトル用）
        /// </summary>
        /// <returns>選択されたセリフ、該当するものがない場合はnull</returns>
        public Serif GetRandomBossBattleSerif()
        {
            var bossSerifs = GetBossBattleSerifs();
            if (bossSerifs.Count == 0) return null;

            int totalRate = 0;
            foreach (var serif in bossSerifs)
            {
                totalRate += serif.BossBattleRate;
            }

            int randomValue = Random.Range(1, totalRate + 1);
            int currentRate = 0;

            foreach (var serif in bossSerifs)
            {
                currentRate += serif.BossBattleRate;
                if (randomValue <= currentRate)
                {
                    return serif;
                }
            }

            return bossSerifs[0]; // フォールバック
        }
    }
}
