using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightManager : MonoBehaviour
{
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
    internal void UpdateLight(string lightName, TrafficLightStatus status)
    {
        var gameObject = GameObject.Find("TrafficLights/" + lightName);
        Texture2D texture = (Texture2D)Resources.Load("Images/Lights/MotorizedGreen");

        var originalsprite = gameObject.GetComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(texture, originalsprite.sprite.rect, originalsprite.sprite.pivot, originalsprite.sprite.pixelsPerUnit);

        gameObject.GetComponent<Image>().overrideSprite = sprite;
    }
}
