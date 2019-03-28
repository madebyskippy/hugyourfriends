using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class hugger : MonoBehaviour {

	[SerializeField] mouse control;

	Vector2 direction = new Vector2(0,0);
	[SerializeField] Rigidbody rb;
	[SerializeField] Animator anim;
	[SerializeField] Transform target;
	[SerializeField] GameObject shadowProj;
	[SerializeField] Sprite shadowsprite;

    AudioController audioController;
	bool onGround;

	bool isHugging;
	bool isHuggingBoss;
	friend currentFriend;

    //[SerializeField]
    Vector3 originalCamPos, originalCamAngles;
    [SerializeField] Transform newCam;

    [SerializeField] Material[] friendMaterials;
    [SerializeField] Color[] originalFriendColors;

	List<GameObject> friends = new List<GameObject>();

	[SerializeField] GameObject[] LJoints;
	[SerializeField] GameObject[] RJoints;

	GameObject shadow;

    [SerializeField] Image fade;


	//[SerializeField] GameObject particlePrefab;
	//List<GameObject> particleSystems;

	// Use this for initialization

	private void Awake()
	{
        for (int i = 0; i < originalFriendColors.Length; ++i){
            friendMaterials[i].color = originalFriendColors[i];
        }
        audioController = FindObjectOfType<AudioController>();
	}
	void Start () {
		isHugging = false;
		isHuggingBoss = false;
		onGround = true;

//		shadow = new GameObject ();
//		shadow.transform.position = transform.position + new Vector3 (0, 0.05f, 0);
//		shadow.AddComponent<SpriteRenderer> ();
//		shadow.transform.Rotate (new Vector3 (90f,0f,0f));
//		shadow.transform.localScale = Vector3.one * 3f;
//		shadow.GetComponent<SpriteRenderer> ().sprite = shadowsprite;
//		shadow.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, 0.25f);
		shadow=Instantiate(shadowProj);
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.R)) {
			SceneManager.LoadScene ("main");
		}

		RaycastHit hit;
		if (Physics.Raycast (target.position,Vector3.down,out hit)) {
            //			Debug.Log (hit.point.y + "," + target.position.y);
            //if(hit.transform.gameObject.layer == GetComponent<Projector>)
            //8 is borders
           /* if(hit.transform.gameObject.layer == 8){
                shadowProj.GetComponent<ProjectorController>().IgnoreFloorLayer(true);
            }  else{
                shadowProj.GetComponent<ProjectorController>().IgnoreFloorLayer(false);
            }*/

			if (Mathf.Abs(hit.point.y - target.position.y) < 1.25f) {
				onGround = true;
			} else {
				onGround = false;
			}
		}

		direction.x = Input.GetAxis ("Horizontal");
		direction.y = Input.GetAxis ("Vertical");

		Vector3 oldvel = rb.velocity;
		Vector3 newvel = Vector3.zero;

		if (Input.GetKeyDown (KeyCode.Space)) { //this was for testing
			newvel = new Vector3(direction.x,0f,direction.y)*5f;
		}

		int i = (int)(control.getVal() * 10f);
		if (isHugging) {
			int state = currentFriend.hug (i);

			anim.Play ("hug" + (i).ToString ());
			if (state != 0) {
                audioController.bgm.DOPitch(1f, 1f);
                audioController.bgm_rolling.DOPitch(1f, 1f);
				Camera.main.GetComponent<camerascript> ().setFriend (null);
				Camera.main.GetComponent<camerascript> ().zoomOut ();
				isHugging = false;
				rb.isKinematic = false;
				anim.Play ("idle");
				if (state == 1) {
					Debug.Log ("u hugged them good");
					currentFriend.tag = "Untagged"; // u can't hug them anymore
					currentFriend.GetComponent<CapsuleCollider>().enabled = false;
					friends.Add (currentFriend.gameObject);
                    if(audioController.hugaccepted[friends.Count - 1])
                    audioController.hugaccepted[friends.Count - 1].Play();
                    Debug.Log("playing");
				}
				currentFriend = null;
			}

		} else {
			newvel = new Vector3 (direction.x, 0f, direction.y) * i;
		}

		if (!isHuggingBoss) {
			if (friends.Count > 0) {
				Vector3 pos = Vector3.Lerp (friends [0].transform.position, 
					             transform.position - (transform.position - friends [0].transform.position).normalized * 3f, 
					             0.25f);
				friends [0].transform.position = new Vector3 (pos.x, target.position.y-0.75f, pos.z);
				friends [0].transform.LookAt (new Vector3 (transform.position.x,
					friends [0].transform.position.y, transform.position.z));
				for (int j = 1; j < friends.Count; j++) {
					pos = Vector3.Lerp (friends [j].transform.position, 
						friends [j - 1].transform.position - (friends [j - 1].transform.position - friends [j].transform.position).normalized * 3f, 
						0.25f);
					friends [j].transform.position = new Vector3 (pos.x, pos.y, pos.z);
					friends [j].transform.LookAt (new Vector3 (transform.position.x,
						friends [j].transform.position.y, transform.position.z));
				}
			}
		}

		if (onGround) {
			rb.velocity = newvel;
		}

        if(rb.velocity.sqrMagnitude > 0.02f){
            for (int f = 0; f < friends.Count; ++f)
            {
                friends[f].GetComponentInChildren<Animator>().SetTrigger("hop");
            }
            if(audioController.bgm_rolling.volume == 0f){
                audioController.bgm_rolling.DOFade(1f, 0.5f);
            }
            //audioController
        } else{
            if (audioController.bgm_rolling.volume == 1f)
            {
                audioController.bgm_rolling.DOFade(0f, 0.5f);
            }
        }

		if (rb.velocity.sqrMagnitude >0.005f) {
			anim.SetTrigger ("ready");


		}if (rb.velocity.sqrMagnitude<0.0001f) {
			anim.SetTrigger ("stop");
            for (int f = 0; f < friends.Count; ++f)
            {
                friends[f].GetComponentInChildren<Animator>().SetTrigger("idle");
            }
		}

