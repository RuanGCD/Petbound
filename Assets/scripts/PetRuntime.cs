[System.Serializable]
public class PetRuntime
{
    public PetData data;
    public int attack;
    public int health;
    public int maxHealth;


    public int level = 1;
    public int xp = 0;
    public AbilityRuntime ability;
    public PetRuntime(PetData data)
    {
         this.data = data;

        attack = data.baseAttack;
        health = data.baseHealth;
        maxHealth = data.baseHealth;
        
         if (data.abilityData != null)
    {
        ability = AbilityFactory.CreateRuntime(data.abilityData);
        ability.Init(data.abilityData, this);
    }
    }

    public int GetXpToLevel()
    {
        if (level == 1) return 2;
        if (level == 2) return 3;
        return 0;
    }

    public bool CanGainXp()
    {
        return level < 3;
    }

    public bool TryLevelUp()
    {
        if (!CanGainXp())
            return false;

        if (xp >= GetXpToLevel())
        {
            level++;
            xp = 0;
            return true;
        }

        return false;
    }
}
