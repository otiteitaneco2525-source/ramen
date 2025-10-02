using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;
using Cysharp.Threading.Tasks;

public class FadeView : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeTime;

    public bool Visible { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }

    public async UniTask FadeInAsync()
    {
        gameObject.SetActive(true);
        _fadeImage.gameObject.SetActive(true);

        var color = _fadeImage.color;
        color.a = 0;
        _fadeImage.color = color;

        var toColor = color;
        toColor.a = 1;

        var motion = LMotion.Create(_fadeImage.color, toColor, _fadeTime)
            .WithEase(Ease.Linear)
            .BindToColor(_fadeImage);
        await motion.ToUniTask();
    }

    public async UniTask FadeOutAsync()
    {
        gameObject.SetActive(true);
        _fadeImage.gameObject.SetActive(true);
        var color = _fadeImage.color;
        color.a = 1;
        _fadeImage.color = color;

        var toColor = color;
        toColor.a = 0;

        var motion = LMotion.Create(_fadeImage.color, toColor, _fadeTime)
            .WithEase(Ease.Linear)
            .BindToColor(_fadeImage);
        await motion.ToUniTask();
    }
}
