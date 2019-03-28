using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class friend : MonoBehaviour {

	/*
	 * non-jitter arduino values(?)
	 */

	[SerializeField] Sprite[] veinpop;
	[SerializeField] Sprite[] sweat;
	[SerializeField] Sprite[] sparkle;
	[SerializeField] Sprite[] heart;
	[SerializeField] Transform eye;
	[SerializeField] SkinnedMeshRenderer skmr;

	[SerializeField] Sprite shadowsprite;
	[SerializeField] GameObject shadowProj;

	GameObject shadow;

	float tolerance; //amount of time it can take in a bad hug
	float sweet; //amount of time it wants in a good hug
	int sweetspot; //between 1 and 9 (inclusive), the intensity of their desired hug

	float tolerancetimer;
	float sweettimer;

	int lastintensity;

	SpriteRenderer[] emotions;

	string emotion;
	string lastEmotion;

	Dictionary<string, Sprite[]> emotionsprites;

	// Use this for initialization
	void Start () {
		tolerance = Random.Range (10f, 15f);
		sweet = Random.Range(5f,10f);
		sweetspot = Random.Range(1,10);
		tolerancetimer = 0;
		sweettimer = 0;
		lastintensity = -1;

		Animator anim = transform.GetChild(0).GetComponent<Animator> ();
		float randomStart = Random.Range (0, anim.GetCurrentAnimatorStateInfo (0).length);
		anim.Play ("idle", 0, randomStart);

		lastEmotion = "sweat";
		emotion = "sweat";

		emotionsprites = new Dictionary<string, Sprite[]> ();
		emotionsprites.Add ("sparkle", sparkle);
		emotionsprites.Add ("sweat", sweat);
		emotionsprites.Add ("veinpop", veinpop);
		emotionsprites.Add ("heart", heart);

		emotions = new SpriteRenderer[3];
		for (int i = 0; i < emotions.Length; i++) {
			GameObject e = new GameObject ("emotionsprite");
//			e.transform.parent = transform;
			e.transform.LookAt (Camera.main.transform);
			e.gameObject.AddComponent<SpriteRenderer> ();
			emotions[i] = e.GetComponent<SpriteRenderer>();
			emotions[i].sprite = emotionsprites[emotion][i];
			Vector3 pos = eye.position+Random.onUnitSphere*1.5f;
			emotions [i].transform.position = new Vector3(pos.x-3f,pos.y+3f,pos.z-3f);
			Sequence idle = DOTween.Sequence ();
			idle.Append (emotions [i].transform.DOScale (Vector3.one * 0.75f, 	Random.Range(0.3f,0.7f)));
			idle.Append (emotions [i].transform.DOScale (Vector3.one, 			Random.Range(0.3f,0.7f)));
			idle.SetLoops (-1);
		}

//		shadow = new GameObject ();
//		shadow.transform.position = transform.position + new Vector3 (0, 0.05f, 0);
//		shadow.AddComponent<SpriteRenderer> ();
//		shadow.transform.Rotate (new Vector3 (90f,0f,0f));
//		shadow.transform.localScale = Vector3.one * 5f;
//		shadow.GetComponent<SpriteRenderer> ().sprite = shadowsprite;
//		shadow.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, 0.25f);
		shadow = Instantiate(shadowProj);
	}

	// Update is called once per frame
	void Update () {
		if (lastEmotion != emotion) {
			//change position
			for (int i = 0; i < 3; i++) {
				Vector3 pos = eye.position+Random.onUnitSphere*1.5f;
				emotions [i].transform.position = new Vector3(pos.x-2.5f,pos.y+3f,pos.z-2.5f);
				if (emotion != "null") {
					Sequence zhwoop = DOTween.Sequence ();
					int index = i;
					zhwoop.Append (emotions [i].transform.DOScale (Vector3.zero, 0.1f));
					zhwoop.AppendCallback (() => changeSprite (emotion, index));
					zhwoop.Append (emotions [i].transform.DOScale (Vector3.one, 0.1f));

					if (emotion == "sparkle") {
						Sequence shiny = DOTween.Sequence ();
						shiny.Append (emotions [i].transform.DOScale (Vector3.one * 0.75f, 0.1f));
						shiny.Append (emotions [i].transform.DOScale (Vector3.one * 1.5f, 0.3f));
						shiny.Append (emotions [i].transform.DOScale (Vector3.one * 1f, 0.2f));
						shiny.Append (emotions [i].transform.DOScale (Vector3.zero, 0.5f).SetDelay (0.1f));
						shiny.AppendCallback (() => nullEmotion ());
					} else if( emotion == "heart"){

                       /* if (!FindObjectOfType<AudioController>().happy.isPlaying)
                        {*/
                           FindObjectOfType<AudioController>().happy.Play();
                        //}
                    } else if (emotion == "veinpop"){
                        FindObjectOfType<AudioController>().angry.Play();
                    }
				} else {
					emotions [i].sprite = null;
				}
			}
			lastEmotion = emotion;
		}

//		RaycastHit hit;
//		if (Physics.Raycast (transform.position+Vector3.up,Vector3.down,out hit)) {
//			GameObject objectHit = hit.transform.gameObject;
//			shadow.transform.position = new Vector3 (transform.position.x,
//				hit.point.y+0.01f,
//				transform.position.z);
//		}
		shadow.transform.position=new Vector3(transform.position.x,
			transform.position.y+2f,
			transform.position.z);
	}

	void changeSprite(string e, int i){
		emotions [i].sprite = emotionsprites [e] [i];
	}
	void nullEmotion(){
		emotion = "null";
	}

	public Transform getEye(){
		return eye;
	}

	//returns:
	//	0: neutral state (either building up + or -)
	//	1: you hugged good
	//	2: omg they're angry
	public int hug(int intensity){
		int result = 0;
//		Debug.Log (intensity + ", you want " + sweetspot);
		if (lastintensity == -1) {
			lastintensity = intensity;
			return 0;
		}

		skmr.SetBlendShapeWeight(0,intensity*10);

		if (intensity == sweetspot) {
			emotion = "heart";


		}
		if (intensity == sweetspot && lastintensity == intensity) {
			sweettimer += Time.deltaTime;
			emotion = "heart";

		}
		if (intensity != sweetspot) {
//			sweettimer = 0; //maybe too harsh lol
			emotion = "sweat";
		}
		if (Mathf.Abs (intensity - sweetspot) > 2) {
			//ur more than 3 away from desired sweet spot
			emotion = "veinpop";
			tolerancetimer += Time.deltaTime;
		}
		lastintensity = intensity;

		if (sweettimer >= sweet) {
			result = 1;
			skmr.SetBlendShapeWeight (0, 0);
			emotion = "sparkle";
		}
		if (tolerancetimer >= tolerance) {
			result = 2;
			tolerancetimer = 0;
			sweettimer = 0;
			emotion = "sweat";
		}
		return result;
	}
}
