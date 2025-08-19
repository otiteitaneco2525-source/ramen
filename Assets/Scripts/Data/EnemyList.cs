using System.Collections.Generic;
using UnityEngine;

namespace Ramen.Data
{
    /// <summary>
    /// 敵リストを管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyList", menuName = "Ramen/Enemy List")]
    public class EnemyList : ScriptableObject
    {
        [Header("敵データ")]
        [SerializeField] private List<Enemy> enemies = new List<Enemy>();

        /// <summary>
        /// 全敵のリスト
        /// </summary>
        public List<Enemy> Enemies => enemies;

        /// <summary>
        /// 敵を追加
        /// </summary>
        /// <param name="enemy">追加する敵</param>
        public void AddEnemy(Enemy enemy)
        {
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
        }

        /// <summary>
        /// 全敵をクリア
        /// </summary>
        public void ClearAllEnemies()
        {
            enemies.Clear();
        }

        /// <summary>
        /// 敵の総数を取得
        /// </summary>
        /// <returns>敵の総数</returns>
        public int GetEnemyCount()
        {
            return enemies.Count;
        }

        /// <summary>
        /// 敵IDで敵を検索
        /// </summary>
        /// <param name="enemyID">検索する敵ID</param>
        /// <returns>見つかった敵、見つからない場合はnull</returns>
        public Enemy GetEnemyByID(string enemyID)
        {
            return enemies.Find(enemy => enemy.EnemyID == enemyID);
        }
    }
}


