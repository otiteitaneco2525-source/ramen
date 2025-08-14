using UnityEngine;

public class CardIdelState : CardStateBase
{
    public CardIdelState(CardView owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Idle");
    }
}
