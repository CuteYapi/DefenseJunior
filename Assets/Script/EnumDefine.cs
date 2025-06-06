public enum EnemyType
{
    None = -1,

    Slime = 0,
    Goblin = 1,
    Orc,

    Max,
}

public enum PlayerCharacterType
{
    None = -1,

    Melee_1,
    Melee_2,

    Range_1,
    Range_2,
    Range_3,

    Max,
}

public enum FxType
{
    None = -1,

    Explosion,
    Skull,
    SwordSlash,

    Max,
}

// MessageType enum 업데이트
public enum MessageType
{
    None = -1,

    NotEnoughGold,
    PlacementConflict,
    UpgradeNotPossible,
    NotEnoughGoldForUpgrade,
    UpgradeSuccess,
    MaxLevelReached,

    Max,
}