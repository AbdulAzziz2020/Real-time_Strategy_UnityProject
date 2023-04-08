using System;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI unitCountText;
    public TextMeshProUGUI foodText;

    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateUnitCountText(int value)
    {
        unitCountText.text = value.ToString();
    }

    public void UpdateFoodText(int value)
    {
        foodText.text = value.ToString();
    }
}