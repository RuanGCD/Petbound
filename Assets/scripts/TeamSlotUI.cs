using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public enum TeamSlotMode
{
    Shop,
    Battle
}

public class TeamSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;
    public GameObject petStatsGroup;
    public Text attackText;
    public Text healthText;
    public GameObject selectionHighlight;

    [Header("Mode")]
    public TeamSlotMode mode = TeamSlotMode.Shop;

    private TeamSlot slot;
    private PetRuntime battlePet;
    private int slotIndex;
    private Action<int> onClickCallback;
    private Button button;
    [Header("Level UI")]
    public GameObject levelGroup;
    public Text levelText;
    public List<Image> xpBars;
    public Color xpOnColor = Color.yellow;
    public Color xpOffColor = Color.gray;


    [Header("Battle Visual")]
    public bool isEnemySlot;
    public float flashDuration = 0.15f;

    [Header("Battle Movement")]
    public float moveDistance = 150f;
    public float moveDuration = 0.12f;
    [Header("Death Animation")]
    public float fallDistance = 120f;
    public float fallDuration = 0.25f;
    [Header("Impact Shake")]
    public float shakeAmount = 20f;
    public float shakeDuration = 0.15f;


    RectTransform rect;
    Vector2 originalPos;
    Coroutine moveRoutine;

    Color originalColor;
    public PetRuntime CurrentPet { get; private set; }


    // 🔧 NOVO (controle de coroutine)
    Coroutine flashRoutine;

    void Awake()
    {
        button = GetComponent<Button>();

        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;

        if (iconImage != null)
            originalColor = iconImage.color;

    }

    // ======================
    // SHOP MODE
    // ======================
    public void Setup(TeamSlot slot, int index, Action<int> onClick)
    {
        mode = TeamSlotMode.Shop;

        this.slot = slot;
        battlePet = null;
        slotIndex = index;
        onClickCallback = onClick;

        if (button != null)
            button.interactable = true;

        Refresh();
    }

    // ======================
    // BATTLE MODE
    // ======================
   public void SetupForBattle(PetRuntime pet)
{
    mode = TeamSlotMode.Battle;

    slot = null;
    onClickCallback = null;

    if (button != null)
        button.interactable = false;

    // 🔥 LIMPEZA REAL
    battlePet = null;
    CurrentPet = null;
    ClearUI();

    // 🔁 AGORA SETA O NOVO PET
    battlePet = pet;
    CurrentPet = pet;

    if (pet == null)
        return;

    Refresh();
}



    // ======================
    // CLICK
    // ======================
   public void OnClick()
{
    Debug.Log("[TeamSlotUI] Clique no slot " + slotIndex);

    if (mode != TeamSlotMode.Shop)
        return;

    if (onClickCallback != null)
    {
        Debug.Log("[TeamSlotUI] Callback chamado");
        onClickCallback.Invoke(slotIndex);
    }
    else
    {
        Debug.LogError("[TeamSlotUI] onClickCallback NULL");
    }
}

    // ======================
    // REFRESH
    // ======================
   public void Refresh()
{
    PetRuntime petToShow = null;

    // ======================
    // DEFINE QUAL PET MOSTRAR
    // ======================
    if (mode == TeamSlotMode.Shop)
    {
        if (slot == null || slot.pet == null)
        {
            ClearUI();
            return;
        }

        petToShow = slot.pet;
    }
    else // Battle
    {
        if (battlePet == null)
        {
            ClearUI();
            return;
        }

        petToShow = battlePet;
        Debug.Log(
    $"[BattleSlotUI] Slot({name}) | battlePet ref = {battlePet?.data.petName} | " +
    $"ATK {battlePet?.TotalAttack} | HP {battlePet?.health}"
);

    }

    // ======================
    // UI BÁSICA
    // ======================
    petStatsGroup.SetActive(true);

    iconImage.enabled = true;
    iconImage.sprite = petToShow.data.icon;

    if (mode == TeamSlotMode.Battle){
        attackText.text = petToShow.TotalAttack.ToString();
    }else{
        attackText.text = petToShow.attack.ToString();
    }
    healthText.text = petToShow.health.ToString();

    // ======================
    // LEVEL / XP UI
    // ======================
    if (mode == TeamSlotMode.Battle)
    {
        //  batalha não mostra level
        if (levelGroup != null)
            levelGroup.SetActive(false);
    }
    else
    {
        //  loja mostra level
        if (levelGroup != null)
            levelGroup.SetActive(true);

        UpdateLevelUI(petToShow);
    }

    // ======================
    // VISUAL LADO (INIMIGO)
    // ======================
    ApplySideVisual();

    // ======================
    // RESET VISUAL
    // ======================
    iconImage.color = originalColor;
    SetSelected(false);
}



    // ======================
    // INVERTE SPRITE INIMIGO
    // ======================
    void ApplySideVisual()
    {
        if (iconImage == null)
            return;

        Vector3 scale = iconImage.rectTransform.localScale;
        scale.x = isEnemySlot ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        iconImage.rectTransform.localScale = scale;
    }

    // ======================
    // FLASH DAMAGE (ORIGINAL)
    // ======================
    public IEnumerator FlashDamage()
    {
        if (iconImage == null)
            yield break;

        iconImage.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        iconImage.color = originalColor;
    }

    public void PlayFlash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashDamage());
    }

    public void SetSelected(bool value)
    {
        if (mode != TeamSlotMode.Shop)
            value = false;

        if (selectionHighlight != null)
            selectionHighlight.SetActive(value);
    }

    // ======================
    // CLEAR
    // ======================
    void ClearUI()
    {
        if (petStatsGroup != null)
            petStatsGroup.SetActive(false);

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            iconImage.color = originalColor;
        }
        if (levelGroup != null)
        levelGroup.SetActive(false);

        SetSelected(false);
    }
    public void PlayAttackMove()
{
    if (moveRoutine != null)
        StopCoroutine(moveRoutine);

    moveRoutine = StartCoroutine(AttackMoveRoutine());
}
// animação de se chocar no meio
IEnumerator AttackMoveRoutine()
{
    float dir = isEnemySlot ? -1f : 1f;
    Vector2 target = originalPos + Vector2.right * moveDistance * dir;

    // AVANÇA
    yield return Move(rect.anchoredPosition, target);

    // VOLTA
    yield return Move(rect.anchoredPosition, originalPos);
}

