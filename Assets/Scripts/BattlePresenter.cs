using UnityEngine;
using VContainer;
using VContainer.Unity;
using Ramen.Data;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;

public class BattlePresenter : IStartable, ITickable
{
    [Inject]
    private readonly BattleSystem _battleSystem;
    [Inject]
    private readonly CardComboList _cardComboList;
    [Inject]
    private readonly CardList _cardList;
    [Inject]
    private readonly EnemyList _enemyList;
    [Inject]
    private readonly SerifList _serifList;
    [Inject]
    private readonly SerifToCardList _serifToCardList;
    [Inject]
    private readonly IDeckView _deckView;
    [Inject]
    private readonly IHandView _handView;
    [Inject]
    private readonly IDiscardView _discardView;
    [Inject]
    private readonly BattleSettings _battleSettings;
    [Inject]
    private readonly IHeroView _heroView;
    [Inject]
    private readonly IEnemyView _enemyView;
    [Inject]
    private readonly IBattleUiView _battleUiView;
    [Inject]
    private readonly EffectView _effectView;

    private BattleCore _battleCore;
    private CompositeDisposable _disposables = new CompositeDisposable();

    public void Start()
    {
        _battleSystem.Initialize();
        _battleSystem.OnDrawCard = OnDrawCard;
        _battleSystem.OnIsPlayerWin = IsPlayerWin;
        _battleSystem.OnIsEnemyWin = IsEnemyWin;
        _deckView.Initialize();

        _handView.Initialize(_battleSettings);

        _battleCore = new BattleCore(_cardList, _battleSettings, _cardComboList, _serifList, _serifToCardList);
        _battleCore.DealCards();

        _deckView.SetDeckCount(_battleCore.DeckCards.Count);
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);
        _heroView.SetHp(_battleSettings.HeroHp);

        _enemyView.SetStatus(_enemyList.GetEnemyByID(1));

        Debug.Log("BattlePresenter Start");

        _battleUiView.OnSkipButtonClicked = OnSkipButtonClicked;

        _battleSystem.OnSetup = OnSetup;
        _battleSystem.OnPlayerAttack = OnPlayerAttack;
        _battleSystem.OnEnemyAttack = OnEnemyAttack;

        // R3でSelectedCardCountを監視し、3になったらイベントを発火
        _handView.SelectedCardCount
            .Where(count => count == 3)
            .Subscribe(_ => OnThreeCardsSelected())
            .AddTo(_disposables);

        _battleSystem.ChangeState(_battleSystem.SetupState);
    }

    public void Tick()
    {

    }

    public void OnDestroy()
    {
        _disposables?.Dispose();
    }

    private async UniTask OnDrawCard()
    {
        _battleCore.DrawCards();

        Debug.Log("OnDrawCard: " + _battleCore.HandCards.Count);

        foreach (var card in _battleCore.HandCards)
        {
            var cardView = _handView.CardViewList.FirstOrDefault(x => x.CardData == null);

            if (cardView == null)
            {
                continue;
            }

            cardView.SetCardData(card);
        }

        _deckView.SetDeckCount(_battleCore.DeckCards.Count);
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);

        await _handView.DrawCardAsync();
    }

    private void OnThreeCardsSelected()
    {
        Debug.Log("Three cards selected! Changing to PlayerAttackState");
        _battleSystem.ChangeState(_battleSystem.PlayerAttackState);
    }

    // private void OnCardSelected(CardView card)
    // {
    //     _battleCore.SelectedCards.Add(card.CardData);
    //     Debug.Log("OnCardSelected: " + _battleCore.SelectedCards.Count);
    // }

    // private void OnCardDeselected(CardView card)
    // {
    //     _battleCore.SelectedCards.Remove(card.CardData);
    // }

    private async UniTask OnPlayerAttack()
    {
        await _handView.SelectedCard();

        var selectedCards = _handView.SelectedCards.Where(x => x.Visible == true && x.CardData != null).ToList().Select(x => x.CardData).ToList();

        // 自分の手札を削除
        foreach (var cardView in _handView.SelectedCards)
        {
            cardView.SetCardData(null);
            cardView.Visible = false;
            cardView.SetIdelState();
            cardView.Reset();
        }
        _handView.SelectedCards.Clear();

        _battleCore.MoveCardsToDiscard(selectedCards);

        _deckView.SetDeckCount(_battleCore.DeckCards.Count);
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);

        var currentSerifToCards = _serifToCardList.SerifToCards.Where(x => x.SelfID == _battleCore.CurrentSerif.SerifID).ToList();

        var attackPower = selectedCards.Sum(x => x.Power);

        attackPower += _battleCore.GetSerifBonusPower(selectedCards);

        foreach (var cardFrom in selectedCards)
        {
            foreach (var cardTo in selectedCards)
            {
                attackPower += _battleCore.GetComboBonusPower(cardFrom, cardTo);
            }
        }

        if (attackPower <= 0)
        {
            attackPower = 0;
        }

        Vector3 startPos = _heroView.GetTransform().position;
        Vector3 endPos = _heroView.GetTransform().position + new Vector3(-1.5f, 0, 0);
        await LMotion.Create(startPos, endPos, 0.15f).WithEase(Ease.Linear).BindToPosition(_heroView.GetTransform());

        await UniTask.Delay(250);

        _enemyView.Damage(attackPower);

        await LMotion.Shake.Create(_enemyView.GetTransform().position, new Vector3(0.5f, 0.5f, 0f), 0.15f)
            .WithFrequency(4)
            .WithDampingRatio(0f)
            .WithRandomSeed(180)
            .BindToPosition(_enemyView.GetTransform());

        await LMotion.Create(endPos, startPos, 0.15f).WithEase(Ease.Linear).BindToPosition(_heroView.GetTransform());

        await UniTask.Delay(500);
    }

    private async UniTask OnEnemyAttack()
    {
        var startPos = _enemyView.GetTransform().position;
        var endPos = _enemyView.GetTransform().position + new Vector3(1.5f, 0, 0);
        await LMotion.Create(startPos, endPos, 0.15f).WithEase(Ease.Linear).BindToPosition(_enemyView.GetTransform());

        await UniTask.Delay(250);

        await LMotion.Create(endPos, startPos, 0.15f).WithEase(Ease.Linear).BindToPosition(_enemyView.GetTransform());

        _heroView.Damage(_enemyView.GetAttackPower());

        await LMotion.Shake.Create(_heroView.GetTransform().position, new Vector3(0.5f, 0.5f, 0f), 0.15f)
            .WithFrequency(4)
            .WithDampingRatio(0f)
            .WithRandomSeed(180)
            .BindToPosition(_heroView.GetTransform());
    }

    private bool IsPlayerWin()
    {
        return _enemyView.GetHp() <= 0;
    }

    private bool IsEnemyWin()
    {
        return _heroView.GetHp() <= 0;
    }

    private void OnSkipButtonClicked()
    {
        if (_battleSystem.CurrentState is BattleCardSelectionState)
        {
            foreach (var cardView in _handView.CardViewList)
            {
                cardView.SetIdelState();
                cardView.Reset();
            }

            _handView.SelectedCards.Clear();
            _battleCore.SelectedCards.Clear();

            _battleSystem.ChangeState(_battleSystem.EnemyAttackState);
        }
    }

    private void OnSetup()
    {
        // セリフを取得
        _battleCore.SetCurrentSerif(_serifList.GetRandomNormalBattleSerif());
        _enemyView.SetSerif(_battleCore.CurrentSerif);
    }
}
