using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using VContainer.Unity;
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
            eventButton.OnEventButtonClicked += OnEventButtonClicked;
            eventButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        List<UniTask> taskList = new List<UniTask>();
        taskList.Add(_soundManager.PlayBgm(Ramen.Data.SoundAsset.BGM_MAP));
        taskList.Add(_fadeView.FadeOutAsync());
        await UniTask.WhenAll(taskList);

        _fadeView.Visible = false;
    }

    private async void OnEventButtonClicked(EventButtonType eventButtonType, int enemyId)
    {
        Debug.Log("EventButton clicked: " + eventButtonType + " " + enemyId);

        switch (eventButtonType)
        {
            case EventButtonType.Battle:
                List<UniTask> taskList = new List<UniTask>();

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
                break;
            case EventButtonType.CardSelect:
                _cardSelectView.DealCards(_gameEntity.CardIdList);
                _cardSelectView.OnShowAsync().Forget();
                break;
            case EventButtonType.Heal:
                _healView.OnShowAsync().Forget();
                break;
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
