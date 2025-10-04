using VContainer;
using VContainer.Unity;
using UnityEngine;
using Ramen.Data;

public class BattleLifetimeScope : LifetimeScope
{
    [SerializeField] DeckView _deckView;
    [SerializeField] HandView _handView;
    [SerializeField] DiscardView _discardView;
    [SerializeField] BattleSettings _battleSettings;
    [SerializeField] HeroView _heroView;
    [SerializeField] CardComboList _cardComboList;
    [SerializeField] CardList _cardList;
    [SerializeField] EnemyList _enemyList;
    [SerializeField] SerifList _serifList;
    [SerializeField] SerifToCardList _serifToCardList;
    [SerializeField] BattleUiView _battleUiView;
    [SerializeField] EffectView _effectView;
    protected override void Configure(IContainerBuilder builder)
    {
        // BattleSystemをシングルトンとして登録
        builder.Register<BattleSystem>(Lifetime.Singleton);
        builder.RegisterInstance<IDeckView>(_deckView);
        builder.RegisterInstance<IHandView>(_handView);
        builder.RegisterInstance<IDiscardView>(_discardView);
        builder.RegisterInstance(_battleSettings);
        builder.RegisterInstance<IHeroView>(_heroView);
        builder.RegisterInstance<CardComboList>(_cardComboList);
        builder.RegisterInstance<CardList>(_cardList);
        builder.RegisterInstance<EnemyList>(_enemyList);
        builder.RegisterInstance<SerifList>(_serifList);
        builder.RegisterInstance<SerifToCardList>(_serifToCardList);
        builder.RegisterInstance<IBattleUiView>(_battleUiView);
        builder.RegisterInstance<EffectView>(_effectView);
        // BattleManagerをエントリーポイントとして登録
        builder.RegisterEntryPoint<BattlePresenter>();
    }
}
