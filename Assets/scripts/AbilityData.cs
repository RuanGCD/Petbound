using UnityEngine;

[CreateAssetMenu(menuName = "AutoBattle/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityID;

    [Header("Triggers")]
    public bool onPreCombat;
    public bool onAttack;
    public bool onDeath;
    public bool onKill;

    [Header("Values")]
    public int value1;
    public int value2;
    public float valueFloat;
}
