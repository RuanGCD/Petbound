using UnityEngine;

public class GaelOnConsumeFoodAbility : AbilityRuntime
{
    private bool usedThisRound = false;

    public override void OnStartRound()
    {
        usedThisRound = false;
    }

    public override void OnConsumeFood(FoodData food)
    {
        if (usedThisRound)
            return;

        usedThisRound = true;

        int healthBonus = 0;
        int attackBonus = 1;

        switch (owner.level)
        {
            case 1:
                healthBonus = 1;
                break;
            case 2:
                healthBonus = 2;
                break;
            case 3:
                healthBonus = 3;
                break;
        }

        owner.health += healthBonus;
        owner.maxHealth += healthBonus;
        owner.attack += attackBonus;

        Debug.Log($"🧙 Gael ativou (1ª comida): +{healthBonus} HP, +{attackBonus} ATK");
    }
}

