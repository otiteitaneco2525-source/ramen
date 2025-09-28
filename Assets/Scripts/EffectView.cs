using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;

public class EffectView : MonoBehaviour
{
    [SerializeField] private Image _playerAttackImage;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _bonusText;
    [SerializeField] private Image _slideImage;
    [SerializeField] private Sprite _yourTurnSprite;
    [SerializeField] private Sprite _enemyTurnSprite;
    [SerializeField] private Sprite _gameClearSprite;

    public void SetDamageText(int damage)
    {
        _damageText.text = damage.ToString();
    }

    public void SetBonusText(int bonus)
    {
        _bonusText.text = bonus.ToString();
    }

    public void SetYourTurnSprite()
    {
        _slideImage.sprite = _yourTurnSprite;
    }
    
    public void SetEnemyTurnSprite()
    {
        _slideImage.sprite = _enemyTurnSprite;
    }

    public void SetGameClearSprite()
    {
        _slideImage.sprite = _gameClearSprite;
    }

    public async UniTask ShowSlideImage()
    {
        List<UniTask> taskList = new List<UniTask>();

        _slideImage.gameObject.SetActive(true);
        _slideImage.SetNativeSize();
        _slideImage.color = new Color(1, 1, 1, 0);
        _slideImage.transform.localScale = Vector3.zero;

        var motion = LMotion.Create(_slideImage.color, new Color(1, 1, 1, 1), 0.25f)
            .WithEase(Ease.OutElastic)
            .BindToColor(_slideImage);
        taskList.Add(motion.ToUniTask());

        var motion2 = LMotion.Create(_slideImage.transform.localScale, Vector3.one, 0.25f)
            .WithEase(Ease.OutElastic)
            .BindToLocalScale(_slideImage.transform);
        taskList.Add(motion2.ToUniTask());

        await UniTask.WhenAll(taskList);

        await UniTask.Delay(1000);

        _slideImage.gameObject.SetActive(false);
    }

    public async UniTask ShowPlayerAttackAsync()
    {
        List<UniTask> taskList = new List<UniTask>();
        
        _playerAttackImage.gameObject.SetActive(true);

        _playerAttackImage.color = new Color(1, 1, 1, 0);
        _playerAttackImage.transform.localScale = Vector3.zero;

        var motion = LMotion.Create(_playerAttackImage.color, new Color(1, 1, 1, 1), 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_playerAttackImage);
        taskList.Add(motion.ToUniTask());

        var motion2 = LMotion.Create(_playerAttackImage.transform.localScale, Vector3.one, 0.25f)
            .WithEase(Ease.OutElastic)
            .BindToLocalScale(_playerAttackImage.transform);
        taskList.Add(motion2.ToUniTask());

        await UniTask.WhenAll(taskList);

        await UniTask.Delay(1000);

        _playerAttackImage.gameObject.SetActive(false);
    }
}
