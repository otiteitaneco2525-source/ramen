using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;
using Ramen.Data;

public sealed class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _costText;

    private CardStateBase _currentState;
    public CardWaitState WaitState { get; private set; }
    public CardSelectedState SelectedState { get; private set; }
    public CardDraggingState DraggingState { get; private set; }
    public CardIdelState IdelState { get; private set; }
    public int DefaultSiblingIndex;

    // カードデータ
    public Card CardData { get; private set; }

    // カードの状態管理プロパティ
    public bool IsInDeck { get; private set; }
    public bool IsInHand { get; private set; }
    public bool IsInDiscard { get; private set; }

    public UnityAction<CardView> OnCardSelected;
    public UnityAction<CardView> OnCardDeselected;

    /// <summary>
    /// カードデータを設定
    /// </summary>
    /// <param name="card">カードデータ</param>
    public void SetCardData(Card card)
    {
        CardData = card;
        UpdateDisplay();
    }

    /// <summary>
    /// 表示を更新
    /// </summary>
    private void UpdateDisplay()
    {
        if (CardData != null)
        {
            if (_nameText != null)
                _nameText.text = CardData.Name;
            
            if (_costText != null)
                _costText.text = CardData.Power.ToString();
            
            // アイコンの設定（必要に応じて実装）
            // if (_icon != null && CardData.Icon != null)
            //     _icon.sprite = CardData.Icon;
        }
    }

    /// <summary>
    /// デッキに配置
    /// </summary>
    public void SetInDeck()
    {
        IsInDeck = true;
        IsInHand = false;
        IsInDiscard = false;
        gameObject.SetActive(false); // デッキ内のカードは非表示
    }

    /// <summary>
    /// 手札に配置
    /// </summary>
    public void SetInHand()
    {
        IsInDeck = false;
        IsInHand = true;
        IsInDiscard = false;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 破棄札に配置
    /// </summary>
    public void SetInDiscard()
    {
        IsInDeck = false;
        IsInHand = false;
        IsInDiscard = true;
        SetWaitState();
        gameObject.SetActive(false); // 破棄札のカードは非表示
    }

    /// <summary>
    /// 現在の状態を取得
    /// </summary>
    /// <returns>カードの現在の状態</returns>
    public string GetCurrentLocation()
    {
        if (IsInDeck) return "デッキ";
        if (IsInHand) return "手札";
        if (IsInDiscard) return "破棄札";
        return "不明";
    }

    public void SetWaitState()
    {
        transform.localScale = Vector3.one;
        ChangeState(WaitState);
    }

    private void Awake()
    {
        WaitState = new CardWaitState(this);
        SelectedState = new CardSelectedState(this);
        DraggingState = new CardDraggingState(this);
        IdelState = new CardIdelState(this);

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
