using System.Collections;
using UnityEngine;

public class BattleSetupState : BattleStateBase
{
    public BattleSetupState(BattleSystem battleSystem) : base(battleSystem)
    {
        // コンストラクタでBattleSystemのインスタンスを受け取る
    }
    public override void OnEnter()
    {
        Debug.Log("SetupにEnter");
        Owner.ChangeState(Owner.PlayerDrawState); // PlayerDrawStateに遷移
    }
}
