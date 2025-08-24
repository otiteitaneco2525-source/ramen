using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Ramen.Data;
using TMPro;

public interface IEnemyView
{
    void Damage(int value);
    int GetHp();
    void SetStatus(Enemy enemy);
    int GetAttackPower();
    void SetSerif(Serif serif);
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

        // 自身を揺らしたい
        transform.DOShakePosition(0.3f, 10f, 10, 90f, false, true);

        if (_hp <= 0)
        {
            _hp = 0;
            _hpSlider.value = _hp;
            Debug.Log("EnemyObjがやられた");
            // Destroy(gameObject);
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
}
