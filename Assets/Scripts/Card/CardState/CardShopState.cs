using UnityEngine.EventSystems;

public class CardShopState : CardStateBase
{
    public CardShopState(CardView owner) : base(owner)
    {
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Owner.OnCardBuy?.Invoke();
    }
}