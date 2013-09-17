using UnityEngine;
using System.Collections;
using System;
 
public class GameInit : MonoBehaviour
{
	public int connectionPort = 25001;
	string connectionIP = "127.0.0.1";
	string jogadores = "2";
 
	void OnGUI ()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			GUI.Label (new Rect (10, 10, 300, 20), "Status: Disconnected");
			//cangaceiro = GUI.Toggle
			GUI.Label (new Rect (10, 30, 100, 20), "Players: ");
			jogadores = GUI.TextField (new Rect (120, 30, 50, 20), jogadores);
			
			connectionIP = GUI.TextField (new Rect (10, 50, 300, 20), connectionIP);
			if (GUI.Button (new Rect (10, 70, 120, 20), "Client Connect")) {
				Network.Connect (connectionIP, connectionPort);
			}
			if (GUI.Button (new Rect (10, 90, 120, 20), "Initialize Server")) {
				GameManager.players = Convert.ToInt32 (jogadores);
				Network.InitializeServer (32, connectionPort, false);
			}
		} else if (Network.peerType == NetworkPeerType.Client) {
			GUI.Label (new Rect (10, 10, 300, 20), "Status: Connected as Client");
			if (GUI.Button (new Rect (10, 30, 120, 20), "Disconnect")) {
				Network.Disconnect (200);
				Application.LoadLevel ("Main");
			}
		} else if (Network.peerType == NetworkPeerType.Server) {
			GUI.Label (new Rect (10, 10, 300, 20), "Status: Connected as Server");
			if (GUI.Button (new Rect (10, 30, 120, 20), "Disconnect")) {
				Network.Disconnect (200);
				Application.LoadLevel ("Main");
			}
		}
	}
}