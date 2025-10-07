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
    public bool IsDebug => _isDebug;

    public UnityAction<EventButtonType, int> OnEventButtonClicked;

    void Start()
    {
        _eventButton.onClick.AddListener(() => OnEventButtonClicked?.Invoke(_eventButtonType, _enemyId));
    }
}
