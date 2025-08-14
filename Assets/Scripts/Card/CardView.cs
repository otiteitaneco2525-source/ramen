using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public sealed class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Text nameText;
    [SerializeField] Text descriptionText;
    [SerializeField] Image icon;
    [SerializeField] Text costText;

    private CardStateBase _currentState;
    public CardWaitState WaitState { get; private set; }
    public CardSelectedState SelectedState { get; private set; }
    public CardDraggingState DraggingState { get; private set; }
    public CardIdelState IdelState { get; private set; }
    public int DefaultSiblingIndex;

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

    public void ResetPos()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
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
