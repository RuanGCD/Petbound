public abstract class AbilityRuntime
{
    protected AbilityData data;
    protected PetRuntime owner;

    public void Init(AbilityData data, PetRuntime owner)
    {
        this.data = data;
        this.owner = owner;
    }

    public virtual void OnPreCombat(BattleContext ctx) { }
    public virtual void OnAttack(BattleContext ctx, PetRuntime target) { }
    public virtual void OnDeath(BattleContext ctx) { }
    public virtual void OnKill(BattleContext ctx, PetRuntime target) { }
}
