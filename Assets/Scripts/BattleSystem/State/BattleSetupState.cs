public class BattleSetupState : BattleStateBase
{
    public BattleSetupState(BattleSystem battleSystem) : base(battleSystem)
    {
    }

    public override async void OnEnter()
    {
        if (Owner.OnSetup != null)
        {
            await Owner.OnSetup.Invoke();
        }
        Owner.ChangeState(Owner.PlayerDrawState);
    }
}
