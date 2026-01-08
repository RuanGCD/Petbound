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
    public PetData calopsitaData;
    private List<PetRuntime> battleTeamA;
    private List<PetRuntime> battleTeamB;


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

    // cria snapshot apenas para o combate
    battleTeamA = new List<PetRuntime>(playerTeam.GetLivingPets());
    battleTeamB = new List<PetRuntime>(enemyTeam.GetLivingPets());

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
    List<PetRuntime> teamA = battleTeamA;
    List<PetRuntime> teamB = battleTeamB;

    while (teamA.Count > 0 && teamB.Count > 0)
    {
        PetRuntime a = teamA[0];
        PetRuntime b = teamB[0];

        // ======================
        // 1️⃣ AVANÇO PARA O CHOQUE
        // ======================
        if (playerBattleSlots.Count > 0 && playerBattleSlots[0] != null)
            playerBattleSlots[0].PlayAttackMove();

        if (enemyBattleSlots.Count > 0 && enemyBattleSlots[0] != null)
            enemyBattleSlots[0].PlayAttackMove();

        yield return new WaitForSeconds(impactDelay);

        // ======================
        // SHAKE DE IMPACTO
        // ======================
        if (playerBattleSlots.Count > 0 && playerBattleSlots[0] != null)
            StartCoroutine(playerBattleSlots[0].PlayImpactShake());

        if (enemyBattleSlots.Count > 0 && enemyBattleSlots[0] != null)
            StartCoroutine(enemyBattleSlots[0].PlayImpactShake());

        // ======================
        // 2️⃣ EVENTO DE ATAQUE
        // ======================
        if (a.ability != null)
            a.ability.OnAttack(CreatePlayerContext(), b);

        if (b.ability != null)
            b.ability.OnAttack(CreateEnemyContext(), a);

        // ======================
        // 3️⃣ APLICA DANO
        // ======================
        b.health -= a.TotalAttack;
        a.health -= b.TotalAttack;

        if (a.health < 0) a.health = 0;
        if (b.health < 0) b.health = 0;

        // ======================
        // 4️⃣ FLASH DE DANO
        // ======================
        if (playerBattleSlots.Count > 0 && playerBattleSlots[0] != null)
            playerBattleSlots[0].PlayFlash();

        if (enemyBattleSlots.Count > 0 && enemyBattleSlots[0] != null)
            enemyBattleSlots[0].PlayFlash();

        yield return new WaitForSeconds(impactDelay);

        UpdateBattleUI(teamA, teamB);

        bool aDied = a.health <= 0;
        bool bDied = b.health <= 0;

        // 🔒 GUARDA SE DEVE ATIVAR ON DEATH
        bool triggerAOnDeath = aDied && !a.isDead && a.ability != null;
        bool triggerBOnDeath = bDied && !b.isDead && b.ability != null;

        if (aDied || bDied)
            yield return new WaitForSeconds(deathDelay);

        // ======================
        // REMOÇÃO + ANIMAÇÃO
        // ======================
        if (aDied && playerBattleSlots.Count > 0 && playerBattleSlots[0] != null)
        {
            a.isDead = true;

            yield return StartCoroutine(playerBattleSlots[0].PlayDeathFall());
            teamA.RemoveAt(0);
        }

        if (bDied && enemyBattleSlots.Count > 0 && enemyBattleSlots[0] != null)
        {
            b.isDead = true;

            yield return StartCoroutine(enemyBattleSlots[0].PlayDeathFall());
            teamB.RemoveAt(0);
        }

        // ======================
        //  ON DEATH (AGORA COM SLOT LIVRE)
        // ======================
        if (triggerAOnDeath)
            a.ability.OnDeath(CreatePlayerContext());

        if (triggerBOnDeath)
            b.ability.OnDeath(CreateEnemyContext());

        // ======================
        // ATUALIZA UI FINAL
        // ======================
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
    // REMOVE PETS INVOCADOS (CALOPSITAS)
    // ======================
    RemoveSummonedPets(playerTeam);
    RemoveSummonedPets(enemyTeam);

    // ======================
    // RESET DE VIDA
    // ======================
    ResetTeamHealth(playerTeam);
    ResetTeamHealth(enemyTeam);

    // ======================
    // RESET DE STATS TEMPORÁRIOS
    // ======================
    ResetTeamTemporaryStats(playerTeam);
    ResetTeamTemporaryStats(enemyTeam);

    // ======================
    // AVANÇA ROUND
    // ======================
    playerState.StartNewRound();

    // ======================
    // ATUALIZA LOJA
    // ======================
    ShopManager shopManager = FindObjectOfType<ShopManager>();
    if (shopManager != null)
        shopManager.OnReturnFromBattle();


    // ======================
    // ATUALIZA UI
    // ======================
    TeamManagerUI teamUI = FindObjectOfType<TeamManagerUI>();
    if (teamUI != null)
        teamUI.RefreshAll();
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
    // 🔁 RESET DE ESTADO DE BATALHA
    ResetBattleAbilities();

    // PRÉ-COMBATE
    yield return StartCoroutine(ExecutePreCombatAbilitiesRoutine());

    yield return new WaitForSeconds(0.3f);

    // COMBATE
    yield return StartCoroutine(BattleRoutine());
}


