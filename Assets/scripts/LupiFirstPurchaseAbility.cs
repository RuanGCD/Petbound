using UnityEngine;

public class LupiFirstPurchaseAbility : AbilityRuntime
{
    public override void OnFirstShopPurchase()
    {
        if (owner == null)
            return;

        switch (owner.level)
        {
            case 1:
                owner.health += 1;
                owner.maxHealth += 1;
                break;

            case 2:
                owner.health += 2;
                owner.maxHealth += 2;
                break;

            case 3:
                owner.health += 2;
                owner.maxHealth += 2;
                owner.attack += 1;
                break;
        }

        Debug.Log($"[Lupi] Ativou habilidade de compra – Lv {owner.level}");
    }
}
