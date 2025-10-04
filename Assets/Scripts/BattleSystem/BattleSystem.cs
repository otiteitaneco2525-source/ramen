using VContainer.Unity;
using UnityEngine.Events;
using System;
using Cysharp.Threading.Tasks;

public sealed class BattleSystem : IInitializable
{
    private BattleWaitState _waitState;
    private BattleSetupState _setupState;
    private BattlePlayerDrawState _playerDrawState;
    private BattleCardSelectionState _cardSelectionState;
    private BattlePlayerAttackState _playerAttackState;
    private BattleEnemyAttackState _enemyAttackState;
    private BattleResultState _resultState;
    private BattleStateBase _currentState;

    public BattleWaitState WaitState { get => _waitState; }
    public BattleSetupState SetupState { get => _setupState; }
    public BattlePlayerDrawState PlayerDrawState { get => _playerDrawState; }
    public BattleCardSelectionState CardSelectionState { get => _cardSelectionState; }
    public BattlePlayerAttackState PlayerAttackState { get => _playerAttackState; }
    public BattleEnemyAttackState EnemyAttackState { get => _enemyAttackState; }
    public BattleResultState ResultState { get => _resultState; }
    public BattleStateBase CurrentState { get => _currentState; }

    public Func<UniTask> OnDrawCard;
    public Func<bool> OnIsPlayerWin;
    public Func<bool> OnIsEnemyWin;
    public Func<UniTask> OnSetup;
    public Func<UniTask> OnPlayerAttack;
    public Func<UniTask> OnEnemyAttack;
    public Func<UniTask> OnPlayerWin;
    public Func<UniTask> OnEnemyWin;
    public Func<UniTask> OnResult;
    public bool IsPlayerWin()
    {
        return OnIsPlayerWin != null && OnIsPlayerWin();
    }
    public bool IsEnemyWin()
    {
        return OnIsEnemyWin != null && OnIsEnemyWin();
    }

    public void Initialize()
    {
        _waitState = new BattleWaitState(this);
        _setupState = new BattleSetupState(this);
        _playerDrawState = new BattlePlayerDrawState(this);
        _cardSelectionState = new BattleCardSelectionState(this);        
        _playerAttackState = new BattlePlayerAttackState(this);
        _enemyAttackState = new BattleEnemyAttackState(this);
        _resultState = new BattleResultState(this);

        ChangeState(_waitState);
    }

    public void ChangeState(BattleStateBase newState)
    {
        if (_currentState != null)
        {
            _currentState.OnExit();
        }
        
        _currentState = newState;
        _currentState.OnEnter();
    }
}
