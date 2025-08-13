using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardWaitState : CardStateBase
{
    public CardWaitState(CardObj owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Wait");
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Owner.transform.DOScale(Vector3.one * 1.2f, 0.1f);
        Owner.defaultSiblingIndex = Owner.transform.GetSiblingIndex();
        // àÍî‘è„Ç…ï\é¶Ç∑ÇÈ
        Owner.transform.SetAsLastSibling();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Owner.transform.SetSiblingIndex(Owner.defaultSiblingIndex);
        Owner.transform.DOScale(Vector3.one, 0.1f);
    }
}