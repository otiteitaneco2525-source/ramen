using System.Collections;
using System.Collections.Generic;
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
        // ’†‰›‚ÉDOTween‚ÅˆÚ“®
        Owner.transform.DOLocalMove(Vector3.zero, 0.1f);
        // ƒXƒP[ƒ‹‚ğ‘å‚«‚­
        Owner.transform.DOScale(Vector3.one * 1.2f, 0.1f);
        BezierArrows.Instance.Show();
        BezierArrows.Instance.SetColor(Color.gray);
    }

    public override void OnUpdate()
    {
        BezierArrows.Instance.SetOriginPos(Owner.transform.position);
        BezierArrows.Instance.SetTopPos(Input.mousePosition);

        if (Input.GetMouseButtonDown(1))
        {
            // ‰ğœ‚·‚é
            Owner.ResetPos();
            Owner.ChangeState(Owner.WaitState);
        }
    }
}
