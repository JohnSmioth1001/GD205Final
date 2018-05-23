using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager {

    private bool firstPlayerJoined;
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        // base.OnServerAddPlayer(conn, playerControllerId);
        GameObject playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        List<Transform> spawnPositions = NetworkManager.singleton.startPositions;

        if (!firstPlayerJoined)
        {
            firstPlayerJoined = true;
            playerObj.transform.position = spawnPositions[0].position;
        }
        else
        {
            playerObj.transform.position = spawnPositions[1].position;
        }

        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);

    }


    void SetPortandAddress()
    {
        NetworkManager.singleton.networkAddress = "localhost";

        NetworkManager.singleton.networkPort = 7777;
       
    }

    public void HostGame()
    {
        SetPortandAddress();
        NetworkManager.singleton.StartHost();
    }

    public void JoinTheGame()
    {
        SetPortandAddress();
        NetworkManager.singleton.StartClient();

    }

}//class



















