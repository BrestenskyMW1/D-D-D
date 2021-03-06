﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ProgressDiamondScript : NetworkBehaviour {
    public SyncListFloat progresses = new SyncListFloat();

    public Image player1, player2, player3, player4;

    // Use this for initialization
    void Start() {
        
        if (isServer)
        {    
            progresses.Add(0f);
            progresses.Add(0f);
            progresses.Add(0f);
            progresses.Add(0f);
        }     
	}

// Update is called once per frame
void Update () {
		
	}

    public void ChangeProgress(int playerIndex, float progress)
    {
        if (isServer)
        {
            progresses[playerIndex] += progress;
            if (playerIndex == 0)
            {
                player1.fillAmount = progresses[playerIndex];
            }
            else if (playerIndex == 1)
            {
                player2.fillAmount = progresses[playerIndex];
            }
            else if (playerIndex == 2)
            {
                player3.fillAmount = progresses[playerIndex];
            }
            else if (playerIndex == 3)
            {
                player4.fillAmount = progresses[playerIndex];
            }
        } 
    }
}
