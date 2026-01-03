using UnityEngine;

public class CalopgangueOnDeathAbility : AbilityRuntime
{
    private bool triggeredThisBattle = false;

    public override void OnDeath(BattleContext ctx)
    {
        if (triggeredThisBattle)
            return;

        triggeredThisBattle = true;

        int amount = Mathf.Clamp(owner.level, 1, 3);

        ctx.battleManager.SpawnCalopsitasAtFront(amount, ctx.isPlayer);
    }

    public void ResetForBattle()
    {
        triggeredThisBattle = false;
    }
}
