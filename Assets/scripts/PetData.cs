using UnityEngine;

[CreateAssetMenu(
    fileName = "NewPet",
    menuName = "AutoBattle/Pet"
)]
public class PetData : ScriptableObject
{
    [Header("Info")]
    public string petName;
    public Sprite icon;

    [Header("Stats Base")]
    public int baseAttack;
    public int baseHealth;

    [Header("Level")]
    public int maxLevel = 3;

    [Header("Ability")]
    //public AbilityData ability;

    [Header("Shop")]
    public int tier; // em que turno aparece
    public AbilityData abilityData;
}
