using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class HpView : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private TextMeshProUGUI _hpText;

    void Start()
    {
        // Unirxをつかって_hpSliderのvalueを_hpTextに表示する
        _hpSlider.onValueChanged.AsObservable().Subscribe(value => _hpText.text = value.ToString());
    }

    public int Hp { 
        get { return (int)_hpSlider.value; } 
        set { _hpSlider.value = value; UpdateHpText(); } 
    }

    public int MaxHp { 
        get { return (int)_hpSlider.maxValue; } 
        set { _hpSlider.maxValue = value; UpdateHpText(); } 
    }

    private void UpdateHpText()
    {
        _hpText.text = _hpSlider.value.ToString();
    }
}
