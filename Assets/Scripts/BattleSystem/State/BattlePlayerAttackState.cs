using UnityEngine;

public class BattlePlayerAttackState : BattleStateBase
{
    public BattlePlayerAttackState(BattleSystem battleSystem) : base(battleSystem)
    {
    }
    
    public override void OnEnter()
    {
        Debug.Log("Player Attack Enter");

        if (Owner.IsPlayerWin())
        {
            Owner.ChangeState(Owner.ResultState);
            return;
        }

        Owner.ChangeState(Owner.EnemyAttackState);
        // 攻撃開始時の処理
        // 例: 攻撃アニメーション開始、ダメージ計算など
    }

    public override void OnUpdate()
    {
        // 攻撃状態での更新処理
        // 例: 攻撃アニメーションの進行状況チェック、ダメージ適用など
    }

    public override void OnExit()
    {
        Debug.Log("Player Attack Exit");
        // 攻撃終了時の処理
        // 例: 攻撃アニメーション終了、次の状態への遷移など
    }
}
