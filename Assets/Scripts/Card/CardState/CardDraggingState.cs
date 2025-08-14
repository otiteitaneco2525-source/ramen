using UnityEngine;

public class CardDraggingState : CardStateBase
{
    public CardDraggingState(CardObj owner) : base(owner)
    {
    }

    // この状態の間はマウスに追従する

    public override void OnUpdate()
    {
        Owner.transform.position = Input.mousePosition;
        if (Input.GetMouseButtonDown(1))
        {
            // 解除する
            Owner.ResetPos();
            Owner.ChangeState(Owner.WaitState);
        }

        // 一定以上yがおおきくなっらたらSelectedに遷移
        if (Owner.transform.localPosition.y > 100)
        {
            Owner.ChangeState(Owner.SelectedState);
        }
    }
}
