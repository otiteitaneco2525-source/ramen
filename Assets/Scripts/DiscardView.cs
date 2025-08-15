using UnityEngine;
using TMPro;

public interface IDiscardView
{
    void SetDiscardCount(int count);
}

public sealed class DiscardView : MonoBehaviour, IDiscardView
{
    [SerializeField] TextMeshProUGUI _deckCountText;

    public void SetDiscardCount(int count)
    {
        _deckCountText.text = count.ToString();
    }
}