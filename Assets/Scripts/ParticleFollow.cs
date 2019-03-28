using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollow : MonoBehaviour {

    public Transform target;
    Transform targetParent;
    Rigidbody playerRB;
	// Use this for initialization
	void Start () {
        targetParent = target.parent;
        playerRB = targetParent.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = target.position;
        /*if(playerRB.velocity != Vector3.zero){
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(playerRB.velocity), Time.deltaTime * 10f);
          
        }*/
        if(playerRB.velocity.sqrMagnitude < 2.5f){
            
            GetComponent<ParticleSystem>().Stop();
        } else{
            if(!GetComponent<ParticleSystem>().isPlaying){

                GetComponent<ParticleSystem>().Play();
            }
        }
	}
}
