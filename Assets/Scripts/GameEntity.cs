using System.Collections.Generic;

public sealed class GameEntity
{
    public List<string> CardIdList { get; private set; }
    public bool ShowTutorial { get; set; }
    public int EnemyID { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get; set; }

    public GameEntity()
    {
        CardIdList = new List<string>();
        ShowTutorial = true;
        EnemyID = 1;
        Hp = 0;
        MaxHp = 0;
    }

    public bool HasCard(string cardId)
    {
        return CardIdList.Contains(cardId);
    }

    public void AddCard(string cardId)
    {
        CardIdList.Add(cardId);
    }

}
