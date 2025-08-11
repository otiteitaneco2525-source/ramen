using UnityEngine;

public class BattleCardSelectionState : BattleStateBase
{
    public BattleCardSelecctionState(BattleSystem battleSystem) : base(battleSystem)
    {

    }

    public override void OnEnter()
    {
        Debug.Log("CardSelection‚ÌEnter");

    }
}