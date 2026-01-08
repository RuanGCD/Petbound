using UnityEngine;

public class MingalAllyOnEnterAbility : AbilityRuntime
{
    private bool usedThisShopRound;

    public override void OnStartRound()
    {
        usedThisShopRound = false;
    }

    // 🛒 LOJA
    public void OnShopAllyEntered(PetRuntime ally)
    {
        if (usedThisShopRound)
            return;

        if (ally == owner)
            return;

        ApplyBuff(ally);
        usedThisShopRound = true;

        Debug.Log("[MINGAL] Ativou na LOJA");
    }

    // ⚔️ BATALHA
    public void OnBattleAllyInvoked(PetRuntime ally)
    {
        if (ally == owner)
            return;

        ApplyBuff(ally);
        Debug.Log("[MINGAL] Ativou em INVOCACÃO DE BATALHA");
    }

    void ApplyBuff(PetRuntime ally)
    {
        int bonusAtk = owner.level switch
        {
            1 => 1,
            2 => 2,
            3 => 3,
            _ => 0
        };

        ally.attack += bonusAtk;
    }
}
