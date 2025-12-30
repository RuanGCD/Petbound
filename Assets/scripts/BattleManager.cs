using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    [Header("References")]
    public PlayerState playerState;
    public TeamManager playerTeam;
    public TeamManager enemyTeam;
    public EnemyTeamGenerator enemyGenerator;

    [Header("UI")]
    public GameObject shopCanvas;
    public GameObject battleCanvas;

    [Header("Battle Slots UI")]
    public List<TeamSlotUI> playerBattleSlots;
    public List<TeamSlotUI> enemyBattleSlots;

    [Header("Battle Config")]
    public float impactDelay = 0.25f;
    public float deathDelay = 0.35f;
    public float attackDelay = 0.6f;
    [Header("Effects")]
    public ArrowProjectile arrowPrefab;


    void Start()
    {
        if (battleCanvas != null)
            battleCanvas.SetActive(false);
    }

    public void StartBattle()
{
    if (playerTeam == null || enemyTeam == null || enemyGenerator == null)
    {
        Debug.LogError("[BattleManager] Referências não atribuídas");
        return;
    }

    enemyGenerator.GenerateEnemyTeam();

    shopCanvas.SetActive(false);
    battleCanvas.SetActive(true);

    SetupBattleSlots();

    StopAllCoroutines();
    StartCoroutine(BattleFlow());
}


    // =========================
    // SETUP UI
    // =========================
    void SetupBattleSlots()
    {
        List<PetRuntime> playerPets = playerTeam.GetLivingPets();
        List<PetRuntime> enemyPets = enemyTeam.GetLivingPets();



        for (int i = 0; i < playerBattleSlots.Count; i++)
        {
            PetRuntime pet = i < playerPets.Count ? playerPets[i] : null;
            playerBattleSlots[i].isEnemySlot = false;
            playerBattleSlots[i].SetupForBattle(pet);
        }

        for (int i = 0; i < enemyBattleSlots.Count; i++)
        {
            PetRuntime pet = i < enemyPets.Count ? enemyPets[i] : null;
            enemyBattleSlots[i].isEnemySlot = true;
            enemyBattleSlots[i].SetupForBattle(pet);
        }
    }

    // =========================
    // BATALHA
    // =========================
    IEnumerator BattleRoutine()
    {
        List<PetRuntime> teamA = playerTeam.GetLivingPets();
        List<PetRuntime> teamB = enemyTeam.GetLivingPets();


        while (teamA.Count > 0 && teamB.Count > 0)
        {
            PetRuntime a = teamA[0];
            PetRuntime b = teamB[0];

            // 1️⃣ AVANÇO PARA O CHOQUE
if (playerBattleSlots.Count > 0 && playerBattleSlots[0] != null)
    playerBattleSlots[0].PlayAttackMove();

if (enemyBattleSlots.Count > 0 && enemyBattleSlots[0] != null)
    enemyBattleSlots[0].PlayAttackMove();

// ESPERA O CHOQUE
yield return new WaitForSeconds(impactDelay);

// SHAKE DE IMPACTO
if (playerBattleSlots.Count > 0 && playerBattleSlots[0] != null)
    StartCoroutine(playerBattleSlots[0].PlayImpactShake());

if (enemyBattleSlots.Count > 0 && enemyBattleSlots[0] != null)
    StartCoroutine(enemyBattleSlots[0].PlayImpactShake());

// 2️⃣ APLICA DANO
b.health -= a.attack;
a.health -= b.attack;

// 3️⃣ FLASH DE DANO
if (playerBattleSlots.Count > 0 && playerBattleSlots[0] != null)
    playerBattleSlots[0].PlayFlash();

if (enemyBattleSlots.Count > 0 && enemyBattleSlots[0] != null)
    enemyBattleSlots[0].PlayFlash();

            // 🔧 FLASH SEM BLOQUEAR
            if (playerBattleSlots.Count > 0 && playerBattleSlots[0] != null)
                playerBattleSlots[0].PlayFlash();

            if (enemyBattleSlots.Count > 0 && enemyBattleSlots[0] != null)
                enemyBattleSlots[0].PlayFlash();

            yield return new WaitForSeconds(impactDelay);

            UpdateBattleUI(teamA, teamB);

            bool aDied = a.health <= 0;
            bool bDied = b.health <= 0;

            if (aDied || bDied)
                yield return new WaitForSeconds(deathDelay);

            // MORTE COM QUEDA
            if (aDied && playerBattleSlots.Count > 0 && playerBattleSlots[0] != null)
            {
                 yield return StartCoroutine(playerBattleSlots[0].PlayDeathFall());
                teamA.RemoveAt(0);
            }

            if (bDied && enemyBattleSlots.Count > 0 && enemyBattleSlots[0] != null)
            {
                yield return StartCoroutine(enemyBattleSlots[0].PlayDeathFall());
                teamB.RemoveAt(0);
            }


            UpdateBattleUI(teamA, teamB);

            yield return new WaitForSeconds(attackDelay);
        }

        EndBattle(teamA.Count, teamB.Count);
    }

    // =========================
    // UI
    // =========================
    void UpdateBattleUI(List<PetRuntime> player, List<PetRuntime> enemy)
    {
        for (int i = 0; i < playerBattleSlots.Count; i++)
        {
            PetRuntime pet = i < player.Count ? player[i] : null;
            playerBattleSlots[i].SetupForBattle(pet);
        }

        for (int i = 0; i < enemyBattleSlots.Count; i++)
        {
            PetRuntime pet = i < enemy.Count ? enemy[i] : null;
            enemyBattleSlots[i].SetupForBattle(pet);
        }
    }

    // =========================
    // RESULTADO
    // =========================
    void EndBattle(int teamACount, int teamBCount)
{
    // ======================
    // RESULTADO DA BATALHA
    // ======================
    if (teamACount > 0 && teamBCount == 0)
    {
        playerState.WinBattle();
        Debug.Log("Vitória do jogador");
    }
    else if (teamBCount > 0 && teamACount == 0)
    {
        int dmg = GetDamageForRound();
        playerState.LoseLife(dmg);
        Debug.Log("Derrota do jogador - Dano: " + dmg);
    }
    else
    {
        Debug.Log("Empate");
    }

    // pequeno delay para animações finais
    Invoke(nameof(ReturnToShop), 1.2f);
}


    [System.Obsolete]
