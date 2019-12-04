using UnityEngine;

public class ButtonPressListener : MonoBehaviour
{
    private bool backspacePressed = false;
    private MqttManager mqttManager;
    private TrafficSpawnManager trafficSpawnManager;

    // Start is called before the first frame update
    private void Start()
    {
        this.trafficSpawnManager = TrafficSpawnManager.Instance;
        this.mqttManager = MqttManager.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!backspacePressed)
            {
                mqttManager.Publish("vessel/warning_light", "1");
            }
            else
            {
                mqttManager.Publish("vessel/warning_light", "0");
            }
            backspacePressed = !backspacePressed;
        }
        if (Input.GetKeyDown("space"))
        {
            trafficSpawnManager.SpawnRandom();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            mqttManager.Publish("motorised/0/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mqttManager.Publish("motorised/1/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mqttManager.Publish("motorised/2/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            mqttManager.Publish("motorised/3/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            mqttManager.Publish("motorised/4/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            mqttManager.Publish("motorised/5/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            mqttManager.Publish("motorised/6/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            mqttManager.Publish("motorised/7/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            mqttManager.Publish("motorised/8/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            mqttManager.Publish("vessel/0/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            mqttManager.Publish("vessel/1/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            mqttManager.Publish("cycle/0/traffic_light/0", "2");
            mqttManager.Publish("cycle/1/traffic_light/0", "2");
            mqttManager.Publish("cycle/2/traffic_light/0", "2");
            mqttManager.Publish("cycle/3/traffic_light/0", "2");
            mqttManager.Publish("cycle/3/traffic_light/1", "2");
            mqttManager.Publish("cycle/4/traffic_light/0", "2");
            mqttManager.Publish("cycle/4/traffic_light/1", "2");
        }
    }
}