using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to update sensors
/// </summary>
public class SensorManager : MonoBehaviour
{
    #region Private variables

    private MqttManager mqttManager;

    private List<Sensor> sensors = new List<Sensor>(){
        new Sensor() { Name = "motorised/0/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/0/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/1/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/1/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/1/sensor/2", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/1/sensor/3", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/2/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/2/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/3/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/3/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/4/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/4/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/5/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/5/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/5/sensor/2", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/5/sensor/3", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/6/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/6/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/7/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/7/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/8/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "motorised/8/sensor/1", Status = SensorStatus.Deactivated },

        new Sensor() { Name = "cycle/0/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "cycle/1/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "cycle/2/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "cycle/3/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "cycle/3/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "cycle/4/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "cycle/4/sensor/1", Status = SensorStatus.Deactivated },

        new Sensor() { Name = "foot/0/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/1/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/2/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/3/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/4/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/5/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/6/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/0/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/1/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/2/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/3/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/4/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/5/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "foot/6/sensor/1", Status = SensorStatus.Deactivated },

        new Sensor() { Name = "track/0/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "track/0/sensor/1", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "track/0/sensor/2", Status = SensorStatus.Deactivated },

        new Sensor() { Name = "vessel/0/sensor/0", Status = SensorStatus.Deactivated },
        new Sensor() { Name = "vessel/0/sensor/2", Status = SensorStatus.Deactivated },
    };

    #endregion Private variables

    #region Singleton pattern

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

    #endregion Singleton pattern

    #region Public methods

    /// <summary>
    /// Converts the sensor name to a sensor type
    /// </summary>
    /// <param name="sensorname">The name of the sensor to convert</param>
    /// <returns></returns>
    public SensorType GetSensorType(string sensorname)
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

            case "nodedeck":
                return SensorType.DeckBarrierNode;

            case "noderemovedeck":
                return SensorType.RemoveDeckNode;

            case "nodeunderdeck":
                return SensorType.UnderDeckNode;

            case "noderemoveunderdeck":
                return SensorType.RemoveUnderDeckNode;

            case "nodetrackwarning":
                return SensorType.TrackWarningNode;

            default:
                return SensorType.NotASensor;
        }
    }

    /// <summary>
    /// Broadcasts that a sensor is pressed or unpressed
    /// </summary>
    /// <param name="pathName"></param>
    /// <param name="sensor_id"></param>
    /// <param name="sensorstatus"></param>
    public void UpdateSensor(string pathName, int sensor_id, SensorStatus sensorStatus)
    {
        var sensor = sensors.Find(a => a.Name == pathName.ToLower() + "/sensor/" + sensor_id);

        if (sensor != null)
        {
            if (sensor.Status != sensorStatus)
            {
                sensor.Status = sensorStatus;
                mqttManager.Publish(pathName.ToLower() + "/sensor/" + sensor_id, ((int) sensor.Status).ToString());
            }
        } 
        else
        {
            mqttManager.Publish(pathName.ToLower() + "/sensor/" + sensor_id, ((int) sensorStatus).ToString());
        }
    }

    #endregion Public methods

    #region Private methods

    // Start is called before the first frame update
    private void Start()
    {
        this.mqttManager = MqttManager.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private methods
}