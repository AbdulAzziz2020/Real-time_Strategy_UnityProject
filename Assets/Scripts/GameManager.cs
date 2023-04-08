using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Player[] players;
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        players = FindObjectsOfType<Player>();
    }

    // returns a random enemy player 
    public Player GetRandomPlayer(Player me)
    {
        Player ranPlayer = players[Random.Range(0, players.Length)];

        while (ranPlayer == me)
        {
            ranPlayer = players[Random.Range(0, players.Length)];
        }

        return ranPlayer;
    }
    
    // called when a unit dies, check to see if there's one remaining player
    public void UnitDeathCheck()
    {
        int remainingPlayers = 0;
        Player winner = null;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].units.Count > 0)
            {
                remainingPlayers++;
                winner = players[i];
            }
        }

        // if there is more than 1 remaining player, return
        if (remainingPlayers != 1) return;
        
        EndGameUI.instance.SetEndScreen(winner.isMe);
    }
    
}
