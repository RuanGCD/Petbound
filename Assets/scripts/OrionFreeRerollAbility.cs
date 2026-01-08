using UnityEngine;

public class OrionFreeRerollAbility : AbilityRuntime
{
    private int freeRerollsRemaining;

    public override void OnStartRound()
    {
        switch (owner.level)
        {
            case 1:
                freeRerollsRemaining = 1;
                break;
            case 2:
            case 3:
                freeRerollsRemaining = 2;
                break;
        }
    }

    public override bool TryFreeReroll()
    {
        if (freeRerollsRemaining > 0)
        {
            freeRerollsRemaining--;
            return true;
        }

        return false;
    }

    public override void OnFreeRerollUsed()
    {
        if (owner.level == 3)
        {
            // ganha +1 ouro
            GameObject.FindObjectOfType<PlayerState>().gold += 1;
            Debug.Log("✨ Orion Lv3: +1 ouro após reroll gratuito");
        }
    }
}

