using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardSelectedState : CardStateBase
{
    public CardSelectedState(CardView owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Selected");
        // 真上に移動する
        Owner.transform.DOLocalMoveY(Owner.transform.position.y + 100, 0.1f);
        // スケールを大きく
        Owner.transform.DOScale(Vector3.one * 1.2f, 0.1f);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Owner.transform.DOLocalMoveY(Owner.transform.position.y, 0.1f);
        Owner.transform.DOScale(Vector3.one, 0.1f);
        Owner.ChangeState(Owner.WaitState);
    }
}
