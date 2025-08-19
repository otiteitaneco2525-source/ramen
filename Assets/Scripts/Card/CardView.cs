using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public sealed class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI costText;

    private CardStateBase _currentState;
    public CardWaitState WaitState { get; private set; }
    public CardSelectedState SelectedState { get; private set; }
    public CardDraggingState DraggingState { get; private set; }
    public CardIdelState IdelState { get; private set; }
    public int DefaultSiblingIndex;

    public delegate void CardSelectedEventHandler(CardView card);
    public event CardSelectedEventHandler OnCardSelected;

    public delegate void CardDeselectedEventHandler(CardView card);
    public event CardDeselectedEventHandler OnCardDeselected;

    public void Select()
    {
        OnCardSelected?.Invoke(this);
    }

    public void Deselect()
    {
        OnCardDeselected?.Invoke(this);
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
