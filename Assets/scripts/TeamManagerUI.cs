using UnityEngine;
using System.Collections.Generic;

public class TeamManagerUI : MonoBehaviour
{
    public TeamManager teamManager;
    public List<TeamSlotUI> slotUIs;
    public PlayerState playerState;
    public int sellGold = 1;

    private int selectedIndex = -1;

    void Start()
    {
        if (teamManager == null)
        {
            Debug.LogError("[TeamManagerUI] TeamManager não atribuído!");
            return;
        }

        for (int i = 0; i < slotUIs.Count; i++)
        {
            int index = i;
            slotUIs[i].Setup(teamManager.slots[i], index, OnSlotClicked);
        }

        RefreshAll();
    }

    //  Clique em slot
    void OnSlotClicked(int index)
{
    ShopManager shop = FindObjectOfType<ShopManager>();

    //  USAR COMIDA
if (shop != null && shop.HasSelectedFood())
{
    PetRuntime pet = teamManager.slots[index].pet;
    if (pet == null)
        return;

    shop.TryUseFoodOnPet(pet);
    RefreshAll();
    return;
}

    //  PRIORIDADE: compra da loja
    if (shop != null && shop.HasSelectedPet())
    {
        shop.TryPlaceSelectedPet(index);
        return;
    }

    //  Seleção inicial
    if (selectedIndex == -1)
    {
        if (teamManager.slots[index].IsEmpty)
            return;

        selectedIndex = index;
        slotUIs[index].SetSelected(true);
        return;
    }

    //  Clique em outro slot
    if (selectedIndex != index)
    {
        TeamSlot fromSlot = teamManager.slots[selectedIndex];
        TeamSlot toSlot   = teamManager.slots[index];

        //  TENTAR MERGE
        if (!fromSlot.IsEmpty && !toSlot.IsEmpty)
        {
            PetRuntime incoming = fromSlot.pet;

            bool merged = teamManager.TryPlacePetAtSlot(incoming, index);

            if (merged)
            {
                // remove o pet de origem
                fromSlot.pet = null;

                ClearSelection();
                RefreshAll();
                return;
            }
        }

        //  se não deu merge → SWAP normal
        teamManager.SwapPets(selectedIndex, index);
    }

    ClearSelection();
    RefreshAll();
}



    void ClearSelection()
    {
        if (selectedIndex >= 0 && selectedIndex < slotUIs.Count)
            slotUIs[selectedIndex].SetSelected(false);

        selectedIndex = -1;
    }

    public void RefreshAll()
    {
        foreach (var slot in slotUIs)
        {
            slot.Refresh();
            slot.SetSelected(false);
        }
    }
    public void SellSelectedPet()
{
    if (selectedIndex == -1)
        return;

    if (teamManager.slots[selectedIndex].IsEmpty)
        return;

    teamManager.RemovePetAt(selectedIndex);
    playerState.gold += sellGold;

    ClearSelection();
    RefreshAll();
}


}
