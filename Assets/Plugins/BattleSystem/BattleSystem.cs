using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    // フェーズ（バトルの状態）の管理
    //1.準備：setup:Deckを作る
    //2.Playerのドローフェーズ:Deckからカードを５枚引く
    //3.Playerのカード選択のフェーズ
    //4.Playerのカード効果のフェーズ
    //5.Enemyのフェーズ

    //これらをステートパターンで管理する
    //利点：各フェーズの処理がクラスに分かれているので、編集がしやすい

    //どうやって実装するの？
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
        // 各状態のインスタンスを生成
        setupState = new BattleSetupState(this);
        playerDrawState = new BattlePlayerDrawState(this);
        cardSelectionState = new BattleCardSelectionState(this);
        
        ChangeState(setupState); // 初期状態をセットアップ状態に設定


        currentState = setupState; // 初期状態をセットアップ状態に設定
        currentState.OnEnter(); // 初期状態のEnterメソッドを呼び出す
    }

    // 状態の切り替え
    public void ChangeState(BattleStateBase newState)
    {
        
        currentState = newState; // 新しい状態に切り替える
        currentState.OnEnter(); // 新しい状態のEnterメソッドを呼び出す
    }
}
