using UnityEngine;

public class BattleWaitState : BattleStateBase
{
    public BattleWaitState(BattleSystem battleSystem) : base(battleSystem)
    {
    }
    
    public override void OnEnter()
    {
        Debug.Log("Wait Enter");
    }

    public override void OnExit()
    {
        Debug.Log("Wait Exit");
    }
}
