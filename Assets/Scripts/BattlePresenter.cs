using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattlePresenter : IStartable
{
    [Inject]
    private readonly BattleSystem _battleSystem;

    public void Start()
    {
        _battleSystem.Initialize();
        Debug.Log("BattlePresenter Start");
    }
}
