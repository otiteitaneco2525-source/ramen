using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UniRx;

public interface IHandView
{
    void Initialize(BattleSettings battleSettings);
    List<CardView> CardViewList { get; }
    List<CardView> SelectedCards { get; }
    Subject<int> SelectedCardCount { get; }
    UniTask DrawCardAnimationAsync();
    UniTask SelectedCardAnimationAsync();
    UniTask ResetSelectedCardsAnimationAsync();
}

public sealed class HandView : MonoBehaviour, IHandView
{
    [SerializeField] private CardView _cardView;
    [SerializeField] float _cardSpacing;
    [SerializeField] private float _offsetX;
    [SerializeField] private float _offsetY;
    [SerializeField] private LitMotion.Ease _ease;
    [SerializeField] private float _canvasXsize;

    private Canvas _canvas;
    private bool _isInitialized = false;
    private Vector2 _logicalCanvasSize;
    private Vector2 _cardSize;
    private readonly List<CardView> _cardViewList = new List<CardView>();
    private readonly List<CardView> _selectedCards = new List<CardView>();
    private BattleSettings _battleSettings;
    private readonly Subject<int> _selectedCardCount = new Subject<int>();

    public List<CardView> CardViewList => _cardViewList;
    public List<CardView> SelectedCards => _selectedCards;
    public Subject<int> SelectedCardCount => _selectedCardCount;

    public void Initialize(BattleSettings battleSettings)
    {
        if (_isInitialized) return;

        _isInitialized = true;

        _selectedCardCount.OnNext(0);

        _battleSettings = battleSettings;

        _canvas = GetComponentInParent<Canvas>();

        _logicalCanvasSize = GetLogicalCanvasSize(_canvas);

        // カードの幅を取得（最初のカードから）
        _cardSize = GetCardSize();

        CreateCard(_battleSettings.MaxCardCount, _cardSize, _logicalCanvasSize);
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

    /// <summary>
    /// カードを作成する
    /// </summary>
    /// <param name="count">作成するカードの枚数</param>
    /// <param name="cardSize">カードのサイズ</param>
    /// <param name="logicalCanvasSize">論理キャンバスのサイズ</param>
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

    /// <summary>
    /// カードを選択した時の処理
    /// </summary>
    /// <param name="card">選択したカード</param>
    private void OnCardSelected(CardView card)
    {
        _selectedCards.Add(card);
        _selectedCardCount.OnNext(_selectedCards.Count);
    }

    /// <summary>
    /// カードを選択解除した時の処理
    /// </summary>
    /// <param name="card">選択解除したカード</param>
    private void OnCardDeselected(CardView card)
    {
        _selectedCards.Remove(card);
        _selectedCardCount.OnNext(_selectedCards.Count);
    }

    /// <summary>
    /// 選択したカードを中央に移動するアニメーションを再生する
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    public async UniTask SelectedCardAnimationAsync()
    {
        _cardViewList.Where(x => x.Visible == true).ToList().ForEach(x => x.SetIdelState());

        List<UniTask> taskList = new List<UniTask>();

        foreach (var cardView in _selectedCards)
        {
            Vector2 endPosition = new Vector2(0, 0);
        
            // アニメーション実行
            var motion = LMotion.Create((Vector3)cardView.RectTransform.localPosition, (Vector3)endPosition, 0.25f)
                .WithEase(Ease.Linear)
                .BindToLocalPosition(cardView.RectTransform);
            taskList.Add(motion.ToUniTask());
        }

        await UniTask.WhenAll(taskList);

        taskList.Clear();

        foreach (var cardView in _selectedCards)
        {
            foreach (var image in cardView.ImageList)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

                var motion = LMotion.Create(image.color, new Color(image.color.r, image.color.g, image.color.b, 0), 0.25f)
                    .WithEase(Ease.Linear)
                    .BindToColor(image);
                taskList.Add(motion.ToUniTask());
            }

            foreach (var text in cardView.TextList)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

                var motion2 = LMotion.Create(text.color, new Color(text.color.r, text.color.g, text.color.b, 0), 0.25f)
                    .WithEase(Ease.Linear)
                    .BindToColor(text);
                taskList.Add(motion2.ToUniTask());
            }
        }

