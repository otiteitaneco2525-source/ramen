using UnityEngine;
using UnityEngine.EventSystems;

public class MapScrollView : MonoBehaviour
{
    [SerializeField] private float _centerX;
    [SerializeField] private float _maxOffsetX;
    [SerializeField] private RectTransform _scrollRectTransform;

    // 実行すると引数のEventButtonのLocalPosition.xが_centerXになるようにOffsetXを計算して_scrollRectTransform.localPosition.xに設定する
    // ただし、OffsetXは_maxOffsetXを超えないようにする
    public void OnScroll(EventButton eventButton)
    {
        float offsetX = _centerX - eventButton.transform.localPosition.x;
        offsetX = Mathf.Clamp(offsetX, -_maxOffsetX, _maxOffsetX);
        _scrollRectTransform.localPosition = new Vector3(offsetX, _scrollRectTransform.localPosition.y, _scrollRectTransform.localPosition.z);
    }
}