public abstract class BattleStateBase
{
    protected BattleSystem Owner;

    public BattleStateBase(BattleSystem owner)
    {
        Owner = owner;
    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnExit()
    {

    }
}
