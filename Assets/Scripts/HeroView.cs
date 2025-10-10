using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] Slider _hpSlider;
    private int _hp;

    public int GetHp()
    {
        return _hp;
    }

    public void SetHp(int value)
    {
        _hp = value;
        _hpSlider.value = _hp;
    }

    public void SetMaxHp(int value)
    {
        _hpSlider.maxValue = value;
    }

    public void Damage(int value)
    {
        _hp -= value;
        _hpSlider.value = _hp;

        if (_hp <= 0)
        {
            _hp = 0;
            _hpSlider.value = _hp;
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
