public class CardSetupState : CardStateBase
{
    public CardSetupState(CardObj owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Owner.ChangeState(Owner.IdelState);
    }
}