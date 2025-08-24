using UnityEngine;

public class BattleEnemyAttackState : BattleStateBase
{
    public BattleEnemyAttackState(BattleSystem battleSystem) : base(battleSystem)
    {
    }
    
    public override void OnEnter()
    {
        Debug.Log("Enemy Attack Enter");
        // 敵攻撃開始時の処理
        // 例: 敵の攻撃アニメーション開始、AIによる攻撃選択など
        Owner.OnEnemyAttack?.Invoke();

        if (Owner.IsEnemyWin())
        {
            Owner.ChangeState(Owner.ResultState);
            return;
        }

        Owner.ChangeState(Owner.SetupState);
    }

    public override void OnUpdate()
    {
        // 敵攻撃状態での更新処理
        // 例: 攻撃アニメーションの進行状況チェック、ダメージ適用など
    }

    public override void OnExit()
    {
        Debug.Log("Enemy Attack Exit");
        // 敵攻撃終了時の処理
        // 例: 攻撃アニメーション終了、次の状態への遷移など
    }
}
