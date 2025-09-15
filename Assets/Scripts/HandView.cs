using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IHandView
{
    void AddCard(CardView card);
    void RemoveCard(CardView card);
    void ArrangeCards(List<CardView> cards);
    Transform GetTransform();
}

public sealed class HandView : MonoBehaviour, IHandView
{
    [SerializeField] float _cardSpacing;

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
