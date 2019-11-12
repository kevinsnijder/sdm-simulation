using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages traffic light statuses and updates them with correct sprites
/// </summary>
public class TrafficLightManager : MonoBehaviour
{
    List<TrafficLight> motorisedLights = new List<TrafficLight>(){ // Whoops hardcoded lights, gotta fix this some time
        new TrafficLight() { Name = "motorised/0/null/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/1/0/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/1/1/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/2/null/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/3/null/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/4/null/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/5/0/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/5/1/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/6/null/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/7/null/traffic_light/0", Status = LightStatus.Red },
        new TrafficLight() { Name = "motorised/8/null/traffic_light/0", Status = LightStatus.Red },
    };

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
        foreach (TrafficLight light in motorisedLights)
        {
            if (light.UpdateRequired)
            {
                light.UpdateRequired = false;
                var gameObject = GameObject.Find(light.Name);
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                switch (light.Status)
                {
                    case LightStatus.Green:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorisedGreen");
                        break;
                    case LightStatus.Red:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorisedRed");
                        break;
                    case LightStatus.Orange:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorisedOrange");
                        break;
                    case LightStatus.Off:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorisedOff");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Updates a light to the preferred status
    /// </summary>
    /// <param name="lightName">Ex. motorised/6/traffic_light/0</param>
    /// <param name="status">Status of the light</param>
    internal void UpdateMotorisedLight(string lightName, LightStatus status)
    {
        motorisedLights.Find(a => a.Name == lightName).Status = status;
        motorisedLights.Find(a => a.Name == lightName).UpdateRequired = true;
    }

    /// <summary>
    /// Gets the status of a light
    /// </summary>
    /// <param name="lightName">Ex. motorised/6/traffic_light/0</param>
    /// <returns></returns>
    internal LightStatus CheckLightStatus(string lightName)
    {
        TrafficLight light = motorisedLights.Find(a => a.Name == lightName.ToLower());
        if (light != null)
        {
            return light.Status;
        }
        return LightStatus.Off;
    }
}
