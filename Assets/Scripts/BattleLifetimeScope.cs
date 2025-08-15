using VContainer;
using VContainer.Unity;
using UnityEngine;

public class BattleLifetimeScope : LifetimeScope
{
    [SerializeField] DeckView _deckView;
    [SerializeField] HandView _handView;
    [SerializeField] DiscardView _discardView;
    [SerializeField] BattleSettings _battleSettings;
    [SerializeField] HeroView _heroView;
    [SerializeField] EnemyView _enemyView;
    protected override void Configure(IContainerBuilder builder)
    {
        // BattleSystemをシングルトンとして登録
        builder.Register<BattleSystem>(Lifetime.Singleton);
        builder.RegisterInstance<IDeckView>(_deckView);
        builder.RegisterInstance<IHandView>(_handView);
        builder.RegisterInstance<IDiscardView>(_discardView);
        builder.RegisterInstance(_battleSettings);
        builder.RegisterInstance<IHeroView>(_heroView);
        builder.RegisterInstance<IEnemyView>(_enemyView);
        // BattleManagerをエントリーポイントとして登録
        builder.RegisterEntryPoint<BattlePresenter>();
    }
}
