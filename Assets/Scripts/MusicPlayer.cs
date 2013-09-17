using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

	public AudioClip[] musics;
	
	public float intervalTime = 5f;
	
	private float lastPlayingTime;
	
	private int lastIndex;
		
	void Start() {
		lastIndex = -1;
		lastPlayingTime = Time.time - 2 * intervalTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameObject.audio.isPlaying && (Time.time - lastPlayingTime) > intervalTime) {
			int nextIndex = -1;
			do {
				nextIndex = GetNextIndex();
			} while(nextIndex == lastIndex);
			lastIndex = nextIndex;
			gameObject.audio.clip = musics[nextIndex];
			gameObject.audio.Play();
		}
	}
	
	private int GetNextIndex() {
		return Random.Range(0, musics.Length);
	}
}