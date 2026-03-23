using UnityEngine;
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
    [SerializeField] TextMeshProUGUI _serifText;
    [SerializeField] private GameObject _serifGameObject;
    [SerializeField] HpView _hpView;
    private Enemy _enemy;

    private void Start()
    {
        _serifGameObject.SetActive(false);
        _serifText.text = string.Empty;
    }

    public int GetHp()
    {
        return _hpView.Hp;
    }

    public void Damage(int value)
    {
        _hpView.Hp -= value;

        if (_hpView.Hp <= 0)
        {
            _hpView.Hp = 0;
        }
    }

    public void SetStatus(Enemy enemy)
    {
        _enemy = enemy;
        _hpView.MaxHp = enemy.HP;
        _hpView.Hp = enemy.HP;
    }

    public int GetAttackPower()
    {
        return _enemy.AttackPower;
    }

    public void SetSerif(Serif serif)
    {
        _serifText.text = serif.SerifName;
        _serifGameObject.SetActive(true);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
