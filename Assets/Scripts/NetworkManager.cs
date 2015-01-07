using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{	
    private const string typeName = "Draw_Game";
    private const string gameName = "RoomName";

    private bool isRefreshingHostList = false;
    private HostData[] hostList;

    public GameObject playerPrefab;
	GameObject[] spawnPoints;

	//Create GUI buttons for connecting to and starting a server
    void OnGUI()
    {
        if (!Network.isClient && !Network.isServer)
        {
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                StartServer();

            if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
                RefreshHostList();

			//On refreshing the host, create a button for each open lobby
            if (hostList != null)
            {
                for (int i = 0; i < hostList.Length; i++)
                {
                    if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
                        JoinServer(hostList[i]);
                }
            }
        }
    }
	//Create the server
    private void StartServer()
    {
        Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    }
	//On server creation, spawn a player for the host
    void OnServerInitialized()
    {
        SpawnPlayer();
    }

	//If the client is searching for hosts and has succesfully found some, stop looking for hosts
    void Update()
    {
        if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
        {
            isRefreshingHostList = false;
            hostList = MasterServer.PollHostList();
        }
    }

    private void RefreshHostList()
    {
        if (!isRefreshingHostList)
        {
            isRefreshingHostList = true;
            MasterServer.RequestHostList(typeName);
        }
    }


    private void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

    void OnConnectedToServer()
    {
        SpawnPlayer();
    }

	[RPC]
	void Damage(GameObject target, float damage) {
		PlayerInfo infoScript = target.GetComponent("PlayerInfo") as PlayerInfo;
		infoScript.health -= damage;
	}
	
    private void SpawnPlayer()
    {
		spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
		Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].transform;
		spawnPoint.Rotate(new Vector3(0, Random.Range(-360, 360), 0));
        Network.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation, 0);
    }
}
