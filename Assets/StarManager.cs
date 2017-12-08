using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour {
    public ParticleSystem ps;
    public Camera camera;
    private System.DateTime epochStart = new System.DateTime(2000, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

    protected float rightAscensionOffset = 0;//-(90 - 1) * Mathf.Deg2Rad;
    public float starSphereRadius = 100;
    public float timeSpeed = 1000;
    public float timeOffset = 0;
    public float lastTimeOffset = 0;

    float YearsSinceEpoch(float offset=0) {
        return (float)((System.DateTime.UtcNow - epochStart).TotalDays + offset) / 365.25f;
    }

    Color GetColorFromSpectralType(string isp) {
        float r = 1;
        float g = 1;
        float b = 1;

        char type = isp[0];
        float subClass = 5;
        if (float.TryParse(char.ToString(isp[1]), out subClass)) {
            subClass = subClass / 10f;
            float temp = 0;

            switch (type) {
                default:
                    //White
                    temp = 66;
                    break;
                case ('O'):
                    temp = 250;
                    break;
                case ('B'):
                    temp = 100 + 150 * subClass;
                    break;
                case ('A'):
                    temp = 75 + 25 * subClass;
                    break;
                case ('F'):
                    temp = 60 + 15 * subClass;
                    break;
                case ('G'):
                    temp = 50 + 10 * subClass;
                    break;
                case ('K'):
                    temp = 35 + 15 * subClass;
                    break;
                case ('M'):
                    temp = 20 + 15 * subClass;
                    break;
                case ('C'):
                    temp = 20 * subClass;
                    break;
            }
            if (temp <= 66) {
                r = 255;
                if (temp < 19) {
                    b = 0;
                } else {
                    b = Mathf.Clamp(138.5177312231f * Mathf.Log(temp - 100) - 305.0447927307f, 0, 255);
                }
                g = Mathf.Clamp(99.4708025861f * Mathf.Log(temp) - 161.1195681661f, 0, 255);
            } else {
                r = Mathf.Clamp(329.698727446f * Mathf.Pow((temp - 60), -0.1332047592f), 0, 255);
                g = Mathf.Clamp(288.1221695283f * Mathf.Pow(temp - 60, -0.0755148492f), 0, 255);
                b = 255;
            }
            r = r / 255.0f;
            g = g / 255.0f;
            b = b / 255.0f;
        }
        return new Color(r, g, b);
    }

    void UpdateStars() {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[StarData.data.GetLength(0)];
        ps.GetParticles(particles);
        float yearsSinceEpoch = YearsSinceEpoch(timeOffset);
        for (int i = 0; i < StarData.data.GetLength(0) - 1; i++) {
            string[] data = StarData.data[i].Split(',');

            float RA = -float.Parse(data[1]) + rightAscensionOffset;
            float DEC = float.Parse(data[2]);
            float RA_PM = float.Parse(data[3]);
            float DEC_PM = float.Parse(data[4]);
            //string ISP = data[5];
            //float MAG = float.Parse(data[6]);
            float y = Mathf.Sin(DEC + DEC_PM * yearsSinceEpoch);
            float ydist = Mathf.Cos(DEC + DEC_PM * yearsSinceEpoch);
            float z = Mathf.Cos(RA + RA_PM * yearsSinceEpoch) * ydist;
            float x = Mathf.Sin(RA + RA_PM * yearsSinceEpoch) * ydist;

            particles[i].position = new Vector3(x, y, z) * starSphereRadius;
        }
        ps.SetParticles(particles, StarData.data.GetLength(0)-1);
    }

    // Use this for initialization
    void Start() {
        float yearsSinceEpoch = YearsSinceEpoch();

        for (int i = 0; i < StarData.data.GetLength(0)-1; i++) {
            string[] data = StarData.data[i].Split(',');

            float RA = -float.Parse(data[1]) + rightAscensionOffset;
            float DEC = float.Parse(data[2]);
            float RA_PM = float.Parse(data[3]);
            float DEC_PM = float.Parse(data[4]);
            string ISP = data[5];
            float MAG = float.Parse(data[6]);
            float y = Mathf.Sin(DEC + DEC_PM * yearsSinceEpoch);
            float ydist = Mathf.Cos(DEC + DEC_PM * yearsSinceEpoch);
            float z = Mathf.Cos(RA + RA_PM * yearsSinceEpoch) * ydist;
            float x = Mathf.Sin(RA + RA_PM * yearsSinceEpoch) * ydist;

            Vector3 position = new Vector3(x, y, z) * starSphereRadius;

            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.position = position;
            emitParams.startColor = GetColorFromSpectralType(ISP);
            emitParams.startSize = 1f * (1-(MAG+2)/10.0f);
            ps.Emit(emitParams, 1);
        }
    }

	// Update is called once per frame
	void Update () {
        transform.position = camera.transform.position + camera.transform.forward*0.31f;
        if (timeOffset != lastTimeOffset)
            UpdateStars();
        lastTimeOffset = timeOffset;
        timeOffset += timeSpeed*Time.deltaTime;
    }
}
