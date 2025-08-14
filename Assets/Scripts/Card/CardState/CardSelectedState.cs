using UnityEngine;
using DG.Tweening;

public class CardSelectedState : CardStateBase
{
    public CardSelectedState(CardObj owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Selected");
        // 中央にDOTweenで移動
        Owner.transform.DOLocalMove(Vector3.zero, 0.1f);
        // スケールを大きく
        Owner.transform.DOScale(Vector3.one * 1.2f, 0.1f);
    }

    public override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // 解除する
            Owner.ResetPos();
            Owner.ChangeState(Owner.WaitState);
        }
    }
}
