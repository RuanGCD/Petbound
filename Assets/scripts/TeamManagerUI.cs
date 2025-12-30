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

    // 🔹 Clique em slot
    void OnSlotClicked(int index)
{
    ShopManager shop = FindObjectOfType<ShopManager>();

    // 🟩 PRIORIDADE: se existe pet selecionado na loja → tentar comprar
    if (shop != null && shop.HasSelectedPet())
    {
        shop.TryPlaceSelectedPet(index);
        return;
    }

    // 🔵 comportamento antigo (seleção / swap)
    if (selectedIndex == -1)
    {
        if (teamManager.slots[index].IsEmpty)
            return;

        selectedIndex = index;
        slotUIs[index].SetSelected(true);
        return;
    }

    if (selectedIndex != index)
    {
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
