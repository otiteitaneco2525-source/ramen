using UnityEngine;

public class BattleEnemyAttackState : BattleStateBase
{
    public BattleEnemyAttackState(BattleSystem battleSystem) : base(battleSystem)
    {
    }
    
    public override async void OnEnter()
    {
        if (Owner.OnEnemyAttack != null)
        {
            await Owner.OnEnemyAttack.Invoke();
        }

        if (Owner.IsEnemyWin())
        {
            if (Owner.OnEnemyWin != null)
            {
                await Owner.OnEnemyWin.Invoke();
            }

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
        // 敵攻撃終了時の処理
        // 例: 攻撃アニメーション終了、次の状態への遷移など
    }
}
