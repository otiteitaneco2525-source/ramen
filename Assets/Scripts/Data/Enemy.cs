using System;

namespace Ramen.Data
{
    /// <summary>
    /// 敵の基本情報を格納するクラス
    /// </summary>
    [Serializable]
    public class Enemy
    {
        /// <summary>
        /// 敵ID（例：1, 2, 3 ...）
        /// </summary>
        public int EnemyID;

        /// <summary>
        /// 敵名（例：スライム、ケモノ、魔女、ボス）
        /// </summary>
        public string EnemyName;

        /// <summary>
        /// 体力（HP）
        /// </summary>
        public int HP;

        /// <summary>
        /// 攻撃力
        /// </summary>
        public int AttackPower;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public Enemy()
        {
        }

        /// <summary>
        /// パラメータ付きコンストラクタ
        /// </summary>
        /// <param name="enemyID">敵ID</param>
        /// <param name="enemyName">敵名</param>
        /// <param name="hp">体力</param>
        /// <param name="attackPower">攻撃力</param>
        public Enemy(int enemyID, string enemyName, int hp, int attackPower)
        {
            EnemyID = enemyID;
            EnemyName = enemyName;
            HP = hp;
            AttackPower = attackPower;
        }

        /// <summary>
        /// 情報の文字列表現
        /// </summary>
        public override string ToString()
        {
            return $"ID: {EnemyID}, Name: {EnemyName}, HP: {HP}, ATK: {AttackPower}";
        }

        /// <summary>
        /// 指定ダメージを受けた後のHPを返す（負値にしない）
        /// </summary>
        /// <param name="damage">ダメージ量</param>
        /// <returns>ダメージ適用後のHP</returns>
        public int CalculateHpAfterDamage(int damage)
        {
            int nextHp = HP - Math.Max(0, damage);
            return nextHp < 0 ? 0 : nextHp;
        }

        /// <summary>
        /// 現在のHPが0以下かどうか
        /// </summary>
        public bool IsDead()
        {
            return HP <= 0;
        }
    }
}


