using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages traffic light statuses and updates them with correct sprites
/// </summary>
public class TrafficLightManager : MonoBehaviour
{
    #region Private variables

    private List<TrafficLight> alternativeLights = new List<TrafficLight>{
        new TrafficLight() { Name = "vessel/0/null/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "vessel/1/null/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "track/0/train_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "track/0/train_light/1", Status = TrafficLightStatus.Red }
    };

    private List<TrafficLight> trafficLights = new List<TrafficLight>(){ // Whoops hardcoded lights, gotta fix this some time
        new TrafficLight() { Name = "motorised/0/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "motorised/1/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "motorised/2/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "motorised/3/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "motorised/4/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "motorised/5/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "motorised/6/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "motorised/7/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "motorised/8/traffic_light/0", Status = TrafficLightStatus.Red },

        new TrafficLight() { Name = "cycle/0/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "cycle/1/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "cycle/2/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "cycle/3/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "cycle/3/traffic_light/1", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "cycle/4/traffic_light/0", Status = TrafficLightStatus.Red },
        new TrafficLight() { Name = "cycle/4/traffic_light/1", Status = TrafficLightStatus.Red }
    };

    #endregion Private variables



    #region Singleton pattern

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

    #endregion Singleton pattern

    #region Public methods

    /// <summary>
    /// Gets the status of a light
    /// </summary>
    /// <param name="lightName">Ex. motorised/6/traffic_light/0</param>
    /// <returns></returns>
    public TrafficLightStatus CheckLightStatus(string lightName)
    {
        var allLights = trafficLights.Concat(alternativeLights).ToList();
        TrafficLight light = allLights.Find(a => a.Name == lightName.ToLower());

        if (light != null)
        {
            return light.Status;
        }
        return TrafficLightStatus.Off;
    }

    /// <summary>
    /// Updates a motorised light to the preferred status
    /// </summary>
    /// <param name="lightName">Ex. motorised/6/traffic_light/0</param>
    /// <param name="status">Status of the light</param>
    public void UpdateLight(string lightName, TrafficLightStatus status)
    {
        trafficLights.Find(a => a.Name == lightName).Status = status;
        trafficLights.Find(a => a.Name == lightName).UpdateRequired = true;

        if (lightName.Contains("cycle/3") || lightName.Contains("cycle/4"))
        {
            trafficLights.Find(a => a.Name == lightName.Replace('0', '1')).Status = status;
            trafficLights.Find(a => a.Name == lightName.Replace('0', '1')).UpdateRequired = true;
        }
    }

    #endregion Public methods

    #region Private methods

    /// <summary>
    /// Updates a light with 2 statuses to the preferred status
    /// </summary>
    /// <param name="lightName">Ex. cycle/0/traffic_light/0</param>
    /// <param name="status">Status of the light</param>
    public void UpdateAlternativeLight(string lightName, TrafficLightStatus status)
    {
        alternativeLights.Find(a => a.Name == lightName).Status = status;
        alternativeLights.Find(a => a.Name == lightName).UpdateRequired = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (TrafficLight light in trafficLights)
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
        foreach (TrafficLight light in alternativeLights)
        {
            if (light.UpdateRequired)
            {
                light.UpdateRequired = false;
                var gameObject = GameObject.Find(light.Name);
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                switch (light.Status)
                {
                    case TrafficLightStatus.Green:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/OtherGreen");
                        break;

                    case TrafficLightStatus.Red:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/OtherRed");
                        break;

                    case TrafficLightStatus.Off:
                        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/OtherOff");
                        break;
                }
            }
        }
    }

    #endregion Private methods
}