using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    public Camera camera;
    public Transform horizon;
    public bool go = false;

    public float Latitude = 49.1044f;
    public float Longitude = 122.8011f;

    public float[] RightAscension = { 0, 0, 0 };
    public float[] Declination = { 0, 0, 0 };

    public Text field;

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
            Goto();
            go = false;
        }
        string output = "none";
        if(TouchScreenKeyboard.visible) {
            output = "visible, ";
            output += GetKeyboardSize();
        } else {
            output = "not visible, ";
        }
        field.text = output;
    }

    protected void Goto() {
        float rotY = -(RightAscension[0] + RightAscension[1] / 60.0f + RightAscension[2] / 3600.0f) * 15;
        float rotX = -(Mathf.Abs(Declination[0]) + Declination[1] / 60.0f + Declination[2] / 3600.0f) * Mathf.Sign(Declination[0]);

        horizon.rotation = Quaternion.Euler(new Vector3(Latitude, Longitude, 0));
        horizon.transform.position = camera.transform.position - camera.transform.up * 0.1f;

        camera.transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY, 0));
    }

    public int GetKeyboardSize() {
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect")) {
                View.Call("getWindowVisibleDisplayFrame", Rct);

                return Screen.height - Rct.Call<int>("height");
            }
        }
    }

    public void Input_RA_H(InputField input) {
        if(input.text != "") {
            float result = 0;
            if(float.TryParse(input.text, out result)) {
                RightAscension[0] = Mathf.Clamp(result, 0, 24);
                input.text = RightAscension[0].ToString();
            }
        }
    }

    public void Input_RA_M(InputField input) {
        if (input.text != "") {
            float result = 0;
            if (float.TryParse(input.text, out result)) {
                RightAscension[1] = Mathf.Clamp(result, 0, 60);
                input.text = RightAscension[1].ToString();
            }
        }
    }

    public void Input_RA_S(InputField input) {
        if (input.text != "") {
            float result = 0;
            if (float.TryParse(input.text, out result)) {
                RightAscension[2] = Mathf.Clamp(result, 0, 60);
                input.text = RightAscension[2].ToString();
            }
        }
    }

    public void Input_DEC_D(InputField input) {
        if (input.text != "") {
            float result = 0;
            if (float.TryParse(input.text, out result)) {
                Declination[0] = Mathf.Clamp(result, -90, 90);
                input.text = Declination[0].ToString();
            }
        }
    }

    public void Input_DEC_M(InputField input) {
        if (input.text != "") {
            float result = 0;
            if (float.TryParse(input.text, out result)) {
                Declination[1] = Mathf.Clamp(result, 0, 60);
                input.text = Declination[1].ToString();
            }
        }
    }

    public void Input_DEC_S(InputField input) {
        if (input.text != "") {
            float result = 0;
            if (float.TryParse(input.text, out result)) {
                Declination[2] = Mathf.Clamp(result, 0, 60);
                input.text = Declination[2].ToString();
            }
        }
    }

    public void Input_GO() {
        Goto();
    }
}
