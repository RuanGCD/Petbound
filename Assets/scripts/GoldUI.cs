using UnityEngine;
using UnityEngine.UI;

public class GoldUI : MonoBehaviour
{
    public PlayerState playerState;
    public Text goldText;

    void Update()
    {
        goldText.text = playerState.gold.ToString();

    }
}
