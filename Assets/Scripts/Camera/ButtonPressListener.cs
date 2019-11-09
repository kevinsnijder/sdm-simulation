using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ButtonPressListener : MonoBehaviour
{
    TrafficSpawnManager trafficSpawnManager;
    MqttManager mqttManager;


    // Start is called before the first frame update
    void Start()
    {
        this.trafficSpawnManager = TrafficSpawnManager.Instance;
        this.mqttManager = MqttManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            trafficSpawnManager.SpawnRandomCar();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            mqttManager.Publish("motorised/0/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mqttManager.Publish("motorised/1/0/traffic_light/0", "2");
            mqttManager.Publish("motorised/1/1/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mqttManager.Publish("motorised/2/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            mqttManager.Publish("motorised/3/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            mqttManager.Publish("motorised/4/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            mqttManager.Publish("motorised/5/0/traffic_light/0", "2");
            mqttManager.Publish("motorised/5/1/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            mqttManager.Publish("motorised/6/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            mqttManager.Publish("motorised/7/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            mqttManager.Publish("motorised/8/null/traffic_light/0", "2");
        }
    }
}