void ReturnToShop()
{
    // ======================
    // TROCA DE TELAS
    // ======================
    battleCanvas.SetActive(false);
    shopCanvas.SetActive(true);

    // ======================
    // RESET DE VIDA DOS PETS (OBRIGATÓRIO)
    // ======================
    ResetTeamHealth(playerTeam);

    // ======================
    // AVANÇA ROUND / RESET PLAYER
    // ======================
    playerState.StartNewRound();

    // ======================
    // ATUALIZA LOJA (ESSENCIAL)
    // ======================
    ShopManager shopManager = FindObjectOfType<ShopManager>();
    if (shopManager != null)
    {
        shopManager.GenerateShop();
    }
    else
    {
        Debug.LogWarning("ShopManager não encontrado ao retornar da batalha");
    }

    // ======================
    // ATUALIZA UI DO TIME
    // ======================
    TeamManagerUI teamUI = FindObjectOfType<TeamManagerUI>();
    if (teamUI != null)
    {
        teamUI.RefreshAll();
    }
    else
    {
        Debug.LogWarning("TeamManagerUI não encontrado ao retornar da batalha");
    }
}



    // =========================
    // HELPERS
    // =========================

    int GetDamageForRound()
    {
        int r = playerState.round;
        if (r <= 5) return 1;
        if (r <= 10) return 2;
        return 3;
    }
  IEnumerator ExecutePreCombatAbilitiesRoutine()
{
    // contexto do jogador
    BattleContext playerCtx = new BattleContext
    {
        allyTeam = playerTeam,
        enemyTeam = enemyTeam,
        battleManager = this
    };

    // contexto do inimigo
    BattleContext enemyCtx = new BattleContext
    {
        allyTeam = enemyTeam,
        enemyTeam = playerTeam,
        battleManager = this
    };

    foreach (var slot in playerTeam.slots)
        if (!slot.IsEmpty && slot.pet.ability != null)
            slot.pet.ability.OnPreCombat(playerCtx);

    foreach (var slot in enemyTeam.slots)
        if (!slot.IsEmpty && slot.pet.ability != null)
            slot.pet.ability.OnPreCombat(enemyCtx);

    yield return new WaitForSeconds(0.5f);
}


void ResetTeamHealth(TeamManager team)
{
    foreach (var slot in team.slots)
    {
        if (slot.IsEmpty) continue;

        PetRuntime pet = slot.pet;

        pet.health = pet.maxHealth;

        if (pet.health < 0)
            pet.health = pet.maxHealth;
    }
}
public void PlayArrowEffect(PetRuntime from, PetRuntime to)
{
    TeamSlotUI fromSlot = FindSlotUI(from);
    TeamSlotUI toSlot = FindSlotUI(to);

    if (fromSlot == null || toSlot == null)
        return;

    ArrowProjectile arrow =
        Instantiate(arrowPrefab, battleCanvas.transform);

    arrow.Init(fromSlot.transform.position, toSlot.transform.position);
}
TeamSlotUI FindSlotUI(PetRuntime pet)
{
    foreach (var slot in playerBattleSlots)
        if (slot.CurrentPet == pet)
            return slot;

    foreach (var slot in enemyBattleSlots)
        if (slot.CurrentPet == pet)
            return slot;

    return null;
}
IEnumerator BattleFlow()
{
    // 1️⃣ PRÉ-COMBATE
    yield return StartCoroutine(ExecutePreCombatAbilitiesRoutine());

    // 2️⃣ PEQUENA PAUSA DRAMÁTICA 😈
    yield return new WaitForSeconds(0.3f);

    // 3️⃣ COMBATE NORMAL
    yield return StartCoroutine(BattleRoutine());
}

}
