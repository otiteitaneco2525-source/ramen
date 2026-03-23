using UnityEngine;

public interface IHeroView
{
    void SetHp(int value);
    void SetMaxHp(int value);
    void Damage(int value);
    int GetHp();
    Transform GetTransform();
}

public class HeroView : MonoBehaviour, IHeroView
{
    [SerializeField] HpView _hpView;

    public int GetHp()
    {
        return _hpView.Hp;
    }

    public void SetHp(int value)
    {
        _hpView.Hp = value;
    }

    public void SetMaxHp(int value)
    {
        _hpView.MaxHp = value;
    }

    public void Damage(int value)
    {
        _hpView.Hp -= value;

        if (_hpView.Hp <= 0)
        {
            _hpView.Hp = 0;
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
