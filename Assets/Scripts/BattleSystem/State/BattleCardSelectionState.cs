using UnityEngine;

public class BattleCardSelectionState : BattleStateBase
{
    public BattleCardSelectionState(BattleSystem battleSystem) : base(battleSystem)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("CardSelection Enter");
    }
}
