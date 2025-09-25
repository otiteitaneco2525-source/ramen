using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardSelectedState : CardStateBase
{
    private float _defaultY;

    public CardSelectedState(CardView owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Selected");

        _defaultY = Owner.transform.localPosition.y;

        // 真上に移動する
        Owner.transform.DOLocalMoveY(_defaultY + 100, 0.1f);
        // スケールを大きく
        Owner.transform.DOScale(Vector3.one * 1.2f, 0.1f);

        Owner.OnCardSelected?.Invoke(Owner);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Owner.transform.DOLocalMoveY(_defaultY, 0.1f);
        Owner.transform.DOScale(Vector3.one, 0.1f);

        Owner.ChangeState(Owner.WaitState);
        Owner.OnCardDeselected?.Invoke(Owner);
    }
}