IEnumerator Move(Vector2 from, Vector2 to)
{
    float t = 0f;

    while (t < 1f)
    {
        t += Time.deltaTime / moveDuration;
        rect.anchoredPosition = Vector2.Lerp(from, to, t);
        yield return null;
    }

    rect.anchoredPosition = to;
}
public IEnumerator PlayDeathFall()
{
    if (rect == null)
        yield break;

    Vector2 start = rect.anchoredPosition;
    Vector2 end = start + Vector2.down * fallDistance;

    float t = 0f;

    while (t < 1f)
    {
        t += Time.deltaTime / fallDuration;
        rect.anchoredPosition = Vector2.Lerp(start, end, t);

        if (iconImage != null)
            iconImage.color = Color.Lerp(originalColor, Color.clear, t);

        yield return null;
    }

    ClearUI();

    // reseta posição pra reutilização
    rect.anchoredPosition = originalPos;
    if (iconImage != null)
        iconImage.color = originalColor;
}
public IEnumerator PlayImpactShake()
{
    if (rect == null)
        yield break;

    float elapsed = 0f;

    while (elapsed < shakeDuration)
    {
        elapsed += Time.deltaTime;

        float offset = Mathf.Sin(elapsed * 80f) * shakeAmount;
        rect.anchoredPosition = originalPos + Vector2.right * offset;

        yield return null;
    }

    rect.anchoredPosition = originalPos;
}
void UpdateLevelUI(PetRuntime pet)
{
    if (levelText != null)
        levelText.text = pet.level.ToString();

    int maxXp = pet.GetXpToLevel();

    for (int i = 0; i < xpBars.Count; i++)
    {
        // desativa barras extras
        if (i >= maxXp || pet.level >= 3)
        {
            xpBars[i].gameObject.SetActive(false);
            continue;
        }

        xpBars[i].gameObject.SetActive(true);

        if (i < pet.xp)
            xpBars[i].color = xpOnColor;
        else
            xpBars[i].color = xpOffColor;
    }
}
}
