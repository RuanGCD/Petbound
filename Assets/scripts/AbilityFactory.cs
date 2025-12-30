using UnityEngine;

public static class AbilityFactory
{
    public static AbilityRuntime CreateRuntime(AbilityData data)
    {
        switch (data.abilityID)
        {
            case "pretinha_precombat":
                return new PretinhaPreCombatAbility();

            // próximas habilidades aqui
        }

        Debug.LogWarning("Ability não encontrada: " + data.abilityID);
        return null;
    }
}
