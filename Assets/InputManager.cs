using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public Camera camera;
    public Transform horizon;
    public bool go = false;

    public float Latitude = 49.1044f;
    public float Longitude = 122.8011f;

    public float[] RightAscension = { 0, 0, 0 };
    public float[] Declination = { 0, 0, 0 };

    // Use this for initialization
    void Start () {
        if (RightAscension.GetLength(0) == 0) {
            RightAscension = new float[3];
        }
        if (Declination.GetLength(0) == 0) {
            Declination = new float[3];
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(go) {
            float rotY = -(RightAscension[0] + RightAscension[1] / 60.0f + RightAscension[2] / 3600.0f)*15;
            float rotX = -(Mathf.Abs(Declination[0]) + Declination[1] / 60.0f + Declination[2] / 3600.0f) * Mathf.Sign(Declination[0]);
            
            horizon.rotation = Quaternion.Euler(new Vector3(Latitude, Longitude, 0));
            horizon.transform.position = camera.transform.position - camera.transform.up * 0.1f;

            camera.transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY, 0));
            go = false;
        }
	}
}
