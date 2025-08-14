using UnityEngine;

public class CardIdelState : CardStateBase
{
    public CardIdelState(CardObj owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Idle");
    }
}
