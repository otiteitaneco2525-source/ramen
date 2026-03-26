using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyPrefabList", menuName = "Scriptable Objects/EnemyPrefabList")]
public class EnemyPrefabList : ScriptableObject
{
    public List<EnemyPrefabData> List;
}

[Serializable]
public class EnemyPrefabData
{
    public int EnemyID;
    public GameObject EnemyPrefab;
}