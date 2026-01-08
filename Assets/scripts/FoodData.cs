using UnityEngine;
public enum FoodType
{
    PermanentStat,
    TemporaryStat,
    Effect,
    XP
}

[CreateAssetMenu(
    fileName = "NewFood",
    menuName = "AutoBattle/Food"
)]

public class FoodData : ScriptableObject
{
    public string foodName;
    public Sprite icon;

    [TextArea]
    public string description;

    public FoodType type;

    // stats
    public int attackBonus;
    public int healthBonus;

    // efeito especial
    public AbilityData ability;

    public int tier;
}

