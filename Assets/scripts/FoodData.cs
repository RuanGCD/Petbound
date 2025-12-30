using UnityEngine;

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

    public AbilityData ability;
    public int tier;
}
