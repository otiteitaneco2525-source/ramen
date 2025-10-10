using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using VContainer;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Ramen.Data;

public class MapManager : MonoBehaviour
{
    [Inject] private readonly FadeView _fadeView;
    [Inject] private readonly GameEntity _gameEntity;
    [Inject] private readonly SoundManager _soundManager;

    [SerializeField] private Button _tutorialButton;
    [SerializeField] private HealView _healView;
    [SerializeField] private BattleSettings _battleSettings;
    [SerializeField] private CardSelectView _cardSelectView;
    [SerializeField] private EnemyList _enemyList;
    [SerializeField] private MapScrollView _mapScrollView;
    [SerializeField] private EventButton _firstEventButton;

    private List<EventButton> _eventButtons = new List<EventButton>();

    async void Start()
    {
        _battleSettings.SetDefaultCardId(_gameEntity.CardIdList);

        _healView.OnCloseButtonClicked += OnCloseButtonClicked;
        _tutorialButton.gameObject.SetActive(_gameEntity.ShowTutorial);

        _cardSelectView.OnCardBuy += OnCardBuy;

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
            eventButton.Initialize();
            eventButton.OnEventButtonClicked = OnEventButtonClicked;
            if (!eventButton.IsDebug)
            {
                eventButton.Image.color = new Color(1, 1, 1, 0);
            }

            if (eventButton.IsDebug)
            {
                eventButton.OnEventButtonClicked = OnDebugEventButtonClicked;
            }
            
#if !UNITY_EDITOR
            if (eventButton.IsDebug)
            {
                eventButton.gameObject.SetActive(false);
            }
#endif
        }

        // 前回のEventButtonを復元するか、初回なら_firstEventButtonを使う
        EventButton currentEventButton = null;
        if (!string.IsNullOrWhiteSpace(_gameEntity.CurrentEventButtonId))
        {
            currentEventButton = _eventButtons.FirstOrDefault(eb => eb.EventButtonId == _gameEntity.CurrentEventButtonId);
        }
        
        if (currentEventButton == null)
        {
            currentEventButton = _firstEventButton;
        }

        _mapScrollView.MoveToCurrentImage(currentEventButton);
        _mapScrollView.OnScroll(currentEventButton);

        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(_soundManager.PlayBgm(Ramen.Data.SoundAsset.BGM_MAP));
        taskList.Add(_fadeView.FadeOutAsync());
        await UniTask.WhenAll(taskList);

        _fadeView.Visible = false;
    }

    // デバッグ用のeventButtonをクリックした時の処理
    private async void OnDebugEventButtonClicked(EventButton eventButton)
    {
        Debug.Log("DebugEventButton clicked: " + eventButton.EventButtonType + " " + eventButton.EnemyId);

        List<UniTask> taskList = new List<UniTask>();
        _gameEntity.EnemyID = eventButton.EnemyId;
        taskList.Add(SceneManager.LoadSceneAsync("BattleScene").ToUniTask());
        taskList.Add(_soundManager.StopBgmAsync());
        taskList.Add(_fadeView.FadeInAsync());
        await UniTask.WhenAll(taskList);
    }

    private async void OnEventButtonClicked(EventButton eventButton)
    {
        Debug.Log("EventButton clicked: " + eventButton.EventButtonType + " " + eventButton.EnemyId);

        // GameEntityのCurrentEventButtonIdがNullの場合、引数のeventButtonが_firstEventButtonであるかチェックする
        if (string.IsNullOrWhiteSpace(_gameEntity.CurrentEventButtonId) && eventButton == _firstEventButton)
        {
            _gameEntity.CurrentEventButtonId = eventButton.EventButtonId;
        }
        // GameEntityのCurrentEventButtonIdがNullでない場合、引数のeventButtonがCurrentEventButtonのNextEventButtonListに含まれているかチェックする
        else if (!string.IsNullOrWhiteSpace(_gameEntity.CurrentEventButtonId))
        {
            var currentEventButton = _eventButtons.FirstOrDefault(eb => eb.EventButtonId == _gameEntity.CurrentEventButtonId);
            if (currentEventButton != null && currentEventButton.NextEventButtonList.Contains(eventButton))
            {
                _gameEntity.CurrentEventButtonId = eventButton.EventButtonId;
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }

        foreach (var e in _eventButtons)
        {
            e.Interactable = false;
        }

        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(_mapScrollView.MoveToCurrentImageAsync(eventButton));
        taskList.Add(_mapScrollView.OnScrollAsync(eventButton));
        await UniTask.WhenAll(taskList);

        switch (eventButton.EventButtonType)
        {
            case EventButtonType.Battle:
                taskList.Clear();

                int enemyId = eventButton.EnemyId;

                // 敵IDが0の場合、ボス以外の敵をランダムで選択する
                if (enemyId == 0)
                {
                    var candidates = _enemyList.Enemies.Where(enemy => !enemy.IsBoss).ToList();
                    if (candidates.Count > 0)
                    {
                        var randomIndex = UnityEngine.Random.Range(0, candidates.Count);
                        enemyId = candidates[randomIndex].EnemyID;
                    }
                }
                _gameEntity.EnemyID = enemyId;
                taskList.Add(SceneManager.LoadSceneAsync("BattleScene").ToUniTask());
                taskList.Add(_soundManager.StopBgmAsync());
                taskList.Add(_fadeView.FadeInAsync());
                await UniTask.WhenAll(taskList);
                // シーン遷移するため、この後の処理はスキップ
                return;
            case EventButtonType.CardSelect:
                _cardSelectView.DealCards(_gameEntity.CardIdList);
                await _cardSelectView.OnShowAsync();
                break;
            case EventButtonType.Heal:
                await _healView.OnShowAsync();
                break;
        }

        foreach (var e in _eventButtons)
        {
            e.Interactable = true;
        }
    }

    private void OnCloseButtonClicked()
    {
        _healView.Visible = false;
    }

    private void OnCardBuy(Card card)
    {
        _gameEntity.CardIdList.Add(card.CardID);
        _cardSelectView.Visible = false;
    }
}
