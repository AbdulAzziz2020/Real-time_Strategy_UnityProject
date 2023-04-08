using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    public GameObject endScreen;
    public TextMeshProUGUI statusText;

    public static EndGameUI instance;

    private const string winText = "You Win!";
    private const string loseText = "You Lose!";

    private void Awake()
    {
        instance = this;
    }

    public void SetEndScreen(bool win)
    {
        endScreen.SetActive(true);

        statusText.text = win ? winText : loseText;
        statusText.color = win ? Color.green : Color.red;
    }

    public void OnPlayAgainButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
