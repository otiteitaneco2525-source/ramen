using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public interface IEnemyView
{
    void SetHp(int value);
    void Damage(int value);
    int GetHp();
}

public class EnemyView : MonoBehaviour, IEnemyView
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
}
