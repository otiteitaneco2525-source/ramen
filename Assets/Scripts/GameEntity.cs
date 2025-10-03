using System.Collections.Generic;
using UnityEngine;

public sealed class GameEntity
{
    public List<string> GetCardList { get; private set; }

    public bool ShowTutorial { get; set; }



    public GameEntity()
    {
        GetCardList = new List<string>();
        ShowTutorial = true;
    }

    public bool HasCard(string cardId)
    {
        return GetCardList.Contains(cardId);
    }

    public void AddCard(string cardId)
    {
        GetCardList.Add(cardId);
    }

}
