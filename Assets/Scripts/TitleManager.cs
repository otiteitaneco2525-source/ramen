using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using R3;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Button _titleButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _titleButton.onClick.AddListener(OnTitleButtonClick);
    }

    private void OnTitleButtonClick()
    {
        Debug.Log("TitleButton clicked");        
    }
}