BattleContext CreatePlayerContext()
{
    return new BattleContext
    {
        allyTeam = playerTeam,
        enemyTeam = enemyTeam,
        battleManager = this,
        isPlayer = true
    };
}

BattleContext CreateEnemyContext()
{
    return new BattleContext
    {
        allyTeam = enemyTeam,
        enemyTeam = playerTeam,
        battleManager = this,
        isPlayer = false
    };
}

void ResetTeamTemporaryStats(TeamManager team)
{
    foreach (var slot in team.slots)
    {
        if (slot.IsEmpty) continue;

        slot.pet.ResetTemporaryStats();
    }
}
public void SpawnCalopsitasAtFront(int amount, bool isPlayer)
{
    List<PetRuntime> battleTeam = isPlayer ? battleTeamA : battleTeamB;
    TeamManager teamManager = isPlayer ? playerTeam : enemyTeam;

    for (int i = 0; i < amount; i++)
    {
        if (battleTeam.Count >= 5)
            break;

        PetRuntime calopsita = CreateCalopsitaRuntime();

        //  entra na frente
        battleTeam.Insert(0, calopsita);

        //  MINGAL: aliado entrou por INVOCAÇÃO
        TriggerMingalOnInvoke(teamManager, calopsita);
    }
}



public PetRuntime CreateCalopsitaRuntime()
{
    PetRuntime pet = new PetRuntime(calopsitaData);
    
    // 🔒 TRAVA ABSOLUTA DE STATUS
    pet.attack = 1;
    pet.health = 1;
    pet.maxHealth = 1;

    // 🔒 NÃO HERDA NADA
    pet.tempAttackBonus = 0;
    pet.level = 1;
    pet.xp = 0;
    pet.ability = null;

    pet.isTemporarySummon = true;

    return pet;
}

void RemoveSummonedPets(TeamManager team)
{
    for (int i = 0; i < team.slots.Count; i++)
    {
        if (team.slots[i].IsEmpty)
            continue;

        PetRuntime pet = team.slots[i].pet;

        if (pet.isTemporarySummon)
        {
            team.slots[i].pet = null;
        }
    }
}
void ResetBattleAbilities()
{
    ResetBattleTeam(battleTeamA);
    ResetBattleTeam(battleTeamB);
}

void ResetBattleTeam(List<PetRuntime> team)
{
    foreach (var pet in team)
    {
        if (pet?.ability is CalopgangueOnDeathAbility calop)
        {
            calop.ResetForBattle();
        }

        pet.isDead = false; 
    }
}
void TriggerMingalOnInvoke(TeamManager team, PetRuntime summoned)
{
    foreach (var pet in team.GetLivingPets())
    {
        if (pet.ability is MingalAllyOnEnterAbility mingal)
        {
            mingal.OnBattleAllyInvoked(summoned);
        }
    }
}



}
