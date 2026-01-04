using UnityEngine;
using System.Collections.Generic;

public class LiahStartRoundAbility : AbilityRuntime
{
    public GameObject buffVFXPrefab;

    public override void OnStartRound()
    {
        TeamManager team = owner == null ? null : owner.data != null ? null : null;
        // (linha acima não faz nada, vamos usar o índice do slot abaixo)
    }

    public void Activate(TeamManager team)
    {
        int index = team.slots.FindIndex(s => !s.IsEmpty && s.pet == owner);

        if (index == -1)
            return;

        List<PetRuntime> targets = new();

        // Lv1 → 1 aliado à frente
        if (owner.level >= 1 && index - 1 >= 0)
            if (!team.slots[index - 1].IsEmpty)
                targets.Add(team.slots[index - 1].pet);

        // Lv2+ → 2 aliados à frente
        if (owner.level >= 2 && index - 2 >= 0)
            if (!team.slots[index - 2].IsEmpty)
                targets.Add(team.slots[index - 2].pet);

        int healAmount = owner.level == 3 ? 2 : 1;

        foreach (var pet in targets)
        {
            pet.health += healAmount;
            pet.maxHealth += healAmount;
            
        }

        if (targets.Count > 0)
            Debug.Log($"[Liah] Buffou {targets.Count} aliado(s) com +{healAmount} HP");
    }
}
