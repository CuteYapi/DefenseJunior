using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/SpawnData", order = 1)]
public class SpawnData : ScriptableObject
{
    public int Type;
    public int Count;
}
