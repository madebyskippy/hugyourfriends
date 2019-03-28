using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class camerascript : MonoBehaviour {

	//code help from hang ruan

	[SerializeField] Camera myCamera;
	[SerializeField] Transform myPlayerTransform;
	float myLerpSpeed=10;
	float myPositionMultiplier;
	float myPositionOffset;
	float myPlayerVision;

	Transform myFriendTransform;

	// Use this for initialization
	void Start () {
		myFriendTransform = null;
	}

	// Update is called once per frame
	void Update () {
		this.transform.position = Vector3.Lerp (this.transform.position, GetCenter (), Time.deltaTime * myLerpSpeed);
	}

	private Vector3 GetCenter () {
		Vector3 t_center = Vector3.zero;

		t_center += myPlayerTransform.position + myPlayerTransform.forward * myPlayerVision;

		if (myFriendTransform != null) {
			t_center += myFriendTransform.position+myFriendTransform.forward*myPlayerVision;
			t_center /= 2;
		}

		return t_center;
	}

	public void setFriend(Transform f){
		myFriendTransform = f;
	}
		
	public void zoomIn(){
		GetComponent<Camera> ().DOOrthoSize (4, 1f).SetEase (Ease.InOutElastic);
	}

	public void zoomOut(){
		GetComponent<Camera> ().DOOrthoSize (10, 1f);
	}
}