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

    // 🔹 MERGE
    existing.attack += incoming.attack;
    existing.health += incoming.health;
    existing.maxHealth += incoming.maxHealth;
    existing.xp++;

    bool leveledUp = existing.TryLevelUp();

    if (leveledUp)
    {
        Debug.Log(existing.data.petName + " subiu para o Level " + existing.level);
        // futuramente: AbilitySystem.OnLevelUp(existing)
    }

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
