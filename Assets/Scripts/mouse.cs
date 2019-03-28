using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouse : MonoBehaviour {

	[SerializeField] Slider slider;
	float val;

	// Use this for initialization
	void Start () {
		val = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
			float scroll = Input.GetAxis ("Mouse ScrollWheel");
			scroll *= 0.1f;
			val = Mathf.Clamp (val + scroll, 0f, 1f);
		} else {
			val = Mathf.Max (0f, val - 0.001f);
		}

		for (int i = 0; i < 10; i++) {
			if (Input.GetKeyDown (KeyCode.Alpha0 + i)) {
				val = i / 10f + 0.095f;
			}
		}

		slider.value = val;
	}

	public float getVal(){
		return val;
	}
}
