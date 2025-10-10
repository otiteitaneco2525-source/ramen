using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "BattleSettings", menuName = "Scriptable Objects/BattleSettings")]
public class BattleSettings : ScriptableObject
{
    public int MaxCardCount;
    public int DrawCount;
    public int HeroHp;
    public int SerifBonusPower;
    public string DefaultCardId;
    public int AddMaxHp;

    public void SetDefaultCardId(List<string> cardIdList)
    {
        if (cardIdList.Count == 0)
        {
            DefaultCardId.Split(',').ToList().ForEach(x => {
                cardIdList.Add(x);
            });
        }
    }
}
