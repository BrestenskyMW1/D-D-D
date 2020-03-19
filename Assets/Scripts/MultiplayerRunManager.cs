﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MultiplayerRunManager : NetworkBehaviour {
    const int ROOM_COUNT = 3;

    public List<GameObject> possibleRoomList;
    public GameObject bossRoom;
    public GameObject boss;
    public GameObject player1, player2, player3, player4;
    bool bossSpawned;
    public Transform zero;
    public Transform p1Space;
    public Transform p2Space;
    public Transform p3Space;
    public Transform p4Space;
    public List<Vector3> playerSpawns;
    System.Random rnd;
    List<GameObject> runOrder;
    int p1CurRoom = 0;
    int p2CurRoom = 0;
    int p3CurRoom = 0;
    int p4CurRoom = 0;
    GameObject p1r1;
    GameObject p2r1;
    GameObject p3r1;
    GameObject p4r1;
    public Camera p1camera, p2camera, p3camera, p4camera, main;
    public Canvas progressCanvas;
    public Image sideToSide;
    public Image topToBottom;
    public Image player1progress, player2progress, player3progress, player4progress;
    bool gameStarted;

    public int numPlayers = 0;

    public ProgressDiamondScript pDiamondScript;

    public SyncListFloat progresses; 

    //stuff for Singleton
    private static MultiplayerRunManager _instance = null;
    public static MultiplayerRunManager Instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }
    // Use this for initialization
    void Start () {
        rnd = new System.Random();
        runOrder = new List<GameObject>();
        for(int rc = 0; rc < ROOM_COUNT; rc++)
        {
            int traps = rnd.Next(3,6);
            GameObject startRoom = possibleRoomList[rnd.Next(0, possibleRoomList.Count)];
            startRoom.transform.GetChild(traps).gameObject.SetActive(true);
            runOrder.Add(startRoom);
        }
        
        p1r1 = Instantiate(runOrder[0], p1Space);
        p2r1 = Instantiate(runOrder[0], p2Space);
        p3r1 = Instantiate(runOrder[0], p3Space);
        p4r1 = Instantiate(runOrder[0], p4Space);

        bossSpawned = false;
        gameStarted = false;

        //Unfill all progress
        player1progress.fillAmount = 0;
        player2progress.fillAmount = 0;
        player3progress.fillAmount = 0;
        player4progress.fillAmount = 0;

        //Initalize Cameras
        main.rect = new Rect(0, 0, 1, 1);

        //Deinitialize progress diamond
        progressCanvas.gameObject.SetActive(false);

        pDiamondScript = FindObjectOfType<ProgressDiamondScript>();
        progresses = pDiamondScript.progresses;
        

    }
	
	// Update is called once per frame
	void Update () {
        if (PlayerInRoom() && bossSpawned)
        {
            Instantiate(boss, zero);
            bossSpawned = true;
            //TODO: enable boss ui and such
        }
        if (!gameStarted && isServer && Input.GetKeyDown(KeyCode.R))
            StartGame();
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////
    /// </summary>
    public void StartGame()
    {
        updateCamera();
        gameStarted = true;
        //TODO: teleport players to correct spots
    }


    public void NextRoom(GameObject player)
    {
        player.GetComponent<CollisionHandler>().ToggleInteractivity(false);
        int playerNum = 0;
        switch (playerNum)
        {
            case 1:
                Destroy(p1r1);
                p1CurRoom++;
                if (p1CurRoom >= runOrder.Count)
                {
                    //player reaches boss room
                }
                else
                {
                    p1r1 = Instantiate(runOrder[p1CurRoom], p1Space);
                    player.transform.position = playerSpawns[0];
                }
                break;
            case 2:
                Destroy(p2r1);
                p2CurRoom++;
                if (p2CurRoom >= runOrder.Count)
                {
                    //player reaches boss room
                }
                else
                {
                    p2r1 = Instantiate(runOrder[p2CurRoom], p2Space);
                    player.transform.position = playerSpawns[1];
                }
                break;
            case 3:
                Destroy(p3r1);
                p3CurRoom++;
                if (p3CurRoom >= runOrder.Count)
                {
                    //player reaches boss room
                }
                else
                {
                    p3r1 = Instantiate(runOrder[p3CurRoom], p3Space);
                    player.transform.position = playerSpawns[2];
                }
                break;
            default:
                Destroy(p4r1);
                p4CurRoom++;
                if (p4CurRoom >= runOrder.Count)
                {
                    //player reaches boss room
                }
                else
                {
                    p4r1 = Instantiate(runOrder[p4CurRoom], p4Space);
                    player.transform.position = playerSpawns[3];
                }
                break;
        }
        float progress = (1.0f / (float)ROOM_COUNT) / 2.0f;
        pDiamondScript.ChangeProgress(playerNum-1, progress);
        RpcProgressUpdate();
        player.GetComponent<CollisionHandler>().ToggleInteractivity(true);
        
    }
    private bool PlayerInRoom()
    {
        //TODO: check if a player reaches boss room
        return true;
    }
    public void PlayerDied()
    {
        //invoke show
        //Invoke("ShowEnd", timeToShowEnd);
        //update final score
        //finalScore.text = string.Format("Rooms Cleared:\n{0}", roomsCleared);
    }

    /// <summary>
    /// Invokable method to show the end screen
    /// </summary>
    private void ShowEnd()
    {
        //endPlayer.mute = true;
        //endPlayer.PlayOneShot(endAudio);
        //gameOverUI.SetActive(true);
    }
    public void BackToMenu()
    {
        //endPlayer.PlayOneShot(click);
        SceneManager.LoadScene("MainMenu");
    }

    [ClientRpc]
    private void RpcProgressUpdate()
    {
        if (isServer)
        {
            progresses = pDiamondScript.progresses;
        }
    }

    public void updateCamera()
    {
        if(numPlayers == 1)
        {
            //Deinitialize progress diamond
            progressCanvas.gameObject.SetActive(false);

            player1.SetActive(true);
            if(player2 != null)
            {
                player2.SetActive(false);
            }
            

            p1camera.rect = new Rect(0, 0, 1, 1);
            main.rect = new Rect(0, 0, 0, 0);
            if(p2camera != null)
            {
                p2camera.rect = new Rect(0, 0, 0, 0);
            }
           

        }else if(numPlayers == 2)
        {
            //Initialize progress diamond
            progressCanvas.gameObject.SetActive(true);

            player2.SetActive(true);
            player3.SetActive(false);

            //Insert camera splits
            topToBottom.gameObject.SetActive(true);
            sideToSide.gameObject.SetActive(false);

            //Instantiate camera sizes
            p1camera.rect = new Rect(0, 0, .5f, 1);
            p2camera.rect = new Rect(.5f, 0, .5f, 1);
            p3camera.rect = new Rect(0, 0, 0, 0);
        }
        else if(numPlayers == 3)
        {
            player3.SetActive(true);
            player4.SetActive(false);

            //Insert horizontal camera split
            sideToSide.gameObject.SetActive(true);

            //Instantiate camera sizes
            p1camera.rect = new Rect(0, .5f, .5f, .5f);
            p2camera.rect = new Rect(.5f, .5f, .5f, .5f);
            p3camera.rect = new Rect(0, 0, .5f, .5f);
            main.rect = new Rect(.5f, 0, .5f, .5f);
            p4camera.rect = new Rect(0, 0, 0, 0);
        }
        else if(numPlayers == 4)
        {
            player4.SetActive(true);

            //Instantiate camera sizes
            p1camera.rect = new Rect(0, .5f, .5f, .5f);
            p2camera.rect = new Rect(.5f, .5f, .5f, .5f);
            p3camera.rect = new Rect(0, 0, .5f, .5f);
            p4camera.rect = new Rect(.5f, 0, .5f, .5f);
        }
    }

    /// <summary>
    /// Called when a player strikes the boss, awards points to the player
    /// </summary>
    public void AwardPointsByID()
    {

    }

    /// <summary>
    /// Called when the boss is defeated, ends the game
    /// Winner = most damage dealt to boss
    /// </summary>
    public void BossDefeated()
    {

    }
}
