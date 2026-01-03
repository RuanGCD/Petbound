using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public int maxSlots = 5;
    public List<TeamSlot> slots;

    void Awake()
    {
        slots = new List<TeamSlot>();

        for (int i = 0; i < maxSlots; i++)
            slots.Add(new TeamSlot());
    }

    //colocar pet direto no slot (com merge)
    public bool TryPlacePetAtSlot(PetRuntime incoming, int index)
{
    if (index < 0 || index >= slots.Count)
        return false;

    TeamSlot slot = slots[index];

    // slot vazio
    if (slot.IsEmpty)
    {
        slot.pet = incoming;
        return true;
    }

    PetRuntime existing = slot.pet;

    // pets diferentes não fazem merge
    if (existing.data != incoming.data)
        return false;

    // já no level máximo
    if (!existing.CanGainXp())
        return false;

    // =========================
    // 🔁 MERGE CORRETO
    // =========================

    // 1️⃣ mantém o MAIOR ataque e vida
    existing.attack = Mathf.Max(existing.attack, incoming.attack);
    existing.health = Mathf.Max(existing.health, incoming.health);
    existing.maxHealth = Mathf.Max(existing.maxHealth, incoming.maxHealth);

    // 2️⃣ bônus fixo de merge
    int bonusAtk = 1;
    int bonusHp = 1;

    // se esse merge causar level up, o bônus muda
    int previousLevel = existing.level;

    existing.xp++;
    bool leveledUp = existing.TryLevelUp();

    if (leveledUp && previousLevel == 2)
    {
        // Lv2 → Lv3
        bonusAtk = 2;
        bonusHp = 2;
    }

    existing.attack += bonusAtk;
    existing.health += bonusHp;
    existing.maxHealth += bonusHp;

    Debug.Log(
        $"[MERGE] {existing.data.petName} Lv {existing.level} | +{bonusAtk} ATK / +{bonusHp} HP"
    );

    return true;
}



    // 🔁 SWAP (inalterado)
    public void SwapPets(int indexA, int indexB)
    {
        if (indexA == indexB) return;
        if (indexA < 0 || indexA >= slots.Count) return;
        if (indexB < 0 || indexB >= slots.Count) return;

        PetRuntime temp = slots[indexA].pet;
        slots[indexA].pet = slots[indexB].pet;
        slots[indexB].pet = temp;
    }
    // venda
    public void RemovePetAt(int index)
{
    if (index < 0 || index >= slots.Count)
        return;

    slots[index].pet = null;
}
public List<PetRuntime> GetLivingPets()
{
    List<PetRuntime> list = new();

    foreach (var slot in slots)
    {
        if (!slot.IsEmpty && slot.pet != null && slot.pet.health > 0)
        {
            list.Add(slot.pet);
        }
    }

    return list;
}
public List<TeamSlot> GetLivingSlots()
{
    List<TeamSlot> list = new();
    foreach (var slot in slots)
        if (!slot.IsEmpty)
            list.Add(slot);
    return list;
}

public void KillPet(PetRuntime pet)
{
    foreach (var slot in slots)
    {
        if (!slot.IsEmpty && slot.pet == pet)
        {
            slot.pet = null;
            return;
        }
    }
}


}
