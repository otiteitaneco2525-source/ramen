public class BattlePlayerAttackState : BattleStateBase
{
    public BattlePlayerAttackState(BattleSystem battleSystem) : base(battleSystem)
    {
    }
    
    public override async void OnEnter()
    {
        if (Owner.OnPlayerAttack != null)
        {
            await Owner.OnPlayerAttack.Invoke();
        }

        if (Owner.IsPlayerWin())
        {
            if (Owner.OnPlayerWin != null)
            {
                await Owner.OnPlayerWin.Invoke();
            }
            return;
        }

        Owner.ChangeState(Owner.EnemyAttackState);
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
    }
}
