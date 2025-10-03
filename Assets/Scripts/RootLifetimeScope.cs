using VContainer;
using VContainer.Unity;
using UnityEngine;

public class RootLifetimeScope : LifetimeScope
{
    [SerializeField] private FadeView _fadeViewPrefab;
    
    protected override void Configure(IContainerBuilder builder)
    {       
        // FadeViewのプレハブからインスタンスを作成して登録
        builder.RegisterComponentInNewPrefab<FadeView>(_fadeViewPrefab, Lifetime.Singleton).DontDestroyOnLoad();
        builder.Register<GameEntity>(Lifetime.Singleton);
    }
}
