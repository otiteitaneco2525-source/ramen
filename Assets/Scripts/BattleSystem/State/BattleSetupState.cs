using UnityEngine;

public class BattleSetupState : BattleStateBase
{
    public BattleSetupState(BattleSystem battleSystem) : base(battleSystem)
    {
    }
    
    public override void OnEnter()
    {
        Debug.Log("Setup Enter");
        Owner.ChangeState(Owner.PlayerDrawState);
    }
}
