using UnityEngine;

public class BattlePlayerDrawState : BattleStateBase
{
    public BattlePlayerDrawState(BattleSystem battleSystem) : base(battleSystem)
    {

    }

    public override async void OnEnter()
    {
        Debug.Log("PlayerDraw Enter");

        if (Owner.OnDrawCard != null)
        {
            await Owner.OnDrawCard.Invoke();
        }
        
        Owner.ChangeState(Owner.CardSelectionState);
    }
}
