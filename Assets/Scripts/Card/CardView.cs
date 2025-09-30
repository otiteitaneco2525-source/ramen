using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;
using Ramen.Data;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public sealed class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ICardView
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] Image _cardImage;
    [SerializeField] TextMeshProUGUI _costText;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private List<Image> _imageList;
    [SerializeField] private List<TextMeshProUGUI> _textList;
    [SerializeField] private Color _defaultColor;

    private CardStateBase _currentState;
    public CardWaitState WaitState { get; private set; }
    public CardSelectedState SelectedState { get; private set; }
    public CardDraggingState DraggingState { get; private set; }
    public CardIdelState IdelState { get; private set; }
    public int DefaultSiblingIndex;

    // カードデータ
    public Card CardData { get; private set; }
    public bool Visible { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }
    public UnityAction<CardView> OnCardSelected;
    public UnityAction<CardView> OnCardDeselected;
    public float CardWidth => _rectTransform.rect.width;
    public float CardHeight => _rectTransform.rect.height;
    public RectTransform RectTransform => _rectTransform;
    public List<Image> ImageList => _imageList;
    public List<TextMeshProUGUI> TextList => _textList;
    public Image CardImage => _cardImage;
    private Vector2 _defaultPosition;

    public void Initialize()
    {
        Visible = false;
        WaitState = new CardWaitState(this);
        SelectedState = new CardSelectedState(this);
        DraggingState = new CardDraggingState(this);
        IdelState = new CardIdelState(this);

        ChangeState(IdelState);
    }

    public void Reset()
    {
        foreach (var image in ImageList)
        {
            Color color = image.color;
            color.a = 1;
            image.color = color;
        }

        foreach (var text in TextList)
        {
            Color color = text.color;
            color.a = 1;
            text.color = color;
        }

        SetDefaultPositionY();
        SetDefaultScale();
    }

    public void SetDefaultPosition(Vector2 position)
    {
        _defaultPosition = position;
        transform.localPosition = position;
    }

    /// <summary>
    /// カードデータを設定
    /// </summary>
    /// <param name="card">カードデータ</param>
    public void SetCardData(Card card)
    {
        CardData = card;
        UpdateDisplay(card);
    }

    /// <summary>
    /// 表示を更新
    /// </summary>
    private void UpdateDisplay(Card card)
    {
        // cardがnullの場合は何もしない
        if (card == null)
        {
            return;
        }

        if (_nameText != null)
        {
            _nameText.text = card.Name;
        }
            
        if (_costText != null)
        {
            _costText.text = card.Power.ToString();
        }

        if (_cardImage != null)
        {
            _cardImage.sprite = null;
            // Addressablesで画像を読み込む
            Addressables.LoadAssetAsync<Sprite>($"Assets/Images/Card/{card.CardID}.png").Completed += (handle) =>
            {
                _cardImage.sprite = handle.Result;
            };
        }
    }

    public void SetIdelState()
    {
        ChangeState(IdelState);
    }

    public void SetDefaultPosition()
    {
        transform.localPosition = _defaultPosition;
    }

    public void SetDefaultPositionY()
    {
        transform.localPosition = new Vector2(transform.localPosition.x, _defaultPosition.y);
    }
    public void SetDefaultScale()
    {
        transform.localScale = Vector3.one;
    }

    public void SetWaitState()
    {
        ChangeState(WaitState);
    }

    public void ChangeState(CardStateBase newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState?.OnEnter();
    }

    private void Update()
    {
        _currentState?.OnUpdate();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _currentState?.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _currentState?.OnPointerExit(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _currentState?.OnPointerClick(eventData);
    }
}
