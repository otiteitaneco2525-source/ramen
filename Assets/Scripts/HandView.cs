using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine.UI;

public interface IHandView
{
    void AddCard(CardView card);
    void RemoveCard(CardView card);
    void ArrangeCards(List<CardView> cards);
    Transform GetTransform();
    void Initialize();
}

public sealed class HandView : MonoBehaviour, IHandView
{
    [SerializeField] private CardView _cardView;
    [SerializeField] float _cardSpacing;
    [SerializeField] private float _offsetX;
    [SerializeField] private float _offsetY;
    [SerializeField] private LitMotion.Ease _ease;
    [SerializeField] private int _maxCardCount;
    [SerializeField] private int _drawCardCount;

    private Canvas _canvas;
    private bool _isInitialized = false;
    private Vector2 _logicalCanvasSize;
    private Vector2 _cardSize;
    private readonly List<CardView> _cardViewList = new List<CardView>();
    private readonly List<CardView> _selectedCards = new List<CardView>();

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_isInitialized) return;

        _isInitialized = true;

        _canvas = GetComponentInParent<Canvas>();

        _logicalCanvasSize = GetLogicalCanvasSize(_canvas);

        // カードの幅を取得（最初のカードから）
        _cardSize = GetCardSize();

        CreateCard(_maxCardCount, _cardSize, _logicalCanvasSize);
    }

    private Vector2 GetLogicalCanvasSize(Canvas canvas)
    {
        var root = canvas.rootCanvas;
        return new Vector2(root.pixelRect.width  / root.scaleFactor,
                           root.pixelRect.height / root.scaleFactor);
    }

    private Vector2 GetCardSize()
    {
        CardView cardView = Instantiate(_cardView, transform);
        float cardWidth = cardView.CardWidth;
        float cardHeight = cardView.CardHeight;
        Destroy(cardView.gameObject);
        return new Vector2(cardWidth, cardHeight);
    }

    public void CreateCard(int count, Vector2 cardSize, Vector2 logicalCanvasSize)
    {
        Vector2 startPosition = new Vector2(cardSize.x / 2 + (logicalCanvasSize.x / 2), cardSize.y / 2 + (logicalCanvasSize.y / 2) * -1 + _offsetY);
        for (int i = 0; i < count; i++)
        {
            CardView cardView = Instantiate(_cardView, transform);
            cardView.Initialize();
            cardView.SetDefaultPosition(startPosition);
            cardView.OnCardSelected = OnCardSelected;
            cardView.OnCardDeselected = OnCardDeselected;
            _cardViewList.Add(cardView);
        }
    }

    private void OnCardSelected(CardView card)
    {
        _selectedCards.Add(card);
    }

    private void OnCardDeselected(CardView card)
    {
        _selectedCards.Remove(card);
    }

    public async UniTask SelectedCard()
    {
        _cardViewList.Where(x => x.Visible == true).ToList().ForEach(x => x.SetIdelState());

        List<UniTask> taskList = new List<UniTask>();

        for (int i = 0; i < _selectedCards.Count; i++)
        {
            CardView cardView = _selectedCards[i];

            Vector2 endPosition = new Vector2(0, 0);
        
            // アニメーション実行
            var motion = LMotion.Create((Vector3)cardView.RectTransform.localPosition, (Vector3)endPosition, 0.25f)
                .WithEase(Ease.Linear)
                .BindToLocalPosition(cardView.RectTransform);
            taskList.Add(motion.ToUniTask());
        }

        await UniTask.WhenAll(taskList);

        taskList.Clear();

        for (int i = 0; i < _selectedCards.Count; i++)
        {
            CardView cardView = _selectedCards[i];

            for (int j = 0; j < cardView.ImageList.Count; j++)
            {
                Image image = cardView.ImageList[j];
                image.color = new Color(1, 1, 1, 1);

                var motion = LMotion.Create(image.color, new Color(1, 1, 1, 0), 0.25f)
                    .WithEase(Ease.Linear)
                    .BindToColor(image);
                taskList.Add(motion.ToUniTask());
            }

            for (int j = 0; j < cardView.TextList.Count; j++)
            {
                TextMeshProUGUI text = cardView.TextList[j];
                text.color = new Color(1, 1, 1, 1);

                var motion2 = LMotion.Create(text.color, new Color(1, 1, 1, 0), 0.25f)
                    .WithEase(Ease.Linear)
                    .BindToColor(text);
                taskList.Add(motion2.ToUniTask());
            }
        }

        await UniTask.WhenAll(taskList);
    }

    public async UniTask DrawCard()
    {
        await DrawCard(_drawCardCount, _cardSize, _logicalCanvasSize);
    }

    public async UniTask DrawCard(int drawCount, Vector2 cardSize, Vector2 logicalCanvasSize)
    {
        var drawCardViewList = _cardViewList.Where(x => x.Visible == false).ToList();

        if (drawCardViewList.Count <= 0)
        {
            return;
        }

        _cardViewList.Where(x => x.Visible == true).ToList().ForEach(x => x.SetIdelState());

        int visibleCount = _cardViewList.Where(x => x.Visible).Count();

        Vector2 startPosition = new Vector2(cardSize.x / 2 + (logicalCanvasSize.x / 2), cardSize.y / 2 + (logicalCanvasSize.y / 2) * -1 + _offsetY);
        
        // 全カードの総幅を計算
        float totalWidth = (drawCount + visibleCount - 1) * (_offsetX + cardSize.x);
        float centerX = -totalWidth / 2f; // 中央揃えの開始位置

        if (visibleCount > 0)
        {
            var movdCardViewList = _cardViewList.Where(x => x.Visible == true).ToList();

            for (int i = 0; i < movdCardViewList.Count; i++)
            {
                CardView cardView = movdCardViewList[i];
                cardView.SetDefaultPositionY();
                cardView.SetDefaultScale();

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

        if (drawCardViewList.Count < drawCount)
        {
            drawCount = drawCardViewList.Count;
        }

        for (int i = 0; i < drawCount; i++)
        {
            CardView cardView = drawCardViewList[i];
            cardView.SetDefaultPosition();
            cardView.SetDefaultScale();
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

        _cardViewList.Where(x => x.Visible == true).ToList().ForEach(x => x.SetWaitState());
    }

    public void AddCard(CardView card)
    {
        card.transform.SetParent(transform, false);
    }

    public void RemoveCard(CardView card)
    {
        card.gameObject.SetActive(false);
    }

    public void ArrangeCards(List<CardView> cards)
    {
        var activeCards = cards.Where(x => x.gameObject.activeSelf).ToList();

        float totalWidth = (activeCards.Count - 1) * _cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < activeCards.Count; i++)
        {
            Vector2 targetPos = new Vector2(startX + i * _cardSpacing, 0f);
            activeCards[i].GetComponent<RectTransform>().anchoredPosition = targetPos;
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
