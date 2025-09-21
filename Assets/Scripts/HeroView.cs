using UnityEngine;
using UnityEngine.UI;

public interface IHeroView
{
    void SetHp(int value);
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
        _hpSlider.maxValue = _hp;
        _hpSlider.value = _hp;
    }

    public void Damage(int value)
    {
        _hp -= value;
        _hpSlider.value = _hp;

        if (_hp <= 0)
        {
            _hp = 0;
            _hpSlider.value = _hp;
            Debug.Log("HeroObjがやられた");
            // Destroy(gameObject);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
