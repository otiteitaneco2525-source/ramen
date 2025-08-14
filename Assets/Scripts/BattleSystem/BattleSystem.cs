using VContainer.Unity;

public sealed class BattleSystem : IInitializable
{
    BattleSetupState setupState;
    BattlePlayerDrawState playerDrawState;
    BattleCardSelectionState cardSelectionState;
    BattleStateBase currentState;

    public BattlePlayerDrawState PlayerDrawState { get => playerDrawState; }

    public void Initialize()
    {
        setupState = new BattleSetupState(this);
        playerDrawState = new BattlePlayerDrawState(this);
        cardSelectionState = new BattleCardSelectionState(this);
        
        ChangeState(setupState);
    }

    public void ChangeState(BattleStateBase newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        
        currentState = newState;
        currentState.OnEnter();
    }
}
