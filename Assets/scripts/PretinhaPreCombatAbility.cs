using UnityEngine;
using System.Collections.Generic;

public class PretinhaPreCombatAbility : AbilityRuntime
{
    public override void OnPreCombat(BattleContext ctx)
    {
        Debug.Log($"[Pretinha] OnPreCombat ativado | Level: {owner.level}");

        int hits = owner.level;

        List<PetRuntime> enemies = ctx.enemyTeam.GetLivingPets();
        if (enemies.Count == 0)
            return;

        for (int i = 0; i < hits; i++)
        {
            if (enemies.Count == 0)
                break;

            PetRuntime target = enemies[Random.Range(0, enemies.Count)];
            target.health -= 1;
            
            ctx.battleManager.PlayArrowEffect(owner, target);

            Debug.Log($"[Pretinha] Acertou {target.data.petName}, vida agora = {target.health}");

            if (target.health <= 0)
            {
                Debug.Log($"[Pretinha] {target.data.petName} morreu no pré-combate");
                enemies.Remove(target);
            }
        }
    }
}
