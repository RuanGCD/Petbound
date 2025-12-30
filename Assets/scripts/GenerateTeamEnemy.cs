using UnityEngine;
using System.Collections.Generic;

public class EnemyTeamGenerator : MonoBehaviour
{
    public TeamManager enemyTeam;
    public PlayerState playerState;
    public List<PetData> availablePets;

    public void GenerateEnemyTeam()
    {
        if (enemyTeam == null || playerState == null)
        {
            Debug.LogError("[EnemyTeamGenerator] Referências faltando");
            return;
        }

        ClearEnemyTeam();

        int petCount = Mathf.Min(playerState.round, enemyTeam.maxSlots);

        List<PetData> validPets =
            availablePets.FindAll(p => p.tier <= playerState.round);

        if (validPets.Count == 0)
        {
            Debug.LogWarning("[EnemyTeamGenerator] Nenhum pet válido");
            return;
        }

        for (int i = 0; i < petCount; i++)
        {
            PetData data = validPets[Random.Range(0, validPets.Count)];
            PetRuntime pet = new PetRuntime(data);

            enemyTeam.slots[i].pet = pet;
        }
    }

    void ClearEnemyTeam()
    {
        foreach (var slot in enemyTeam.slots)
            slot.pet = null;
    }
}
