using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialView : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Button _tutorialButton_1;
    [SerializeField] private Button _tutorialButton_2;
    [SerializeField] private TextMeshProUGUI _tutorialText;
    public bool ButtonVisible_1 { 
        get { return _tutorialButton_1.gameObject.activeSelf; } 
        set { _tutorialButton_1.gameObject.SetActive(value); _backgroundImage.gameObject.SetActive(value); } 
    }

    public bool ButtonVisible_2 { 
        get { return _tutorialButton_2.gameObject.activeSelf; } 
        set { _tutorialButton_2.gameObject.SetActive(value); _backgroundImage.gameObject.SetActive(value); } 
    }
    public bool TextVisible { get { return _tutorialText.gameObject.activeSelf; } set { _tutorialText.gameObject.SetActive(value); } }

    private void Start()
    {
        _tutorialButton_1.onClick.AddListener(() => {ButtonVisible_1 = false; ButtonVisible_2 = true;});
        _tutorialButton_2.onClick.AddListener(() => {ButtonVisible_1 = false; ButtonVisible_2 = false;});
    }

    private void Update()
    {
        if (TextVisible && Input.GetKeyDown(KeyCode.Q))
        {
            ButtonVisible_1 = !ButtonVisible_1;
            ButtonVisible_2 = false;
        }
    }
}
