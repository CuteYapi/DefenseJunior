using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacter", menuName = "ScriptableObjects/PlayerCharacterData", order = 1)]
public class PlayerCharacterData : ScriptableObject
{
    public float AttackDamage;
    public float AttackCooldown;

    public int BuildCost;
}
