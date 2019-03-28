using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorController : MonoBehaviour {

    public LayerMask borderLayer;
    public LayerMask floorlayer;
    public LayerMask nothingLayer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//if()
	}

    public void IgnoreFloorLayer(bool ignore){
        if (ignore)
        {
            GetComponent<Projector>().ignoreLayers = floorlayer;
        } else{
            GetComponent<Projector>().ignoreLayers = nothingLayer;
        }
    }
}
