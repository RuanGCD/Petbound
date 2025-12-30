using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;
    public Text nameText;
    public Button button;

    [Header("Freeze")]
    public Button freezeButton;          // botão de freeze
    public GameObject freezeIcon;         // ícone/overlay visual

    private PetRuntime pet;
    private FoodData food;
    private bool isFrozen;

    // ===== PET =====
    public void SetupPet(PetRuntime pet, Action<PetRuntime> onClick)
    {
        this.pet = pet;
        this.food = null;

        iconImage.sprite = pet.data.icon;
        iconImage.enabled = true;

        nameText.text = pet.data.petName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(pet));

        SetupFreeze();
    }

    // ===== FOOD =====
    public void SetupFood(FoodData food, Action<FoodData> onClick)
    {
        this.food = food;
        this.pet = null;

        iconImage.sprite = food.icon;
        iconImage.enabled = true;

        nameText.text = food.foodName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(food));

        SetupFreeze();
    }

    // ===== FREEZE =====
    private void SetupFreeze()
    {
        if (freezeButton == null)
            return;

        freezeButton.onClick.RemoveAllListeners();
        freezeButton.onClick.AddListener(ToggleFreeze);

        UpdateFreezeVisual();
    }

    private void ToggleFreeze()
    {
        isFrozen = !isFrozen;
        UpdateFreezeVisual();
    }

    private void UpdateFreezeVisual()
    {
        if (freezeIcon != null)
            freezeIcon.SetActive(isFrozen);
    }

    public bool IsFrozen => isFrozen;

    // ===== USADO PELO SHOP MANAGER =====
    public bool ContainsData(object data)
    {
        return ReferenceEquals(data, pet) || ReferenceEquals(data, food);
    }

    public PetRuntime GetPet()
    {
        return pet;
    }

    public FoodData GetFood()
    {
        return food;
    }

    public void Clear()
    {
        pet = null;
        food = null;
        isFrozen = false;

        iconImage.sprite = null;
        iconImage.enabled = false;
        nameText.text = "";

        button.onClick.RemoveAllListeners();

        if (freezeButton != null)
            freezeButton.onClick.RemoveAllListeners();

        if (freezeIcon != null)
            freezeIcon.SetActive(false);
    }
}
