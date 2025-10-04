using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.Events;

public class HealView : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _frontFrontImage;
    [SerializeField] private Image _frontImage;
    [SerializeField] private Image _character1Image;
    [SerializeField] private Image _character2mage;
    [SerializeField] private Button _closeButton;

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
        _character1Image.gameObject.SetActive(false);
        _character2mage.gameObject.SetActive(false);
        _closeButton.gameObject.SetActive(false);

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

        _character1Image.gameObject.SetActive(true);
        var character1ImageColor = _character1Image.color;
        character1ImageColor.a = 0;
        var character1ImageToColor = character1ImageColor;
        character1ImageToColor.a = 1;
        var motion4 = LMotion.Create(character1ImageColor, character1ImageToColor, 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_character1Image);
        await motion4.ToUniTask();

        await UniTask.Delay(1500);

        _character1Image.gameObject.SetActive(false);

        _character2mage.gameObject.SetActive(true);
        var character2mageColor = _character2mage.color;
        character2mageColor.a = 0;
        var character2mageToColor = character2mageColor;
        character2mageToColor.a = 1;
        var motion5 = LMotion.Create(character2mageColor, character2mageToColor, 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_character2mage);
        await motion5.ToUniTask();

        _closeButton.gameObject.SetActive(true);
    }
}