        await UniTask.WhenAll(taskList);
    }

    /// <summary>
    /// 選択したカードを元の位置に戻すアニメーションを再生する
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    public async UniTask ResetSelectedCardsAnimationAsync()
    {
        List<UniTask> taskList = new List<UniTask>();

        foreach (var cardView in _selectedCards)
        {
            cardView.SetIdelState();
        }

        // 選択したカードを元の位置に戻す
        foreach (var cardView in _selectedCards)
        {
            // 元の位置に戻すアニメーションを再生する
            var motion = LMotion.Create((Vector3)cardView.RectTransform.localPosition, (Vector3)new Vector2(cardView.RectTransform.localPosition.x, cardView.DefaultY), 0.25f)
                .WithEase(Ease.Linear)
                .BindToLocalPosition(cardView.RectTransform);
            taskList.Add(motion.ToUniTask());

            cardView.SetDefaultScale();
        }

        await UniTask.WhenAll(taskList);

        foreach (var cardView in _selectedCards)
        {
            cardView.SetWaitState();
        }

        _selectedCards.Clear();
        _selectedCardCount.OnNext(0);
    }

    /// <summary>
    /// カードを引くアニメーションを再生する
    /// </summary>
    /// <returns>アニメーションを再生する</returns>
    public async UniTask DrawCardAnimationAsync()
    {
        await DrawCardAnimationAsync(_battleSettings.DrawCount, _cardSize, _logicalCanvasSize);
    }

    /// <summary>
    /// カードを引くアニメーションを再生する
    /// </summary>
    /// <param name="drawCount">引くカードの枚数</param>
    /// <param name="cardSize">カードのサイズ</param>
    /// <param name="logicalCanvasSize">論理キャンバスのサイズ</param>
    /// <returns>アニメーションを再生する</returns>
    private async UniTask DrawCardAnimationAsync(int drawCount, Vector2 cardSize, Vector2 logicalCanvasSize)
    {
        List<UniTask> taskList = new List<UniTask>();
        
        var drawCardViewList = _cardViewList.Where(x => x.Visible == false && x.CardData != null).ToList();

        if (drawCardViewList.Count <= 0)
        {
            return;
        }

        int visibleCount = _cardViewList.Where(x => x.Visible).Count();

        if (drawCardViewList.Count < drawCount)
        {
            drawCount = drawCardViewList.Count;
        }

        float startX = cardSize.x + logicalCanvasSize.x;
        float startY = cardSize.y / 2 + (logicalCanvasSize.y / 2) * -1 + _offsetY;
        float offsetX = _offsetX;

        // 全カードの総幅を計算（カード幅 + OffsetX間隔）
        int totalCardCount = drawCount + visibleCount;
        // 表示領域が_canvasXsizeを超える場合、offsetXを調整してカードが重なるようにする
        if (totalCardCount > 1)
        {
            float maxOffsetX = (_canvasXsize - totalCardCount * cardSize.x) / (totalCardCount - 1);
            offsetX = Mathf.Min(offsetX, maxOffsetX);
        }
        float totalWidth = totalCardCount * cardSize.x + (totalCardCount - 1) * offsetX;
        float drawCardStartX = (logicalCanvasSize.x / 2) - (totalWidth / 2);

        // 既にカードが表示されている場合は移動する
        if (visibleCount > 0)
        {
            var movdCardViewList = _cardViewList.Where(x => x.Visible == true).ToList();

            for (int i = 0; i < movdCardViewList.Count; i++)
            {
                CardView cardView = movdCardViewList[i];
                cardView.SetDefaultPositionY();
                cardView.SetDefaultScale();
                // 左のカードが前面、右のカードが背面になるように表示順を設定
                cardView.transform.SetSiblingIndex(totalCardCount - 1 - i);

                Vector2 endPosition = new Vector2(drawCardStartX + (i * (offsetX + cardSize.x)), startY);
            
                // アニメーション実行
                var motion = LMotion.Create((Vector3)cardView.RectTransform.localPosition, (Vector3)endPosition, 1.0f)
                    .WithEase(_ease)
                    .BindToLocalPosition(cardView.RectTransform);
                taskList.Add(motion.ToUniTask());

                await UniTask.Delay(100);    
            }
        }

        if (drawCardViewList.Count < drawCount)
        {
            drawCount = drawCardViewList.Count;
        }

        // 新しいカードの移動処理を行う
        for (int i = visibleCount; i < drawCount + visibleCount; i++)
        {
            CardView cardView = drawCardViewList[i - visibleCount];
            cardView.SetDefaultPosition();
            cardView.SetDefaultScale();
            cardView.Visible = true;
            // 左のカードが前面、右のカードが背面になるように表示順を設定
            cardView.transform.SetSiblingIndex(totalCardCount - 1 - i);
            
            // 各カードの終了位置を計算（offsetX分の間隔で横に並べる）
            Vector2 endPosition = new Vector2(drawCardStartX + (i * (offsetX + cardSize.x)), startY);
            
            // アニメーション実行
            var motion = LMotion.Create(new Vector3(startX, startY), (Vector3)endPosition, 1.0f)
                .WithEase(_ease)
                .BindToLocalPosition(cardView.RectTransform);
            taskList.Add(motion.ToUniTask());

            await UniTask.Delay(100);
        }

        await UniTask.WhenAll(taskList);
    }
}
