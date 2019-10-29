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
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            mqttManager.Publish("motorised/6/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            mqttManager.Publish("motorised/8/traffic_light/0", "2");
        }
    }
}
