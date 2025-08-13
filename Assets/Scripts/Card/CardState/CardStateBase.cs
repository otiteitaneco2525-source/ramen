using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.EventSystems;

public class CardStateBase
{
    protected CardObj Owner;

    public CardStateBase(CardObj owner)
    {
        Owner = owner;
    }

    // その状態に入った時に呼ばれる
    public virtual void OnEnter()
    {
    }

    // その状態の毎フレーム更新処理
    public virtual void OnUpdate()
    {
    }
    // その状態から出る時に呼ばれる
    public virtual void OnExit()
    {
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
    }
}
