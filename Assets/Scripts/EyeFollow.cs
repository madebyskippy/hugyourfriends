using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeFollow : MonoBehaviour {
    
    public float maxRadius = 0.23f;

    private GameObject player;
    [SerializeField]
    private GameObject eye;
    private float originalZ;
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");
        originalZ = eye.transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 distanceToTarget = player.transform.position - transform.position;
        distanceToTarget = Vector3.ClampMagnitude(distanceToTarget, maxRadius);

        Vector3 finalPos = transform.position + distanceToTarget;
       // finalPos = new Vector3(finalPos.x, finalPos.y, originalZ);
        eye.transform.position = finalPos;
        eye.transform.localPosition = new Vector3(eye.transform.localPosition.x,
                                                  eye.transform.localPosition.y,
                                                  0f);
       
	}
}
