using UnityEngine;
//状態の基底クラス
public class BattleStateBase//MonoBehaviourを使わない=>Unityの制約を受けない
{
    //状態の所有者:BattleSystemの状態を管理する
    protected BattleSystem Owner;

        //コンストラクタ:所有者を設定
        public BattleStateBase(BattleSystem owner)
    {
        Owner = owner;
    }




    //この状態に入ったときに呼ばれる
    public virtual void OnEnter()
    {

    }

    //この状態の毎フレーム更新処理
    public virtual void OnUpdate()
    {
        // デフォルトでは何もしない
    }

    //この状態から出るときに呼ばれる
    public virtual void OnExit()
    {
        Debug.Log("Exiting state: " + this.GetType().Name);
    }
}
