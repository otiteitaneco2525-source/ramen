using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.Events;
using TMPro;

public class HealView : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _frontFrontImage;
    [SerializeField] private Image _frontImage;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _character1Text;
    [SerializeField] private TextMeshProUGUI _character2Text;

    public bool Visible { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }
    public UnityAction OnCloseButtonClicked { get; set; }

    void Start()
    {
        _closeButton.onClick.AddListener(() => OnCloseButtonClicked?.Invoke());
    }

    public async UniTask OnShowAsync()
    {
        gameObject.SetActive(true);
        _frontImage.gameObject.SetActive(false);
        _frontImage.transform.localPosition = new Vector3(0, 1080, 0);
        _frontFrontImage.gameObject.SetActive(false);
        _closeButton.gameObject.SetActive(false);
        _character1Text.gameObject.SetActive(false);
        _character2Text.gameObject.SetActive(false);

        var backgroundImageFromColor = _backgroundImage.color;
        backgroundImageFromColor.a = 0;
        var backgroundImageToColor = backgroundImageFromColor;
        backgroundImageToColor.a = 1;

        var motion = LMotion.Create(backgroundImageFromColor, backgroundImageToColor, 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_backgroundImage);
        await motion.ToUniTask();

        _frontImage.gameObject.SetActive(true);
        var motion2 = LMotion.Create(_frontImage.transform.localPosition, Vector3.zero, 0.25f)
            .WithEase(Ease.OutBack)
            .BindToLocalPosition(_frontImage.transform);
        await motion2.ToUniTask();

        _frontFrontImage.gameObject.SetActive(true);
        var frontFrontImageColor = _frontFrontImage.color;
        frontFrontImageColor.a = 0;
        var frontFrontImageToColor = frontFrontImageColor;
        frontFrontImageToColor.a = 1;
        var motion3 = LMotion.Create(frontFrontImageColor, frontFrontImageToColor, 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_frontFrontImage);
        await motion3.ToUniTask();

        _character1Text.gameObject.SetActive(true);
        var character1TextColor = _character1Text.color;
        character1TextColor.a = 0;
        var character1TextToColor = character1TextColor;
        character1TextToColor.a = 1;
        var motion5 = LMotion.Create(character1TextColor, character1TextToColor, 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_character1Text);
        await motion5.ToUniTask();

        await UniTask.Delay(1500);

        _character1Text.gameObject.SetActive(false);

        _character2Text.gameObject.SetActive(true);
        var character2TextColor = _character2Text.color;
        character2TextColor.a = 0;
        var character2TextToColor = character2TextColor;
        character2TextToColor.a = 1;
        var motion6 = LMotion.Create(character2TextColor, character2TextToColor, 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_character2Text);
        await motion6.ToUniTask();

        _closeButton.gameObject.SetActive(true);
    }
}
