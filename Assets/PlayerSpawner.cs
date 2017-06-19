using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {


    [SerializeField]
    GameObject player;

    public GameObject InstantiatePlayer()
    {
        GameObject playerGO = Instantiate(player, transform.position, Quaternion.identity);
        return playerGO;
    }
}
