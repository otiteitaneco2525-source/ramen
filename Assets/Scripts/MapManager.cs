using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using VContainer;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Inject] private readonly FadeView _fadeView;
    [Inject] private readonly GameEntity _gameEntity;

    [SerializeField] private Button _tutorialButton;
    private List<EventButton> _eventButtons = new List<EventButton>();

    async void Start()
    {
        _tutorialButton.gameObject.SetActive(_gameEntity.ShowTutorial);

        if (_gameEntity.ShowTutorial)
        {
            _tutorialButton.onClick.AddListener(() => 
            {
                _gameEntity.ShowTutorial = false;
                _tutorialButton.gameObject.SetActive(_gameEntity.ShowTutorial);
            });
        }

        _eventButtons = FindObjectsByType<EventButton>(FindObjectsSortMode.None).ToList();

        foreach (var eventButton in _eventButtons)
        {
            eventButton.OnEventButtonClicked += OnEventButtonClicked;
        }

        await _fadeView.FadeOutAsync();
        _fadeView.Visible = false;
    }

    private void OnEventButtonClicked(EventButtonType eventButtonType, int enemyId)
    {
        Debug.Log("EventButton clicked: " + eventButtonType + " " + enemyId);

        switch (eventButtonType)
        {
            case EventButtonType.Battle:
                break;
            case EventButtonType.CardSelect:
                break;
            case EventButtonType.Heal:
                break;
        }
    }
}
