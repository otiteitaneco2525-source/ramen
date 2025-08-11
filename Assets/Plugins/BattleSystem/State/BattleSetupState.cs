using System.Collections;
using UnityEngine;

public class BattleSystemSetUp : BattleStateBase
{
    public BattleSystemSetUp(BattleSystem battleSystem) : base(battleSystem)
    {
        // コンストラクタでBattleSystemのインスタンスを受け取る
    }
    public override void OnEnter()
    {
        Debug.Log("SetupのEnter");
        Owner.ChangeState(Owner.PlayerDrawState); // PlayerDrawStateに遷移
    }
}
