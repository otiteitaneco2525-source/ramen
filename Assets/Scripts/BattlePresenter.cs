using UnityEngine;
using VContainer;
using VContainer.Unity;
using Ramen.Data;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UniRx;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public class BattlePresenter : IStartable, IDisposable
{
    [Inject]
    private readonly BattleSystem _battleSystem;
    [Inject]
    private readonly CardComboList _cardComboList;
    [Inject]
    private readonly CardList _cardList;
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
    private readonly IBattleUiView _battleUiView;
    [Inject]
    private readonly EffectView _effectView;
    [Inject]
    private readonly FadeView _fadeView;
    [Inject]
    private readonly GameEntity _gameEntity;
    [Inject]
    private readonly EnemyList _enemyList;
    [Inject]
    private readonly SoundManager _soundManager;
    [Inject]
    private readonly EnemyRoot _enemyRoot;

    private BattleCore _battleCore;
    private CompositeDisposable _disposables = new CompositeDisposable();
    private EnemyView _enemyView;
    private Enemy _enemy;

    public async void Start()
    {
        _battleSettings.SetDefaultCardId(_gameEntity.CardIdList);
        if (_gameEntity.Hp == 0)
        {
            _gameEntity.Hp = _battleSettings.HeroHp;
            _gameEntity.MaxHp = _battleSettings.HeroHp;            
        }

        Debug.Log("BattlePresenter Start: " + _gameEntity.ToString());

        var enemyPrefab = Addressables.LoadAssetAsync<GameObject>($"Assets/Prefabs/EnemyView_{_gameEntity.EnemyID}.prefab").WaitForCompletion();
        var enemyObject = GameObject.Instantiate(enemyPrefab, _enemyRoot.transform);
        _enemyView = enemyObject.GetComponent<EnemyView>();

        _battleSystem.Initialize();
        _battleSystem.OnDrawCard = OnDrawCardAsync;
        _battleSystem.OnIsPlayerWin = IsPlayerWin;
        _battleSystem.OnIsEnemyWin = IsEnemyWin;
        _battleSystem.OnPlayerWin = OnPlayerWinAsync;
        _battleSystem.OnEnemyWin = OnEnemyWinAsync;
        _battleSystem.OnLose = OnLoseAsync;
        _deckView.Initialize();

        _handView.Initialize(_battleSettings);

        _battleCore = new BattleCore(_cardList, _battleSettings, _cardComboList, _serifList, _serifToCardList);
        _battleCore.DealDefaultCard(_gameEntity.CardIdList);

        _deckView.SetDeckCount(_battleCore.DeckCards.Count);
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);
        _heroView.SetMaxHp(_gameEntity.MaxHp);
        _heroView.SetHp(_gameEntity.Hp);

        _enemy = _enemyList.GetEnemyByID(_gameEntity.EnemyID);
        _enemyView.SetStatus(_enemy);

        _battleUiView.OnSkipButtonClicked = OnSkipButtonClicked;

        _battleSystem.OnSetup = OnSetupAsync;
        _battleSystem.OnPlayerAttack = OnPlayerAttackAsync;
        _battleSystem.OnEnemyAttack = OnEnemyAttackAysnc;

        // R3でSelectedCardCountを監視し、3になったらイベントを発火
        _handView.SelectedCardCount
            .Where(count => count == 3)
            .Subscribe(_ => OnCardsSelectedAsync().Forget())
            .AddTo(_disposables);

        _effectView.OnGameOverButtonClicked = OnGameOverButtonClicked;
        _effectView.OnEndingButtonClicked = OnEndingButtonClicked;
        _effectView.SetAsLastSibling();

        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(_soundManager.PlayBgm(Ramen.Data.SoundAsset.BGM_BATTLE));
        taskList.Add(_fadeView.FadeOutAsync());
        await UniTask.WhenAll(taskList);

        _fadeView.Visible = false;

        _battleSystem.ChangeState(_battleSystem.SetupState);
    }

    public void Dispose()
    {
        _disposables?.Dispose();
    }

    private async UniTask OnDrawCardAsync()
    {
        // カードを引く
        _battleCore.DrawCards();

        // 引いたカードを手札に反映する
        foreach (var card in _battleCore.HandCards)
        {
            if (_handView.CardViewList.Where(x => x.CardData != null && x.CardData.CardID == card.CardID).Count() > 0)
            {
                continue;
            }
            
            var cardView = _handView.CardViewList.FirstOrDefault(x => x.CardData == null);

            if (cardView == null)
            {
                continue;
            }

            cardView.SetCardData(card);
        }

        // デッキの枚数を反映する
        _deckView.SetDeckCount(_battleCore.DeckCards.Count);

        // 捨てたカードの枚数を反映する
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);

        // 手札のカードを待機状態にする
        _handView.CardViewList.ForEach(x => x.SetIdelState());

        // カードを引くアニメーションを再生する
        await _handView.DrawCardAnimationAsync();

        // 手札のカードを待機状態にする
        _handView.CardViewList.Where(x => x.Visible == true).ToList().ForEach(x => x.SetWaitState());
    }

    /// <summary>
    /// カードを選択した時の処理
    /// </summary>
    private async UniTask OnCardsSelectedAsync()
    {
        // 選択した手札のカードタイプが全て違うかどうかを確認する
        var selectedCards = _handView.SelectedCards.Where(x => x.Visible == true && x.CardData != null).ToList().Select(x => x.CardData).ToList();
        var selectedCardTypes = selectedCards.Select(x => x.CardType).ToList();
        if (selectedCardTypes.Distinct().Count() != selectedCardTypes.Count)
        {
            // 選択したカードを元の位置に戻すアニメーションを再生する
            await _handView.ResetSelectedCardsAnimationAsync();
            return;
        }
        
        _battleSystem.ChangeState(_battleSystem.PlayerAttackState);
    }

    /// <summary>
    /// プレイヤーの攻撃処理
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    private async UniTask OnPlayerAttackAsync()
    {
        // 選択したカードを中央に移動するアニメーションを再生する
        await _handView.SelectedCardAnimationAsync();

        // 選択したカードを取得する
        var selectedCards = _handView.SelectedCards.Where(x => x.Visible == true && x.CardData != null).ToList().Select(x => x.CardData).ToList();

        // 選択したカードを非表示にする
        foreach (var cardView in _handView.SelectedCards)
        {
            cardView.SetCardData(null);
            cardView.Visible = false;
            cardView.SetIdelState();
            cardView.Reset();
        }
        _handView.SelectedCards.Clear();

        // 選択したカードを墓地に移動する
        _battleCore.MoveCardsToDiscard(selectedCards);

        // デッキの枚数を反映する
        _deckView.SetDeckCount(_battleCore.DeckCards.Count);

        // 捨てたカードの枚数を反映する
        _discardView.SetDiscardCount(_battleCore.DiscardCards.Count);

        // 選択したカードの攻撃力を計算する
        var attackPower = selectedCards.Sum(x => x.Power);

        // セリフのボーナスパワーを取得する
        var bonusPower = _battleCore.GetSerifBonusPower(selectedCards);

        // コンボのボーナスパワーを取得する
        foreach (var cardFrom in selectedCards)
        {
            foreach (var cardTo in selectedCards)
            {
                bonusPower += _battleCore.GetComboBonusPower(cardFrom, cardTo);
            }
        }

        // 攻撃力を計算する
        attackPower += bonusPower;

        if (attackPower <= 0)
        {
            attackPower = 0;
        }

        // プレイヤーの攻撃アニメーションを再生する
        await OnPlayerAttackAnimationAsync(attackPower, bonusPower);
    }

    /// <summary>
    /// プレイヤーの攻撃アニメーションを再生する
    /// </summary>
    /// <param name="attackPower">攻撃力</param>
    /// <param name="bonusPower">ボーナスパワー</param>
    /// <returns>アニメーションを再生する</returns>
    private async UniTask OnPlayerAttackAnimationAsync(int attackPower, int bonusPower)
    {
        _effectView.SetDamageText(attackPower);
        _effectView.SetBonusText(bonusPower);
        await _effectView.ShowPlayerAttackAsync();

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

    /// <summary>
    /// 敵の攻撃アニメーションを再生する
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    private async UniTask OnEnemyAttackAysnc()
    {
        _effectView.SetEnemyTurnSprite();
        await _effectView.ShowSlideAsync();

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

    /// <summary>
    /// プレイヤーが勝ったかどうかを判定する
    /// </summary>
    /// <returns>プレイヤーが勝ったかどうか</returns>
    private bool IsPlayerWin()
    {
        return _enemyView.GetHp() <= 0;
    }

    /// <summary>
    /// プレイヤーが勝った時の処理
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    private async UniTask OnPlayerWinAsync()
    {
        _effectView.SetGameClearSprite();
        await _effectView.ShowSlideAsync();

        await UniTask.Delay(1500);

        if (_enemy.IsBoss)
        {
            _gameEntity.Reset();
            await _effectView.ShowEndingAsync();
        }
        else
        {
            // 戦闘終了後に_gameEntityのHpを現在のHpにする
            _gameEntity.Hp = _heroView.GetHp();
            List<UniTask> taskList = new List<UniTask>();
            taskList.Add(_soundManager.StopBgmAsync());
            taskList.Add(_fadeView.FadeInAsync());
            taskList.Add(SceneManager.LoadSceneAsync("MapScene").ToUniTask());
            await UniTask.WhenAll(taskList);
        }
    }

    /// <summary>
    /// エンディングボタンがクリックされた時の処理
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    private async void OnEndingButtonClicked()
    {
        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(_soundManager.StopBgmAsync());
        taskList.Add(_fadeView.FadeInAsync());
        taskList.Add(SceneManager.LoadSceneAsync("TitleScene").ToUniTask());
        await UniTask.WhenAll(taskList);
    }

    /// <summary>
    /// 敵が勝ったかどうかを判定する
    /// </summary>
    /// <returns>敵が勝ったかどうか</returns>
    private bool IsEnemyWin()
    {
        return _heroView.GetHp() <= 0;
    }

    /// <summary>
    /// 敵が勝った時の処理
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    private async UniTask OnEnemyWinAsync()
    {
        await _effectView.ShowGameOverAsync();
    }

    /// <summary>
    /// スキップボタンがクリックされた時の処理
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
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

            _battleSystem.ChangeState(_battleSystem.EnemyAttackState);
        }
    }

    /// <summary>
    /// セットアップ処理
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    private async UniTask OnSetupAsync()
    {
        // セリフを取得
        _battleCore.SetCurrentSerif(_serifList.GetRandomNormalBattleSerif());
        _enemyView.SetSerif(_battleCore.CurrentSerif);

        _effectView.SetYourTurnSprite();
        await _effectView.ShowSlideAsync();
    }

    /// <summary>
    /// ゲームオーバーボタンがクリックされた時の処理
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    private async void OnGameOverButtonClicked()
    {
        _gameEntity.Reset();
        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(_fadeView.FadeInAsync());
        taskList.Add(SceneManager.LoadSceneAsync("TitleScene").ToUniTask());
        await UniTask.WhenAll(taskList);
    }

    /// <summary>
    /// 負けた時の処理
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    private async UniTask OnLoseAsync()
    {
        _gameEntity.Reset();
        await UniTask.Delay(3000);
        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(_soundManager.StopBgmAsync());
        taskList.Add(_fadeView.FadeInAsync());
        taskList.Add(SceneManager.LoadSceneAsync("TitleScene").ToUniTask());
        await UniTask.WhenAll(taskList);
    }
}