public class CardSetupState : CardStateBase
{
    public CardSetupState(CardView owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Owner.ChangeState(Owner.IdelState);
    }
}