using UnityEngine;
using DG.Tweening;

public class CardDraggingState : CardStateBase
{
    public CardDraggingState(CardView owner) : base(owner)
    {
    }

    public override void OnUpdate()
    {
        Owner.transform.position = Input.mousePosition;
        if (Input.GetMouseButtonDown(1))
        {
            Owner.transform.DOLocalMoveY(Owner.transform.position.y, 0.1f);
            Owner.transform.DOScale(Vector3.one, 0.1f);
            Owner.ChangeState(Owner.WaitState);
        }

        // 一定以上yがおおきくなっらたらSelectedに遷移
        if (Owner.transform.localPosition.y > 100)
        {
            Owner.ChangeState(Owner.SelectedState);
        }
    }
}
