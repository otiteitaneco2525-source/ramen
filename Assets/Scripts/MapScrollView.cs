using UnityEngine;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.UI;
using System.Linq;

public class MapScrollView : MonoBehaviour
{
    [SerializeField] private float _centerX;
    [SerializeField] private float _maxOffsetX;
    [SerializeField] private RectTransform _scrollRectTransform;
    [SerializeField] private GameObject _checkMarkPrefab;
    [SerializeField] private GameObject _arrowPrefab;

    // 実行すると引数のEventButtonのLocalPosition.xが_centerXになるようにOffsetXを計算して_scrollRectTransform.localPosition.xに設定する
    // ただし、OffsetXは_maxOffsetXを超えないようにする
    public void OnScroll(EventButton eventButton)
    {
        float offsetX = _centerX - eventButton.transform.localPosition.x;
        offsetX = Mathf.Clamp(offsetX, -_maxOffsetX, _maxOffsetX);
        _scrollRectTransform.localPosition = new Vector3(offsetX, _scrollRectTransform.localPosition.y, _scrollRectTransform.localPosition.z);
    }

    // LitMotionを使用してスクロールをアニメーションで行う非同期版
    public async UniTask OnScrollAsync(EventButton eventButton, float duration = 0.25f)
    {
        float offsetX = _centerX - eventButton.transform.localPosition.x;
        offsetX = Mathf.Clamp(offsetX, -_maxOffsetX, _maxOffsetX);
        
        Vector3 targetPosition = new Vector3(offsetX, _scrollRectTransform.localPosition.y, _scrollRectTransform.localPosition.z);
        
        var motion = LMotion.Create(_scrollRectTransform.localPosition, targetPosition, duration)
            .WithEase(Ease.OutCubic)
            .BindToLocalPosition(_scrollRectTransform);
        await motion.ToUniTask();
    }

    public void CreateCheckMarkImage(EventButton eventButton)
    {
        Instantiate(_checkMarkPrefab, eventButton.transform);
    }

    public void CreateArrowImage(EventButton eventButton)
    {
        Instantiate(_arrowPrefab, eventButton.transform);
    }

    // _currentImage.transform.localPositionの位置を引数のEventButtonのtransform.localPositionの位置にLitmotionで移動する
    public async UniTask MoveToCurrentImageAsync(EventButton eventButton)
    {
        // var motion = LMotion.Create(_currentImage.transform.localPosition, eventButton.transform.localPosition, 0.25f)
        //     .WithEase(Ease.Linear)
        //     .BindToLocalPosition(_currentImage.transform);
        // await motion.ToUniTask();
        await UniTask.Delay(1000);
    }
}