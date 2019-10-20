using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightManager : MonoBehaviour
{
    List<TrafficLight> lights = new List<TrafficLight>(){ new TrafficLight() { Name = "motorised/6", Status = TrafficLightStatus.Red } };

    #region SINGLETON PATTERN
    public static TrafficLightManager _instance;
    public static TrafficLightManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TrafficLightManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("TrafficSingleton");
                    _instance = container.AddComponent<TrafficLightManager>();
                }
            }

            return _instance;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Updates a light to the preferred status
    /// </summary>
    /// <param name="lightName">Ex. motorised/6</param>
    /// <param name="status">Status of the light</param>
    internal void UpdateMotorizedLight(string lightName, TrafficLightStatus status)
    {
        int lightNumber = lights.FindIndex(a => a.Name == lightName);

        TrafficLight light = lights[lightNumber];
        if (light == null)
        {
            throw new Exception("Light does not exist");
        }

        var gameObject = GameObject.Find("TrafficLights/" + lightName);
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        switch (status)
        {
            case TrafficLightStatus.Green:
                light.Status = TrafficLightStatus.Green;
                spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorizedGreen");
                break;
            case TrafficLightStatus.Red:
                light.Status = TrafficLightStatus.Red;
                spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorizedRed");
                break;
            case TrafficLightStatus.Orange:
                light.Status = TrafficLightStatus.Orange;
                spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorizedOrange");
                break;
            case TrafficLightStatus.Off:
                light.Status = TrafficLightStatus.Off;
                spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorizedOff");
                break;
        }
        lights[lightNumber] = light;

    }

    internal TrafficLightStatus CheckLightStatus(string lightName)
    {
        TrafficLight light = lights.Find(a => a.Name == lightName);
        if(light == null)
        {
            throw new Exception("Light does not exist");
        }
        Debug.Log("Light status: " + lightName + " " + light.Status);
        return light.Status;
    }
}
