using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    // ステート（戦闘の状態）の管理
    //1.初期化:setup:Deckの準備
    //2.Playerのドローステート:Deckからカードを引く準備
    //3.Playerのカード選択ステート
    //4.Playerのカード実行ステート
    //5.Enemyのステート

    //ステートパターンで管理する
    //利点:各ステートの処理がクラスに分かれているので、編集しやすい

    //どうやって状態を管理するか？
    //・状態の基底クラスを作る
    //・それぞれのクラスを作る
    //・状態の切り替えを行う関数を作る

    BattleSetupState setupState;
    BattlePlayerDrawState playerDrawState;
    BattleCardSelectionState cardSelectionState;

    BattleStateBase currentState;

    public BattlePlayerDrawState PlayerDrawState { get => playerDrawState; }

    void Start()
    {
        // 各状態のインスタンスを作成
        setupState = new BattleSetupState(this);
        playerDrawState = new BattlePlayerDrawState(this);
        cardSelectionState = new BattleCardSelectionState(this);
        
        ChangeState(setupState); // 初期状態をセットアップ状態に設定
    }

    // 状態の切り替え
    public void ChangeState(BattleStateBase newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(); // 現在の状態のExitメソッドを呼び出す
        }
        
        currentState = newState; // 新しい状態に切り替える
        currentState.OnEnter(); // 新しい状態のEnterメソッドを呼び出す
    }
}
