using System.Collections.Generic;
using UnityEngine;
using Ramen.Data;
using System.Linq;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.Events;

public class CardSelectView : MonoBehaviour
{
    [SerializeField] private CardView _cardViewPrefab;
    [SerializeField] private CardList _cardList;
    [SerializeField] private Transform _cardContainer;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _frontFrontImage;
    [SerializeField] private Image _character1Image;
    [SerializeField] private float _offsetX;
    [SerializeField] private float _offsetY;

    public bool Visible { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }

    public UnityAction<Card> OnCardBuy;

    private List<CardView> _cardViews = new List<CardView>();
    private List<Card> _selectedCards = new List<Card>();
    private Vector2 _cardSize;
    private Canvas _canvas;
    private Vector2 _logicalCanvasSize;

    public void DealCards(List<string> cardIDs)
    {
        // cardIDsに含まれないカードを取得
        var availableCards = _cardList.Cards.Where(card => !cardIDs.Contains(card.CardID)).ToList();

        // CardTypeごとに一種類ずつ選択
        _selectedCards.Clear();
        var selectedTypes = new HashSet<CardType>();

        // 利用可能なカードをランダムにシャッフル
        var shuffledCards = availableCards.OrderBy(x => UnityEngine.Random.Range(0f, 1f)).ToList();

        foreach (var card in shuffledCards)
        {
            if (!selectedTypes.Contains(card.CardType))
            {
                _selectedCards.Add(card);
                selectedTypes.Add(card.CardType);
            }
        }

        // 既存のカードビューをクリア
        ClearCardViews();

        // カードビューを作成
        _cardSize = CreateCardViews(_selectedCards);

        _canvas = _canvas ?? GetComponentInParent<Canvas>();

        _logicalCanvasSize = GetLogicalCanvasSize(_canvas);
    }

    public async UniTask OnShowAsync()
    {
        List<UniTask> taskList = new List<UniTask>();
        gameObject.SetActive(true);

        _frontFrontImage.gameObject.SetActive(true);
        _frontFrontImage.gameObject.SetActive(false);
        _character1Image.gameObject.SetActive(false);

        var backgroundImageFromColor = _backgroundImage.color;
        backgroundImageFromColor.a = 0;
        var backgroundImageToColor = backgroundImageFromColor;
        backgroundImageToColor.a = 1;

        var motion = LMotion.Create(backgroundImageFromColor, backgroundImageToColor, 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_backgroundImage);
        taskList.Add(motion.ToUniTask());

        _frontFrontImage.gameObject.SetActive(true);
        var frontFrontImageColor = _frontFrontImage.color;
        frontFrontImageColor.a = 0;
        var frontFrontImageToColor = frontFrontImageColor;
        frontFrontImageToColor.a = 1;
        var motion2 = LMotion.Create(frontFrontImageColor, frontFrontImageToColor, 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_frontFrontImage);
        taskList.Add(motion2.ToUniTask());

        _character1Image.gameObject.SetActive(true);
        var character1ImageColor = _character1Image.color;
        character1ImageColor.a = 0;
        var character1ImageToColor = character1ImageColor;
        character1ImageToColor.a = 1;
        var motion3 = LMotion.Create(character1ImageColor, character1ImageToColor, 0.25f)
            .WithEase(Ease.Linear)
            .BindToColor(_character1Image);
        taskList.Add(motion3.ToUniTask());

        await UniTask.WhenAll(taskList);

        taskList.Clear();

        // 全カードの総幅を計算
        float totalWidth = (_cardViews.Count - 1) * (_offsetX + _cardSize.x);
        float centerX = -totalWidth / 2f; // 中央揃えの開始位置
        Vector2 startPosition = new Vector2(_cardSize.x / 2 + (_logicalCanvasSize.x / 2), _offsetY);

        // 新しいカードの移動処理を行う
        for (int i = 0; i < _cardViews.Count; i++)
        {
            CardView cardView = _cardViews[i];
            cardView.SetDefaultScale();
            cardView.RectTransform.localPosition = startPosition;
            cardView.Visible = true;
            cardView.ChangeState(cardView.ShopState);
            cardView.OnCardBuy = () => {
                OnCardBuy?.Invoke(cardView.CardData);
            };

            // 各カードの終了位置を計算（中央に移動）
            Vector2 endPosition = new Vector2(centerX + (i) * (_offsetX + _cardSize.x), startPosition.y);
            
            // アニメーション実行
            var motion5 = LMotion.Create((Vector3)startPosition, (Vector3)endPosition, 1.0f)
                .WithEase(Ease.OutBack)
                .BindToLocalPosition(cardView.RectTransform);
            taskList.Add(motion5.ToUniTask());

            await UniTask.Delay(100);
        }

        await UniTask.WhenAll(taskList);
    }

    private void ClearCardViews()
    {
        foreach (var cardView in _cardViews)
        {
            if (cardView != null)
            {
                Destroy(cardView.gameObject);
            }
        }
        _cardViews.Clear();
    }

    private Vector2 GetLogicalCanvasSize(Canvas canvas)
    {
        var root = canvas.rootCanvas;
        return new Vector2(root.pixelRect.width  / root.scaleFactor,
                           root.pixelRect.height / root.scaleFactor);
    }

    private Vector2 CreateCardViews(List<Card> cards)
    {
        var cardSize = Vector2.zero;
        foreach (var card in cards)
        {
            var cardView = Instantiate(_cardViewPrefab, _cardContainer);
            cardView.Initialize();
            cardView.SetCardData(card);
            _cardViews.Add(cardView);

            float cardWidth = cardView.CardWidth;
            float cardHeight = cardView.CardHeight;
            cardSize = new Vector2(cardWidth, cardHeight);
        }

        return cardSize;
    }
}
