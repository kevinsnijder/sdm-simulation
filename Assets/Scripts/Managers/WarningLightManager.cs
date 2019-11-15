﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLightManager : MonoBehaviour
{

    WarningLight vesselWarningLight = new WarningLight() { Name = "vessel/warning_light", Status = WarningLightStatus.Off };
    WarningLight trackWarningLight = new WarningLight() { Name = "track/warning_light", Status = WarningLightStatus.Off };

    #region SINGLETON PATTERN
    public static WarningLightManager _instance;
    public static WarningLightManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<WarningLightManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("WarningSingleton");
                    _instance = container.AddComponent<WarningLightManager>();
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
        if(vesselWarningLight.UpdateRequired)
        {
            UpdateSprite(vesselWarningLight);
        }
        if (trackWarningLight.UpdateRequired)
        {
            UpdateSprite(trackWarningLight);
        }
    }

    private void UpdateSprite(WarningLight warningLight)
    {
        warningLight.UpdateRequired = false;
        var gameObject = GameObject.Find(warningLight.Name);
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i).gameObject;
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            switch (warningLight.Status)
            {
                case WarningLightStatus.Off:
                    spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/warningOff");
                    break;
                case WarningLightStatus.Flashing:
                    spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/WarningOn");
                    break;

            }
        }
    }

    /// <summary>
    /// Updates a  warning light to the preferred status
    /// </summary>
    /// <param name="lightName">Ex. vessel/warning_light</param>
    /// <param name="status">Status of the light</param>
    internal void UpdateWarningLight(string lightName, WarningLightStatus status, string laneType)
    {
        if (laneType == LaneType.Vessel)
        {
            vesselWarningLight.Status = status;
            vesselWarningLight.UpdateRequired = true;
        }
        if (laneType == LaneType.Track)
        {
            trackWarningLight.Status = status;
            trackWarningLight.UpdateRequired = true;
        }
    }

    /// <summary>
    /// Gets the status of a light
    /// </summary>
    /// <param name="lightName">Ex. motorised/6/traffic_light/0</param>
    /// <returns></returns>
    internal WarningLightStatus CheckLightStatus(string lightName)
    {
        if(lightName == vesselWarningLight.Name)
        {
            return vesselWarningLight.Status;
        }
        else if(lightName == trackWarningLight.Name)
        {
            return trackWarningLight.Status;
        }
        else
        {
            return WarningLightStatus.Off;
        }
    }
}