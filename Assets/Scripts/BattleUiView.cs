using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public interface IBattleUiView
{
    UnityAction OnSkipButtonClicked { get; set; }
}

public class BattleUiView : MonoBehaviour, IBattleUiView
{
    [SerializeField] Button _skipButton;

    public UnityAction OnSkipButtonClicked { get; set; }

    void Start()
    {
        _skipButton.onClick.AddListener(() => OnSkipButtonClicked?.Invoke());
    }
}
