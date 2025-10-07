using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class TestMoveManager : MonoBehaviour
{
    [SerializeField] private HandView _handView;
    [SerializeField] private Button _drawButton;
    [SerializeField] private Button _selectedButton;
    [SerializeField] private EffectView _effectView;
    [SerializeField] private Button _effectButton;
    [SerializeField] private Button _slideButton;
    [SerializeField] private BattleSettings _battleSettings;
    void Start()
    {
        _handView.Initialize(_battleSettings);

        _drawButton.onClick.AddListener(async () => await _handView.DrawCardAsync());

        _selectedButton.onClick.AddListener(async () => await _handView.SelectedCard());

        _effectButton.onClick.AddListener(() => _effectView.ShowPlayerAttackAsync().Forget());

        _slideButton.onClick.AddListener(() => {
            // _effectView.SetYourTurnSprite();
            // _effectView.SetEnemyTurnSprite();
            // _effectView.SetGameClearSprite();
            // _effectView.ShowSlideAsync();
            _effectView.ShowEndingAsync().Forget();
        });
    }
}
