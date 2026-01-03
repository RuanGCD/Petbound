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


        }

        Debug.LogWarning("Ability não encontrada: " + data.abilityID);
        return null;
    }
}
