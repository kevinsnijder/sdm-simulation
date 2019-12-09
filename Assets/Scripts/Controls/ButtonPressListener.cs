using UnityEngine;

public class ButtonPressListener : MonoBehaviour
{
    private bool BackspacePressed = false;
    private MqttManager MqttManager;
    private TrafficSpawnManager TrafficSpawnManager;

    // Start is called before the first frame update
    private void Start()
    {
        this.TrafficSpawnManager = TrafficSpawnManager.Instance;
        this.MqttManager = MqttManager.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!BackspacePressed)
            {
                MqttManager.Publish("track/0/warning_light/0", "1");
                MqttManager.Publish("vessel/0/warning_light/0", "1");

                MqttManager.Publish("vessel/0/boat_light/0", "1");
                MqttManager.Publish("vessel/0/boat_light/1", "1");

                MqttManager.Publish("vessel/0/barrier/0", "1");
                MqttManager.Publish("track/0/barrier/0", "1");

                MqttManager.Publish("track/0/deck/0", "1");
            }
            else
            {
                MqttManager.Publish("track/0/warning_light/0", "0");
                MqttManager.Publish("vessel/0/warning_light/0", "0");

                MqttManager.Publish("vessel/0/boat_light/0", "0");
                MqttManager.Publish("vessel/0/boat_light/1", "0");

                MqttManager.Publish("vessel/0/barrier/0", "0");
                MqttManager.Publish("track/0/barrier/0", "0");

                MqttManager.Publish("track/0/deck/0", "0");
            }
            BackspacePressed = !BackspacePressed;
        }
        if (Input.GetKeyDown("space"))
        {
            TrafficSpawnManager.SpawnRandom();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            MqttManager.Publish("motorised/0/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MqttManager.Publish("motorised/1/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MqttManager.Publish("motorised/2/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MqttManager.Publish("motorised/3/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MqttManager.Publish("motorised/4/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            MqttManager.Publish("motorised/5/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            MqttManager.Publish("motorised/6/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            MqttManager.Publish("motorised/7/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            MqttManager.Publish("motorised/8/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            MqttManager.Publish("vessel/0/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            MqttManager.Publish("vessel/1/null/traffic_light/0", "2");
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            MqttManager.Publish("cycle/0/traffic_light/0", "2");
            MqttManager.Publish("cycle/1/traffic_light/0", "2");
            MqttManager.Publish("cycle/2/traffic_light/0", "2");
            MqttManager.Publish("cycle/3/traffic_light/0", "2");
            MqttManager.Publish("cycle/3/traffic_light/1", "2");
            MqttManager.Publish("cycle/4/traffic_light/0", "2");
            MqttManager.Publish("cycle/4/traffic_light/1", "2");
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            MqttManager.Publish("track/0/train_light/0", "1");
            MqttManager.Publish("track/0/train_light/1", "1");
        }
    }
}