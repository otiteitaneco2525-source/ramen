
public class BattlePlayerAttackState : BattleStateBase
{
    public BattlePlayerAttackState(BattleSystem battleSystem) : base(battleSystem)
    {
    }
    
    public override async void OnEnter()
    {
        if (Owner.IsPlayerWin())
        {
            Owner.ChangeState(Owner.ResultState);
            return;
        }

        if (Owner.OnPlayerAttack != null)
        {
            await Owner.OnPlayerAttack.Invoke();
        }
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
    }
}
