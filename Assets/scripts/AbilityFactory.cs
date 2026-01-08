using UnityEngine;

public static class AbilityFactory
{
    public static AbilityRuntime CreateRuntime(AbilityData data)
    {
        switch (data.abilityID)
        {
            case "pretinha_precombat":
                return new PretinhaPreCombatAbility();

            case "jade_first_attack":
                return new JadeFirstAttackAbility();

            case "calopgangue_on_death":
                return new CalopgangueOnDeathAbility();

            case "lupi_first_purchase":
                return new LupiFirstPurchaseAbility();
            
            case "liah_start_round":
                return new LiahStartRoundAbility();

            case "mingal_ally_on_buy_or_invoke":
                return new MingalAllyOnEnterAbility();

            case "gael_on_consume_food":
                return new GaelOnConsumeFoodAbility();

            case "orion_free_reroll":
                return new OrionFreeRerollAbility();


        }

        Debug.LogWarning("Ability não encontrada: " + data.abilityID);
        return null;
    }
}
