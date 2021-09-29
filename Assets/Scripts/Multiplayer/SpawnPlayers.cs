using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;    
    public float minY;
    public float maxY;
    public float minz;
    public float maxZ;

    public void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        Vector3 RandomPosition =
            new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minz, maxZ));
       GameObject myPlayer = (GameObject)PhotonNetwork.Instantiate(playerPrefab.name, RandomPosition, Quaternion.identity);
       myPlayer.GetComponentInChildren<Camera>().gameObject.SetActive(true);
    }

}
