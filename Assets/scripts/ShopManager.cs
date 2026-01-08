using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public Transform shopPanel;
    public ShopSlotUI slotPrefab;

    [Header("Data")]
    public List<PetData> availablePets;
    public List<FoodData> availableFoods;

    [Header("Team")]
    public TeamManager teamManager;
    public TeamManagerUI teamManagerUI;

    [Header("Player State")]
    public PlayerState playerState;
    private FoodData selectedFood;
    private ShopSlotUI selectedFoodSlot;


    [Header("Shop Config")]
    public int petsInShop = 3;
    public int foodsInShop = 2;

    private List<ShopSlotUI> currentSlots = new();

    // 🔹 NOVO: pet selecionado da loja
    private PetRuntime selectedPet;
    private ShopSlotUI selectedSlotUI;
    //habilidades
    private bool firstPurchaseDoneThisRound = false;


    void Start()
    {
        if (playerState == null)
        {
            Debug.LogError("[ShopManager] PlayerState não atribuído.");
            return;
        }

        GenerateShop();
    }

    // =========================
    // SHOP
    // =========================
    public void GenerateShop()
    {
        List<ShopSlotUI> frozenSlots = new();

        foreach (var slot in currentSlots)
        {
            if (slot != null && slot.IsFrozen)
                frozenSlots.Add(slot);
            else if (slot != null)
                Destroy(slot.gameObject);
        }

        currentSlots.Clear();

        foreach (var frozen in frozenSlots)
        {
            frozen.transform.SetParent(shopPanel);
            currentSlots.Add(frozen);
        }

        int petsToGenerate = petsInShop - CountFrozenPets();
        int foodsToGenerate = foodsInShop - CountFrozenFoods();

        for (int i = 0; i < petsToGenerate; i++)
        {
            PetData petData = GetRandomPetForRound();
            if (petData == null) continue;

            PetRuntime runtime = new PetRuntime(petData);
            ShopSlotUI slot = Instantiate(slotPrefab, shopPanel);
            slot.SetupPet(runtime, OnPetClicked);
            currentSlots.Add(slot);
        }

        for (int i = 0; i < foodsToGenerate; i++)
        {
            FoodData foodData = GetRandomFoodForRound();
            if (foodData == null) continue;

            ShopSlotUI slot = Instantiate(slotPrefab, shopPanel);
            slot.SetupFood(foodData, OnFoodClicked);
            currentSlots.Add(slot);
        }
    }

    // =========================
    // PET CLICK (SELEÇÃO)
    // =========================
    private void OnPetClicked(PetRuntime pet)
    {
        selectedPet = pet;
        selectedSlotUI = currentSlots.Find(s => s.GetPet() == pet);

        Debug.Log("Pet selecionado: " + pet.data.petName);
    }

    // =========================
    // SLOT DO TIME RECEBE PET
    // =========================
    public void TryPlaceSelectedPet(int slotIndex)
{
    if (selectedPet == null)
        return;

    if (!playerState.SpendGold(playerState.petCost))
        return;

    bool slotWasEmpty = teamManager.slots[slotIndex].IsEmpty;

    bool success = teamManager.TryPlacePetAtSlot(selectedPet, slotIndex);

    if (!success)
    {
        playerState.gold += playerState.petCost;
        return;
    }

    // 🛒 MINGAL: somente compra normal (slot vazio)
    if (slotWasEmpty)
    {
        foreach (var pet in teamManager.GetAllPets())
        {
            if (pet.ability is MingalAllyOnEnterAbility mingal)
            {
                mingal.OnShopAllyEntered(selectedPet);
            }
        }
    }

    TriggerFirstPurchaseAbilities();
    teamManagerUI.RefreshAll();
    RemoveSlot(selectedPet);

    selectedPet = null;
    selectedSlotUI = null;
}

    // =========================
    // FOOD
    // =========================
    private void OnFoodClicked(FoodData food)
{
    selectedFood = food;
    selectedFoodSlot = currentSlots.Find(s => s.GetFood() == food);

    Debug.Log("Food selecionada: " + food.foodName);
}
    // =========================
    // REROLL
    // =========================
    public void Reroll()
{
    bool freeRerollUsed = false;

    foreach (var pet in teamManager.GetAllPets())
    {
        if (pet.ability != null && pet.ability.TryFreeReroll())
        {
            freeRerollUsed = true;
            pet.ability.OnFreeRerollUsed();
            break; // só 1 habilidade pode aplicar
        }
    }

    if (!freeRerollUsed)
    {
        if (!playerState.SpendGold(playerState.rerollCost))
            return;
    }

    GenerateShop();
}
    // =========================
    // HELPERS
    // =========================
    private void RemoveSlot(object data)
    {
        for (int i = currentSlots.Count - 1; i >= 0; i--)
        {
            if (currentSlots[i].ContainsData(data))
            {
                Destroy(currentSlots[i].gameObject);
                currentSlots.RemoveAt(i);
                break;
            }
        }
    }

    private int CountFrozenPets()
    {
        int count = 0;
        foreach (var slot in currentSlots)
            if (slot.IsFrozen && slot.GetPet() != null)
                count++;
        return count;
    }

    private int CountFrozenFoods()
    {
        int count = 0;
        foreach (var slot in currentSlots)
            if (slot.IsFrozen && slot.GetFood() != null)
                count++;
        return count;
    }

    private PetData GetRandomPetForRound()
    {
        List<PetData> validPets =
            availablePets.FindAll(p => p.tier <= playerState.round);

        if (validPets.Count == 0)
            return null;

        return validPets[Random.Range(0, validPets.Count)];
    }

    private FoodData GetRandomFoodForRound()
    {
        List<FoodData> validFoods =
            availableFoods.FindAll(f => f.tier <= playerState.round);

        if (validFoods.Count == 0)
            return null;

        return validFoods[Random.Range(0, validFoods.Count)];
    }
    public bool HasSelectedPet()
{
    return selectedPet != null;
}
public void OnReturnFromBattle()
{
     firstPurchaseDoneThisRound = false;
    // limpa seleção
    selectedPet = null;
    selectedSlotUI = null;

     //  INÍCIO DA RODADA
    foreach (var pet in teamManager.GetAllPets())
    {
        if (pet.ability is LiahStartRoundAbility liah)
            liah.Activate(teamManager);
        else
            pet.ability?.OnStartRound();
    }

    // gera loja respeitando freeze
    GenerateShop();
}
private void TriggerFirstPurchaseAbilities()
{
    if (firstPurchaseDoneThisRound)
        return;

    firstPurchaseDoneThisRound = true;

    foreach (var pet in teamManager.GetAllPets())
    {
        pet.ability?.OnFirstShopPurchase();
    }
}
public bool HasSelectedFood()
{
    return selectedFood != null;
}
public void TryUseFoodOnPet(PetRuntime pet)
{
    if (!playerState.SpendGold(playerState.foodCost))
        return;

    ApplyFoodEffect(pet, selectedFood);

    RemoveSlot(selectedFood);

    selectedFood = null;
    selectedFoodSlot = null;
}
private void ApplyFoodEffect(PetRuntime pet, FoodData food)
{
    switch (food.type)
    {
        case FoodType.PermanentStat:
            pet.attack += food.attackBonus;
            pet.health += food.healthBonus;
            pet.maxHealth += food.healthBonus;
            break;

        case FoodType.TemporaryStat:
            pet.tempAttackBonus += food.attackBonus;
            break;

        case FoodType.XP:
            pet.xp++;
            pet.TryLevelUp();
            break;

        case FoodType.Effect:
            pet.AddFoodEffect(food);
            break;
    }
    // 🔥 DISPARA HABILIDADE AO CONSUMIR COMIDA
    pet.ability?.OnConsumeFood(food);
    Debug.Log($"🍎 {food.foodName} usada em {pet.data.petName}");
}


}
