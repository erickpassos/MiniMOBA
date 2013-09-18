using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
	public string team;
	public float health;
	public float maxHealth;
	public int creeps = 0;
	public int level = 1;
	public float range = 2;
	public float damage = 1.5f;
	private Texture2D backgroundTexture;
	public Texture2D healthTexture;
	public int charID = 0;
	public bool isHero = false;
	public bool isBase = false;
	public static int killsA = 0;
	public static int killsB = 0;
	public static char victoriousTeam = '0';
	public float xp = 0;
	public GUISkin skin;
	public Transform bloodPrefab;
	public Animation avatar;
	
	void Awake ()
	{
		backgroundTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		backgroundTexture.SetPixel (0, 0, Color.black);
		backgroundTexture.Apply ();
	}
	
	private float oldHealth;
	void Update ()
	{
		if (oldHealth > health) {
			Instantiate (bloodPrefab, transform.position, transform.rotation);
			audio.Play();
		}
		gameObject.name = "char" + charID;
		if (avatar != null) {
			Vector3 v = GetComponent<CharacterController>().velocity;
			v.y = 0;
			if (v.magnitude > 0f) {
				avatar.CrossFade("walk-cicle",0.1f);
			}
			else {
				avatar.CrossFade("idle", 0.5f);
			}
		}
		oldHealth = health;
	}
	
	public void Xp(float xp) {
		this.xp += xp;
		if (level < 10 && this.xp >= 5) {
			level++;
			this.health = maxHealth;
			this.xp = 0;
			if (level >= 10)
				level = 10;
		}
	}
	
	public float Damage() {
		if (isHero) {
			return 2 * (0.833f + level*0.16667f);
		}
		else {
			return damage;
		}
	}
	
	public void Hit (float damage)
	{
		if (victoriousTeam != '0') {
			return;
		}
		if (isHero)
			health -= damage/level;
		else
			health -= damage;
		if (health < 0) {
			health = 0;
			if (isHero) {
				if (tag == "teamA") {
					killsB++;
				}
				else {
					killsA++;
				}
			}
			
			if (isBase) {
				if (tag == "teamA") {
					victoriousTeam = 'v';
				}
				else {
					victoriousTeam = 'c';
				}
				Debug.Log(victoriousTeam);
			}
			
		} else if (health > maxHealth) {
			health = maxHealth;
		}
	}
	
	// Health Bar (all) and level (hero only)
	void OnGUI ()
	{
		if (GameManager.paused)
			return;
		GUI.skin = skin;
		GUI.depth = 3;

		Vector2 backgroundBarSize = new Vector2 (Screen.width * 0.2f, Screen.height * 0.06f);
			
		Vector3 viewPos = Camera.main.WorldToScreenPoint (this.transform.position + new Vector3 (0, 3, 0));
			
		float valueZ = viewPos.z;
		if (valueZ < 1) {
			valueZ = 1;
		} else if (valueZ > 4) {
			valueZ = 4;
		}
		float valueToNormalize = Mathf.Abs (1 / (valueZ - 0.5f));
			
		int backgroundBarWidth = (int)(backgroundBarSize.x * valueToNormalize);
		if (backgroundBarWidth % 2 != 0) {
			backgroundBarWidth++;
		}
		float backgroundBarHeight = (int)(backgroundBarSize.y * valueToNormalize);
		if (backgroundBarHeight % 2 != 0) {
			backgroundBarHeight++;
		}
			
		float innerBarWidth = backgroundBarWidth - 2 * 2;
		float innerBarHeight = backgroundBarHeight - 2 * 2;
			
			
		float posYHealthBar = Screen.height - viewPos.y - backgroundBarHeight;
			
		GUI.BeginGroup (new Rect (viewPos.x - backgroundBarWidth / 2, posYHealthBar, backgroundBarWidth, backgroundBarHeight));
		GUI.DrawTexture (new Rect (0, 0, backgroundBarWidth, backgroundBarHeight), backgroundTexture, ScaleMode.StretchToFill);
			
		float healthPercent = (health / maxHealth);
		GUI.DrawTexture (new Rect (2, 2, innerBarWidth * healthPercent, innerBarHeight), healthTexture, ScaleMode.StretchToFill);
		
		GUI.EndGroup ();
		
		if (isHero) {
			GUI.Label(new Rect (viewPos.x - 50, posYHealthBar - 23, 100, 25), "Level "+level);
		}
			
	}
	
	
}
