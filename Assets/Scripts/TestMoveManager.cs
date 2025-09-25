using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using System.Linq;

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

    [SerializeField] private int _maxCardCount;

    [SerializeField] private int _drawCardCount;

    [SerializeField] private Button _drawButton;

    private readonly List<CardView> _cardViewList = new List<CardView>();

    void Start()
    {
        Vector2 logicalCanvasSize = GetLogicalCanvasSize(_canvas);

        CreateCard(_maxCardCount);
        
        // カードの幅を取得（最初のカードから）
        Vector2 cardSize = GetCardSize();
        

        _drawButton.onClick.AddListener(async () => await DrawCard(_drawCardCount, cardSize, logicalCanvasSize));

        // await DrawCard(_drawCardCount, cardSize, logicalCanvasSize);
    }

    public async UniTask DrawCard(int drawCount, Vector2 cardSize, Vector2 logicalCanvasSize)
    {
        Vector2 startPosition = new Vector2(cardSize.x / 2 + (logicalCanvasSize.x / 2), cardSize.y / 2 + (logicalCanvasSize.y / 2) * -1 + _offsetY);
        int visibleCount = _cardViewList.Where(x => x.Visible).Count();
        
        // 全カードの総幅を計算
        float totalWidth = (drawCount + visibleCount - 1) * (_offsetX + cardSize.x);
        float centerX = -totalWidth / 2f; // 中央揃えの開始位置

        if (visibleCount > 0)
        {
            var movdCardViewList = _cardViewList.Where(x => x.Visible == true).ToList();

            for (int i = 0; i < movdCardViewList.Count; i++)
            {
                CardView cardView = movdCardViewList[i];
                Vector2 endPosition = new Vector2(centerX + i * (_offsetX + cardSize.x), startPosition.y);
            
                cardView.RectTransform.localPosition = cardView.RectTransform.localPosition;
            
                // アニメーション実行
                var motion = LMotion.Create((Vector3)cardView.RectTransform.localPosition, (Vector3)endPosition, 1.0f)
                    .WithEase(_ease)
                    .BindToLocalPosition(cardView.RectTransform);
                _ = motion;

                await UniTask.Delay(100);    
            }
        }

        var drawCardViewList = _cardViewList.Where(x => x.Visible == false).ToList();

        if (drawCardViewList.Count < drawCount)
        {
            drawCount = drawCardViewList.Count;
        }

        for (int i = 0; i < drawCount; i++)
        {
            CardView cardView = drawCardViewList[i];
            cardView.Visible = true;
            
            // 各カードの終了位置を計算（中央に移動）
            Vector2 endPosition = new Vector2(centerX + (i + visibleCount) * (_offsetX + cardSize.x), startPosition.y);
            
            cardView.RectTransform.localPosition = startPosition;
            
            // アニメーション実行
            var motion = LMotion.Create((Vector3)startPosition, (Vector3)endPosition, 1.0f)
                .WithEase(_ease)
                .BindToLocalPosition(cardView.RectTransform);
            _ = motion;

            await UniTask.Delay(100);
        }
    }

    public void CreateCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CardView cardView = Instantiate(_cardView, _cardParent);
            cardView.Visible = false;
            _cardViewList.Add(cardView);
        }
    }

    public Vector2 GetCardSize()
    {
        CardView firstCard = _cardViewList[0];
        float cardWidth = firstCard.CardWidth;
        float cardHeight = firstCard.CardHeight;
        return new Vector2(cardWidth, cardHeight);
    }

    public Vector2 GetLogicalCanvasSize(Canvas canvas)
    {
        var root = canvas.rootCanvas;
        return new Vector2(root.pixelRect.width  / root.scaleFactor,
                           root.pixelRect.height / root.scaleFactor);
    }
}
