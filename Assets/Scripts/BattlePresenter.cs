using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BattlePresenter : IStartable, ITickable
{
    [Inject]
    private readonly BattleSystem _battleSystem;
    [Inject]
    private readonly IDeckView _deckView;
    [Inject]
    private readonly IHandView _handView;
    [Inject]
    private readonly BattleSettings _battleSettings;

    public void Start()
    {
        _battleSystem.Initialize();
        _deckView.Initialize();
        Debug.Log("BattlePresenter Start");
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < _battleSettings.DrawCount; i++)
            {
                CardView cardView = _deckView.Draw();
                _handView.AddCard(cardView);
            }
        }
    }
}
