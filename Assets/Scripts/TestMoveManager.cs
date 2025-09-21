using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;

public class TestMoveManager : MonoBehaviour
{


    [SerializeField] private CardView _cardView;
    [SerializeField] private Transform _cardParent;

    [SerializeField] private CanvasScaler _canvasScaler;

    [SerializeField] private Canvas _canvas;


    [SerializeField] private Vector2 _basePosition;

    [SerializeField] private float _offsetX;

    [SerializeField] private float _offsetY;

    

    [SerializeField] private LitMotion.Ease _ease;

    [SerializeField] private float _drawCardCount;
    
    async UniTask Start()
    {
        Vector2 logicalCanvasSize = GetLogicalCanvasSize(_canvas);
        
        // カードの幅を取得（最初のカードから）
        CardView firstCard = Instantiate(_cardView, _cardParent);
        float cardWidth = firstCard.CardWidth;
        float cardHeight = firstCard.CardHeight;
        Destroy(firstCard.gameObject);
        
        // 全カードの総幅を計算
        float totalWidth = (_drawCardCount - 1) * (_offsetX + cardWidth);
        float x = -totalWidth / 2f; // 中央揃えの開始位置

        Vector2 startPosition = new Vector2(cardWidth / 2 + (logicalCanvasSize.x / 2), cardHeight / 2 + (logicalCanvasSize.y / 2) * -1 + _offsetY);
        
        for (int i = 0; i < _drawCardCount; i++)
        {
            CardView cardView = Instantiate(_cardView, _cardParent);
            
            // 各カードの終了位置を計算（中央に移動）
            Vector2 endPosition = new Vector2(x + i * (_offsetX + cardWidth), startPosition.y);
            
            cardView.RectTransform.localPosition = startPosition;
            
            // アニメーション実行
            var motion = LMotion.Create((Vector3)startPosition, (Vector3)endPosition, 1.0f)
                .WithEase(_ease)
                .BindToLocalPosition(cardView.RectTransform);
            _ = motion;

            await UniTask.Delay(100);
        }
    }

    public Vector2 GetLogicalCanvasSize(Canvas canvas)
    {
        var root = canvas.rootCanvas;
        return new Vector2(root.pixelRect.width  / root.scaleFactor,
                           root.pixelRect.height / root.scaleFactor);
    }
}