//		update shadow position
//		shadow.transform.position = target.position - new Vector3(0,0.75f,0);
//		RaycastHit hit;
//		if (Physics.Raycast (target.position,Vector3.down,out hit)) {
//			GameObject objectHit = hit.transform.gameObject;
//			if (objectHit.tag != "friend" && objectHit.tag != "Finish") {
//				//if it's not the boss or the friends
//				shadow.transform.position = new Vector3 (target.position.x,
//					hit.point.y+0.01f,
//					target.position.z);
//			}
//		}

		shadow.transform.position=new Vector3(target.transform.position.x,
			target.transform.position.y+1f,
			target.transform.position.z);

	}

	public void setgoin(){
		anim.SetTrigger ("goin");
	}
	public void setidle(){
		anim.SetTrigger ("idle");
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "friend") {
			//ENTER HUG PHASE
			if (!isHugging) {
				Debug.Log ("hug!!");
				currentFriend = col.gameObject.GetComponent<friend>();

				//position them in front of the friend
				transform.LookAt (currentFriend.gameObject.transform);
				Vector3 rot = transform.rotation.eulerAngles;
				transform.DOLocalRotate(new Vector3(0f,rot.y+180,0f),1f);
				transform.DOMoveY (currentFriend.transform.position.y+1.198f, 1f);
				Vector3 pos = transform.position;
				Vector3 tofriend = transform.position - currentFriend.transform.position;
				tofriend = tofriend.normalized;
				Vector3 endpos = currentFriend.transform.position + tofriend * 2.5f;
				transform.DOMove (new Vector3 (endpos.x, pos.y, endpos.z), 0.5f);


				currentFriend.transform.LookAt (new Vector3 (transform.position.x,
					currentFriend.transform.position.y, transform.position.z));

				anim.SetTrigger ("hugging"); //arms outstretch
				rb.velocity = Vector3.zero;
				rb.isKinematic = true;
				isHugging = true;

				Camera.main.GetComponent<camerascript> ().setFriend (currentFriend.getEye());
				Camera.main.GetComponent<camerascript> ().zoomIn ();

                audioController.bgm.DOPitch(2f, 1f);
                audioController.bgm_rolling.DOPitch(2f, 1f);

			}
		}
		if (col.gameObject.tag == "Finish") {
			//big boss
			if (friends.Count >= 4) { //you have everyone with you
                audioController.bgm.Stop();
                audioController.bgm_rolling.Stop();

                CameraControl();

				isHuggingBoss = true;
				//hug the fuckin boss
				//i love copypasting code
				GameObject boss = col.gameObject;
				transform.LookAt (boss.transform);
				Vector3 rot = transform.rotation.eulerAngles;
				transform.DOLocalRotate (new Vector3 (0f, rot.y + 180, 0f), 1f);
				transform.DOMoveY (boss.transform.position.y+1.198f, 1f);
				Vector3 tofriend = transform.position - boss.transform.position;
				tofriend = tofriend.normalized;
				Vector3 endpos = boss.transform.position + tofriend * 4f;
				transform.DOMove (new Vector3 (endpos.x, transform.position.y, endpos.z), 0.5f);
				anim.Play("hugboss");
				rb.velocity = Vector3.zero;
				rb.isKinematic = true;

				boss.transform.DOLookAt (new Vector3 (target.position.x, 0f, target.position.z), 1.5f);

				float radius = 4.5f;
				float yourangle = Vector3.Angle(new Vector3(0,0,1),target.position-boss.transform.position);
				if (target.position.x < boss.transform.position.x) {
					yourangle = yourangle * -1 + 360;
				}
				float handangle = 8 * 12;
				handangle *= Mathf.Deg2Rad;
				yourangle *= Mathf.Deg2Rad;
				for (int i = 0; i < friends.Count; i++) {
					//tween to position
//					friends[i].transform.DOMove();
					float offset = -1;
					if (i < 2)
						offset = 1;
					Vector2 center = new Vector2(boss.transform.position.x,boss.transform.position.z);
					Vector2 point = new Vector2 (radius * Mathf.Sin (yourangle+offset*(handangle+Mathf.PI / 4 * (i%2)))+center.x, 
						radius * Mathf.Cos (yourangle+offset*(handangle+Mathf.PI / 4 * (i%2)))+center.y);
					Sequence friendhug = DOTween.Sequence ();
					int index = i;
					friendhug.Append (friends [index].transform.DOMove (new Vector3 (point.x, 0f, point.y), 1f));
					friendhug.Append(friends[index].transform.DOLookAt(new Vector3(boss.transform.position.x, 0f, boss.transform.position.z),0.25f));
				}
                EndSequence1();
			}
		}
	}

    void CameraControl(){
        Camera.main.GetComponent<camerascript>().enabled = false;
        originalCamPos = Camera.main.transform.position;
        originalCamAngles = Camera.main.transform.eulerAngles;
        Camera.main.transform.DOMove(newCam.position, 3f);
        Camera.main.transform.DORotate(newCam.eulerAngles, 3f);
    }

    void EndSequence1(){
        StartCoroutine(PlayAudioInSeq());
        for (int i = 0; i < friendMaterials.Length-2; ++i){
            friendMaterials[i].DOColor(Color.white, 5f).OnComplete(()=>EndSequence2());
        }

    }

    IEnumerator PlayAudioInSeq(){
        for (int i = 0; i < audioController.hugaccepted.Length - 1; ++i)
        {
            audioController.hugaccepted[i].Play();
            yield return new WaitForSeconds(1f);
        }
    }
    void EndSequence2(){

        audioController.hugaccepted[audioController.hugaccepted.Length - 1].Play();
        Camera.main.transform.DOMove(originalCamPos, 3f);
        Camera.main.transform.DORotate(originalCamAngles, 3f);
        friendMaterials[friendMaterials.Length - 2].DOColor(Color.white, 3f);
        FindObjectOfType<bossfriend>().eyelid.DOColor(Color.white, 3f).OnComplete(
            ()=>EndSequence3()); //3s

    }
    void EndSequence3(){
        // FindObjectOfType<bossfriend>().CompleteChange();
        friendMaterials[friendMaterials.Length - 2].DOColor(
            friendMaterials[friendMaterials.Length - 1].color, 3f);
        FindObjectOfType<bossfriend>().eyelid.DOColor(friendMaterials[friendMaterials.Length - 1].color, 3f).OnComplete(
            () => EndSequence4()); //3s
    }
    void EndSequence4(){
        FindObjectOfType<bossfriend>().eyelid.gameObject.SetActive(false);
        FindObjectOfType<bossfriend>().eye.enabled = true;
        StartCoroutine(FinalSequence());
    }

    IEnumerator FinalSequence(){
        
        yield return new WaitForSeconds(3f); //3s
        FindObjectOfType<bossfriend>().eyelid.flipY = true;
        FindObjectOfType<bossfriend>().eyelid.gameObject.SetActive(true);
        //2s

        FindObjectOfType<bossfriend>().GetComponentInChildren<Animator>().SetTrigger("flourish");
        fade.DOColor(new Color(1f, 1f, 1f, 1f), 2f).OnComplete(()=>
                                                               SceneManager.LoadScene("end"));
    }
}
