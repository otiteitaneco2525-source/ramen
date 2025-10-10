using UnityEngine;
using UnityEngine.UI;
using Ramen.Data;
using TMPro;

public interface IEnemyView
{
    void Damage(int value);
    int GetHp();
    void SetStatus(Enemy enemy);
    int GetAttackPower();
    void SetSerif(Serif serif);
    Transform GetTransform();
}

public class EnemyView : MonoBehaviour, IEnemyView
{
    [SerializeField] Slider _hpSlider;
    [SerializeField] TextMeshProUGUI _serifText;
    private int _hp;
    private Enemy _enemy;

    public int GetHp()
    {
        return _hp;
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

    public void SetStatus(Enemy enemy)
    {
        _enemy = enemy;
        _hp = enemy.HP;
        _hpSlider.maxValue = _hp;
        _hpSlider.value = _hp;
    }

    public int GetAttackPower()
    {
        return _enemy.AttackPower;
    }

    public void SetSerif(Serif serif)
    {
        _serifText.text = serif.SerifName;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
