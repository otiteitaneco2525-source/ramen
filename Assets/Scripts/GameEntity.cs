using System;
using System.Collections.Generic;

public sealed class GameEntity
{
    public List<string> CardIdList { get; private set; }
    public bool ShowTutorial { get; set; }
    public int EnemyID { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public string CurrentEventButtonId { get; set; } // EventButtonのIDを保存
    public int BattleClearCount { get; set; }
    public List<String> ClearEventButtonIdList { get; private set; }

    public GameEntity()
    {
        CardIdList = new List<string>();
        ShowTutorial = true;
        EnemyID = 1;
        Hp = 0;
        MaxHp = 0;
        CurrentEventButtonId = null;
        BattleClearCount = 0;
        ClearEventButtonIdList = new List<string>();
    }

    public void Reset()
    {
        CardIdList.Clear();
        ShowTutorial = false;
        EnemyID = 1;
        Hp = 0;
        MaxHp = 0;
        CurrentEventButtonId = null;
        BattleClearCount = 0;
        ClearEventButtonIdList.Clear();
    }

    public bool HasCard(string cardId)
    {
        return CardIdList.Contains(cardId);
    }

    public void AddCard(string cardId)
    {
        CardIdList.Add(cardId);
    }

    // ToString
    public override string ToString()
    {
        return $"EnemyID: {EnemyID}, Hp: {Hp}, MaxHp: {MaxHp}, CurrentEventButtonId: {CurrentEventButtonId}, CardCount: {CardIdList.Count}, CardIdList: {string.Join(", ", CardIdList)}";
    }
}
