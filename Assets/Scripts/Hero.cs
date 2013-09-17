using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{

	public NetworkPlayer theOwner;
	Vector3 lastClientClick;
	Vector3 serverCurrentClick;
	bool clicked = false;
	private Character c;
	private CharacterController charController;
	private Vector3 movement;
	private Vector3 originalPos;
	private Character target;
	public Transform ShotPrefab;
	
	void Start ()
	{
		c = GetComponent<Character> ();
		charController = GetComponent<CharacterController> ();
	}
	
	void Awake ()
	{
		if (Network.isServer) {
			originalPos = transform.position;
		}
		c = GetComponent<Character> ();
		charController = GetComponent<CharacterController> ();
	}

	[RPC]
	void SetPlayer (NetworkPlayer player)
	{
		theOwner = player;
		if (player == Network.player)
			enabled = true;
	}
	
	private float gunTime = 0;
	
	void Update ()
	{
		if (Network.player == theOwner) {
			Camera.main.GetComponent<CameraControl> ().SetTarget (transform);
			if (Input.GetButtonDown ("Fire1")) {
				Vector2 point = Input.mousePosition;
				RaycastHit hit = new RaycastHit ();
				Vector3 click = lastClientClick;
				string hitName = name;
				if (Physics.Raycast (Camera.main.ScreenPointToRay (point), out hit, 100.0f)) {
					if (hit.collider.name != transform.name) {
						click = hit.point;
						hitName = hit.collider.name;
					}
				}
				
				if (lastClientClick != click) {
					lastClientClick = click;
					if (Network.isServer) {
						SendMovementInput (click.x, click.y, click.z, hitName);
					} else if (Network.isClient) {
						networkView.RPC ("SendMovementInput", RPCMode.Server, click.x, click.y, click.z, hitName);
					}
				}
			}
		}
		
		if (Network.isServer) {
			gunTime -= Time.deltaTime;
			if (target == null) {
				float distance = (serverCurrentClick - transform.position).magnitude;
				if (serverCurrentClick != Vector3.zero && distance > 1) {
					transform.LookAt (serverCurrentClick);
					Vector3 euler = transform.localEulerAngles;
					euler.x = 0;
					euler.z = 0;
					transform.localEulerAngles = euler;
					movement = transform.TransformDirection (Vector3.forward) * 5 - Vector3.up * 10;
				}
				else {
					movement = Vector3.zero - Vector3.up * 10;
				}
			} else {
				transform.LookAt (target.transform);
				float distance = (transform.position - target.transform.position).magnitude;
				if (distance < c.range) {
					movement = Vector3.zero - Vector3.up * 10;
					if (clicked) {
						clicked = false;
						if (gunTime <= 0) {
							gunTime = 0.25f;
							if (target.health > 0) {
								target.Hit(c.Damage());
								Network.Instantiate (ShotPrefab, transform.position, transform.rotation, 0);
								if (target.health <= 0 && target.tag != tag) {
									if (target.isHero) {
										c.Xp(3);
									}
									else {
										c.creeps++;
										c.Xp(1);
									}
								}
							}
						}
					}
				} else {
					movement = transform.TransformDirection (Vector3.forward) * 5 - Vector3.up * 10;
				}
			}
			
			if (c.health <= 0) {
				c.health = c.maxHealth;
				transform.position = originalPos;
			}
			else {
				c.Hit(-0.25f * Time.deltaTime);
			}
		}
		
		charController.Move(movement*Time.deltaTime);
	}

	[RPC]
	void SendMovementInput (float x, float y, float z, string hitName)
	{
		if (hitName != "Terrain") {
			GameObject go = GameObject.Find (hitName);
			if (go != null) {
				target = go.GetComponent<Character> ();
				
			}
		} else {
			target = null;
		}
		serverCurrentClick = new Vector3 (x, y, z);
		clicked = true;
	}
	
	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			Vector3 pos = transform.position;
			float rot = transform.eulerAngles.y;
			Vector3 vel = movement;
			float health = c.health;
			int id = c.charID;
			int kills = Character.killsB;
			if (tag == "teamA")
				kills = Character.killsA;
			int level = c.level;
			char v = Character.victoriousTeam;
			stream.Serialize (ref v);
			stream.Serialize (ref id);
			stream.Serialize (ref kills);
			stream.Serialize (ref level);
			stream.Serialize (ref pos);
			stream.Serialize (ref rot);
			stream.Serialize (ref vel);
			stream.Serialize (ref health);
		} else {
			Vector3 posReceive = Vector3.zero;
			float rotReceive = 0;
			Vector3 velReceive = Vector3.zero;
			float health = 0;
			int id = 0;
			int kills = 0;
			int level = 0;
			char v = '0';
			stream.Serialize (ref v);
			stream.Serialize (ref id);
			stream.Serialize (ref kills);
			stream.Serialize (ref level);
			stream.Serialize (ref posReceive);
			stream.Serialize (ref rotReceive);
			stream.Serialize (ref velReceive);
			stream.Serialize (ref health);
			Character.victoriousTeam = v;
			transform.position = posReceive;
			Vector3 rot = transform.eulerAngles;
			rot.y = rotReceive;
			transform.eulerAngles = rot;
			movement = velReceive;
			c.health = health;
			c.charID = id;
			if (tag == "teamA")
				Character.killsA = kills;
			else
				Character.killsB = kills;
			c.level = level;
		}
	}
}
