using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public enum EventButtonType
{
    Battle,
    CardSelect,
    Heal,
}

public class EventButton : MonoBehaviour
{
    [SerializeField] private EventButtonType _eventButtonType;
    [SerializeField] private Button _eventButton;
    [SerializeField] private int _enemyId;
    [SerializeField] private bool _isDebug;
    [SerializeField] private List<EventButton> _nextEventButtonList;
    public string EventButtonId => _eventButtonId;
    public bool IsDebug => _isDebug;
    public UnityAction<EventButton> OnEventButtonClicked;
    public List<EventButton> NextEventButtonList => _nextEventButtonList;
    public Image Image => _image;
    public EventButtonType EventButtonType => _eventButtonType;
    public int EnemyId => _enemyId;
    public bool Interactable { get { return _eventButton.interactable; } set { _eventButton.interactable = value; } }

    private Image _image;
    private bool _isInitialized;
    private string _eventButtonId; // EventButtonの一意のID

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        _eventButtonId = gameObject.name.Split('_')[1];

        _image = GetComponent<Image>();
        _eventButton.onClick.AddListener(() => OnEventButtonClicked?.Invoke(this));
    }
}
