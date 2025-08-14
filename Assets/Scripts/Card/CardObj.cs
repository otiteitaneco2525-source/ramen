using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardObj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Text nameText;
    [SerializeField] Text descriptionText;
    [SerializeField] Image icon;
    [SerializeField] Text costText;

    private CardStateBase currentState;
    public CardWaitState WaitState { get; private set; }
    public CardSelectedState SelectedState { get; private set; }
    public CardDraggingState DraggingState { get; private set; }
    public CardIdelState IdelState { get; private set; }
    public int defaultSiblingIndex;

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
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }

    public void ResetPos()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        currentState?.OnUpdate();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentState?.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentState?.OnPointerExit(eventData);
    }
}
