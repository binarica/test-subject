using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    PlayerSpawner spawner;

    [SerializeField]
    CameraControl cameraControl;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MySceneManager.Instance.QuitGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            MySceneManager.Instance.Restart();
        }
        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnPlayer();
        }*/
    }
    
    public void SpawnPlayer()
    {
        PowerManager.Instance.ResetToOriginalState();
        GameObject player = spawner.InstantiatePlayer();
        cameraControl.SetPlayer(player);
        PowerManager.Instance.ResetPlayer(player);
    }
}
