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
            eventButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        await _fadeView.FadeOutAsync();
        _fadeView.Visible = false;
    }

    private async void OnEventButtonClicked(EventButtonType eventButtonType, int enemyId)
    {
        Debug.Log("EventButton clicked: " + eventButtonType + " " + enemyId);

        switch (eventButtonType)
        {
            case EventButtonType.Battle:
                List<UniTask> taskList = new List<UniTask>();
                _gameEntity.EnemyID = enemyId;
                taskList.Add(SceneManager.LoadSceneAsync("BattleScene").ToUniTask());
                taskList.Add(_fadeView.FadeInAsync());
                await UniTask.WhenAll(taskList);
                break;
            case EventButtonType.CardSelect:
                break;
            case EventButtonType.Heal:
                break;
        }
    }
}
