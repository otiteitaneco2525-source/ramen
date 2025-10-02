using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private List<EventButton> _eventButtons = new List<EventButton>();

    void Start()
    {
        _eventButtons = FindObjectsByType<EventButton>(FindObjectsSortMode.None).ToList();

        foreach (var eventButton in _eventButtons)
        {
            eventButton.OnEventButtonClicked += OnEventButtonClicked;
        }
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
