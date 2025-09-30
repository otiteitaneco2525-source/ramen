using UnityEngine;

public class BattleSetupState : BattleStateBase
{
    public BattleSetupState(BattleSystem battleSystem) : base(battleSystem)
    {
    }

    public override async void OnEnter()
    {
        Debug.Log("Setup Enter");

        if (Owner.OnSetup != null)
        {
            await Owner.OnSetup.Invoke();
        }
        Owner.ChangeState(Owner.PlayerDrawState);
    }
}
