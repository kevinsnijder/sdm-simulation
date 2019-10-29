using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightManager : MonoBehaviour
{
    List<TrafficLight> lights = new List<TrafficLight>(){ new TrafficLight() { Name = "motorised/6/traffic_light/0", Status = TrafficLightStatus.Red },
    new TrafficLight() { Name = "motorised/8/traffic_light/0", Status = TrafficLightStatus.Red }};

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
        foreach (TrafficLight light in lights)
        {
            if (light.UpdateRequired)
            {
                light.UpdateRequired = false;
                var gameObject = GameObject.Find(light.Name);
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                switch (light.Status)
                {
                    case TrafficLightStatus.Green:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorisedGreen");
                        break;
                    case TrafficLightStatus.Red:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorisedRed");
                        break;
                    case TrafficLightStatus.Orange:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/MotorisedOrange");
                        break;
                    case TrafficLightStatus.Off:
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
    internal void UpdateMotorisedLight(string lightName, TrafficLightStatus status)
    {
        lights.Find(a => a.Name == lightName).Status = status;
        lights.Find(a => a.Name == lightName).UpdateRequired = true;
    }

    /// <summary>
    /// Gets the status of a light
    /// </summary>
    /// <param name="lightName">Ex. motorised/6/traffic_light/0</param>
    /// <returns></returns>
    internal TrafficLightStatus CheckLightStatus(string lightName)
    {
        TrafficLight light = lights.Find(a => a.Name == lightName);
        if (light != null)
        {
            return light.Status;
        }
        return TrafficLightStatus.Off;
    }
}
