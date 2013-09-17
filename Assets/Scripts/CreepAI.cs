using UnityEngine;
using System.Collections;

public class CreepAI : MonoBehaviour
{
	public Waypoint nextWaypoint;
	private Character c;
	public Character target = null;
	public Vector3 movement;
	public float speed = 2;
	public bool tower = false;
	public Transform ShotPrefab;
	
	void Start ()
	{
		c = GetComponent<Character> ();
	}
	
	void Awake ()
	{
		c = GetComponent<Character> ();
	}
	
	float gunTime = 0;
	
	void Update ()
	{
		if (Network.isServer) {
			if (nextWaypoint != null) {
				if (target == null && !tower) {
					transform.LookAt (nextWaypoint.transform);
					float distance = (transform.position - nextWaypoint.transform.position).magnitude;
					if (distance < 1f) {
						nextWaypoint = nextWaypoint.next;
					} else {
						movement = transform.TransformDirection (Vector3.forward) * speed - Vector3.up * 10;
					}
				} else if (target != null) {
					if (!tower) {
						transform.LookAt (target.transform);
						Vector3 euler = transform.localEulerAngles;
						euler.x = 0;
						euler.z = 0;
						transform.localEulerAngles = euler;
					}
					float distance = (transform.position - target.transform.position).magnitude;
					if (distance < c.range) {
						if (!tower)
							movement = Vector3.zero - Vector3.up * 10;
						if (gunTime <= 0 && target.health > 0) {
							Network.Instantiate (ShotPrefab, transform.position, transform.rotation, 0);
							target.Hit (c.Damage ());
							gunTime = 1;
						}
						gunTime -= Time.deltaTime;
					} else if (!tower) {
						movement = transform.TransformDirection (Vector3.forward) * speed - Vector3.up * 10;
					}
				}
			
			} else if (target == null && !tower) {
				movement = Vector3.zero - Vector3.up * 10;
			}
		}

		// creep died
		if (Network.isServer && c.health <= 0) {
			Network.Destroy (gameObject);
		}
		if (!tower) {
			GetComponent<CharacterController> ().Move (this.movement * Time.deltaTime);
		}
	}
	
	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			Vector3 pos = transform.position;
			float rot = transform.eulerAngles.y;
			Vector3 vel = movement;
			float health = c.health;
			int id = c.charID;
			stream.Serialize (ref id);
			if (!tower) {
				stream.Serialize (ref pos);
				stream.Serialize (ref rot);
				stream.Serialize (ref vel);
			}
			stream.Serialize (ref health);
		} else {
			Vector3 posReceive = Vector3.zero;
			float rotReceive = 0;
			Vector3 velReceive = Vector3.zero;
			float health = 0;
			int id = 0;
			stream.Serialize (ref id);
			if (!tower) {
				stream.Serialize (ref posReceive);
				stream.Serialize (ref rotReceive);
				stream.Serialize (ref velReceive);
				transform.position = posReceive;
				Vector3 rot = transform.eulerAngles;
				rot.y = rotReceive;
				transform.eulerAngles = rot;
				movement = velReceive;
			}
			stream.Serialize (ref health);
			c.health = health;
			c.charID = id;
		}
	}
}
