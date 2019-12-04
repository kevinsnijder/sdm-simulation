using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Used to update sensors
/// </summary>
public class SensorManager : MonoBehaviour
{
    private MqttManager mqttManager;


    #region SINGLETON PATTERN
    public static SensorManager _instance;
    public static SensorManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SensorManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("SensorSingleton");
                    _instance = container.AddComponent<SensorManager>();
                }
            }

            return _instance;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        this.mqttManager = MqttManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Broadcasts that a sensor is pressed or unpressed
    /// </summary>
    /// <param name="pathName"></param>
    /// <param name="sensor"></param>
    /// <param name="sensorstatus"></param>
    internal void UpdateSensor(string pathName, int sensor, int sensorstatus)
    {
        mqttManager.Publish(pathName.ToLower() + "/sensor/" + sensor, sensorstatus.ToString());
    }

    /// <summary>
    /// Converts the sensor name to a sensor type
    /// </summary>
    /// <param name="sensorname">The name of the sensor to convert</param>
    /// <returns></returns>
    internal SensorType GetSensorType(string sensorname)
    {
        sensorname = sensorname.ToLower();

        switch (sensorname)
        {
            case "sensor0":
                return SensorType.FirstSensorNode;
            case "sensor1":
                return SensorType.SecondSensorNode;
            case "sensor2":
                return SensorType.ThirdSensorNode;
            case "sensor3":
                return SensorType.FourthSensorNode;
            case "nodewarning":
                return SensorType.WarningNode;
            default:
                return SensorType.NotASensor;
            
        }
    }
}
