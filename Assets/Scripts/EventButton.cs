using UnityEngine;

using UnityEngine.UI;
using R3;
using UnityEngine.Events;

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

    

    public UnityAction<EventButtonType, int> OnEventButtonClicked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _eventButton.onClick.AddListener(() => OnEventButtonClicked?.Invoke(_eventButtonType, _enemyId));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
