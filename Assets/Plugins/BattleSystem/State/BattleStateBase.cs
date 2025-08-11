using UnityEngine;
//状態の基底クラス
public class BattleState//MonoBehaviourを継承しない=>Unityの性質がない
{
    //誰の状態か:BattleSystemの状態を考える
    protected BattleSystem Owner;

        //コンストラクタ：初期化処理
        public BattleStateBase(BattleSystem owner)
    {
        Owner = owner;
    }




    //その状態に入ったときに呼ばれる
    public virtual void OnEnter()
    {

    }

    //その状態の毎フレームを更新処理
    public virtual void OnUpdate()
    {
        // デフォルトでは何もしない
    }

    //その状態から出るときに呼ばれる
    public virtual void OnExit()
    {
        Debug.Log("Exiting state: " + this.GetType().Name);
    }
}
