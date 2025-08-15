using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;

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

    private readonly List<CardView> _deckCards = new List<CardView>();
    private readonly List<CardView> _handCards = new List<CardView>();

    public void Start()
    {
        _battleSystem.Initialize();
        _deckView.Initialize();

        for (int i = 0; i < 10; i++)
        {
            _deckCards.Add(_deckView.AddCard());
        }
        _deckView.SetDeckCount(_deckCards.Count);

        Debug.Log("BattlePresenter Start");
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < _battleSettings.DrawCount; i++)
            {
                CardView cardView = _deckView.Draw(_deckCards);
                _handCards.Add(cardView);
                _handView.AddCard(cardView);
            }
            _handView.ArrangeCards(_handCards);
            _deckView.SetDeckCount(_deckCards.Count);
        }
    }
}
