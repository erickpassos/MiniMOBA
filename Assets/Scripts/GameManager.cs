using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	
	// hero prefabs
	public Transform playerPrefabA;
	public Transform playerPrefabB;
	
	// creep prefabs
	public CreepAI creepPrefabA;
	public CreepAI creepPrefabB;
	
	// lanes
	public Waypoint spawnA;
	public Waypoint spawnB;
	
	public static bool paused = true;
	
	public List<Hero> playerScripts = new List<Hero> ();
	private int charNumber = 0;
	public static int players = 0;
	private int playersConnected = 0;
	private NetworkPlayer[] networkPlayers;
 	private bool init = false;
	
	void Start() {
		paused = true;
	}
	
	void OnServerInitialized ()
	{
		networkPlayers = new NetworkPlayer[players];
		networkPlayers[playersConnected++] = Network.player;
		if (playersConnected >= players) {
			InitGame();
		}
	}
	
	private void InitGame() {
		paused = false;
		int count = 0;
		init = true;
		foreach(NetworkPlayer player in networkPlayers) {
			if (count < players / 2) {
				SpawnPlayer (playerPrefabA, player, spawnA.transform.position + new Vector3(-2-2*count,0.5f,0));
			}
			else {
				SpawnPlayer (playerPrefabB, player, spawnB.transform.position + new Vector3(-2-2*count,0.5f,0));
			}
			count++;
		}
	}
	
	void OnPlayerConnected (NetworkPlayer player)
	{
		networkPlayers[playersConnected++] = player;
		if (playersConnected >= players) {
			InitGame();
		}
	}
	
	void SpawnPlayer (Transform prefab, NetworkPlayer player, Vector3 position)
	{
		string tempPlayerString = player.ToString ();
		int playerNumber = Convert.ToInt32 (tempPlayerString);
		Transform newPlayerTransform = (Transform)Network.Instantiate (prefab, position, transform.rotation, playerNumber);
		newPlayerTransform.GetComponent<Character>().charID = charNumber++;
		playerScripts.Add (newPlayerTransform.GetComponent<Hero> ());
		NetworkView theNetworkView = newPlayerTransform.networkView;
		theNetworkView.RPC ("SetPlayer", RPCMode.AllBuffered, player);
	}
	
	private float time = 15;

	public void Update ()
	{
		if (Network.isServer && init && Character.victoriousTeam == '0') {
			if (time >= 15) {
				// creep A
				SpawnCreep(creepPrefabA, spawnA, 0);
				SpawnCreep(creepPrefabA, spawnA, -2);
				SpawnCreep(creepPrefabA, spawnA, -4);
				
				// creep B
				SpawnCreep(creepPrefabB, spawnB, 0);
				SpawnCreep(creepPrefabB, spawnB, 2);
				SpawnCreep(creepPrefabB, spawnB, 4);
				time = 0;
			}
			time += Time.deltaTime;
		}
	}
	
	private void SpawnCreep(CreepAI prefab, Waypoint spawn, int offset) {
		CreepAI creep = (CreepAI) Network.Instantiate (prefab, spawn.transform.position + new Vector3(offset,0,offset), spawn.transform.rotation, 0);
		creep.nextWaypoint = spawn;
		creep.GetComponent<Character>().charID = charNumber++;
	}
	
}
