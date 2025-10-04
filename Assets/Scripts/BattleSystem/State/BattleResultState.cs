using UnityEngine;

public class BattleResultState : BattleStateBase
{
    public BattleResultState(BattleSystem battleSystem) : base(battleSystem)
    {
    }
    
    public override async void OnEnter()
    {
        // 戦闘結果処理開始時の処理
        // 例: 勝利/敗北判定、経験値計算、報酬表示など
        if (Owner.OnResult != null)
        {
            await Owner.OnResult.Invoke();
        }
    }

    public override void OnUpdate()
    {
        // 戦闘結果状態での更新処理
        // 例: 結果画面の表示、ユーザー入力待ちなど
    }

    public override void OnExit()
    {
        // 戦闘結果処理終了時の処理
        // 例: 結果画面終了、次のシーンへの遷移など
    }
}
