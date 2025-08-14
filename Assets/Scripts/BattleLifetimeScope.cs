using VContainer;
using VContainer.Unity;

public class BattleLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // BattleSystemをシングルトンとして登録
        builder.Register<BattleSystem>(Lifetime.Singleton);
        
        // BattleManagerをエントリーポイントとして登録
        builder.RegisterEntryPoint<BattlePresenter>();
    }
}
