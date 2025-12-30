using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("Progressão")]
    public int round = 1;
    public int wins = 0;
    public int life = 10;

    [Header("Ouro")]
    public int gold = 10;
    public int maxGold = 10;

    [Header("Custos")]
    public int petCost = 3;
    public int foodCost = 3;
    public int rerollCost = 1;

    //  Valida se pode gastar
    public bool CanAfford(int cost)
    {
        return gold >= cost;
    }

    //  Gasta ouro
    public bool SpendGold(int cost)
    {
        if (!CanAfford(cost))
            return false;

        gold -= cost;
        return true;
    }

    //  Novo round
    public void StartNewRound()
    {
        round++;
        gold = maxGold;
    }

    //  Vitória
    public void WinBattle()
    {
        wins++;
    }

    //  Derrota
    public void LoseLife(int damage)
    {
        life -= damage;
        if (life < 0)
            life = 0;
    }
}
