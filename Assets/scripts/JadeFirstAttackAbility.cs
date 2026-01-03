using UnityEngine;

public class JadeFirstAttackAbility : AbilityRuntime
{
    public override void OnAttack(BattleContext ctx, PetRuntime target)
    {
        // já atacou antes? então ignora
        if (owner.hasAttackedThisBattle)
            return;

        owner.hasAttackedThisBattle = true;

        int bonus = 0;

        switch (owner.level)
        {
            case 1: bonus = 2; break;
            case 2: bonus = 3; break;
            case 3: bonus = 5; break;
        }

        owner.tempAttackBonus += bonus;

        Debug.Log(
            $"[Jade] {owner.data.petName} ativou habilidade: +{bonus} ATK (temporário)"
        );
    }
}
