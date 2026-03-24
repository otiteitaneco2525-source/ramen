using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class RecyclingView : MonoBehaviour
{
    [SerializeField] private Button _recyclingButton;
    [SerializeField] private TextMeshProUGUI _recyclingText;

    public UnityAction OnRecyclingButtonClicked;

    void Start()
    {
        _recyclingButton.onClick.AddListener(() => OnRecyclingButtonClicked?.Invoke());
    }

    public void SetRecyclingCount(int count)
    {
        _recyclingText.text = count.ToString();
    }
}