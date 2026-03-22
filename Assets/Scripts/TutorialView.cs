using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialView : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private TextMeshProUGUI _tutorialText;
    public bool ButtonVisible { 
        get { return _tutorialButton.gameObject.activeSelf; } 
        set { _tutorialButton.gameObject.SetActive(value); _backgroundImage.gameObject.SetActive(value); } 
    }
    
    public bool TextVisible { get { return _tutorialText.gameObject.activeSelf; } set { _tutorialText.gameObject.SetActive(value); } }

    private void Start()
    {
        _tutorialButton.onClick.AddListener(() => ButtonVisible = false);
    }

    private void Update()
    {
        if (TextVisible && Input.GetKeyDown(KeyCode.Q))
        {
            ButtonVisible = !ButtonVisible;
        }
    }
}
