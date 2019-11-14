using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

/// <summary>
/// Manages all connections with the MQTT server
/// </summary>
public class MqttManager : MonoBehaviour
{
    public string brokerHostname = "arankieskamp.com";
    public int teamId = 10;
    private TrafficLightManager trafficLightManager;
    private WarningLightManager warningLightManager;


    private MqttClient client;

    #region SINGLETON PATTERN
    public static MqttManager _instance;
    public static MqttManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MqttManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("MqttSingleton");
                    _instance = container.AddComponent<MqttManager>();
                }
            }

            return _instance;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        trafficLightManager = TrafficLightManager.Instance;
        warningLightManager = WarningLightManager.Instance;
        Debug.Log("Connecting to " + brokerHostname);
        Connect();
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
        client.Subscribe(new string[] { teamId + "/#" }, qosLevels);
        Publish("connect", "Simulation Online");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Connect()
    {
        Debug.Log("About to connect on '" + brokerHostname + "'");
        client = new MqttClient(brokerHostname);
        string clientId = Guid.NewGuid().ToString();
        try
        {
            client.Connect(clientId);
            Debug.Log("Success!");
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e);
        }
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string msg = Encoding.UTF8.GetString(e.Message);
        //Debug.Log("Received message from " + e.Topic + " : " + msg);

        string topic = e.Topic.Substring(e.Topic.IndexOf('/')+1);

        // Check if its an update traffic statement
        if (topic.IndexOf(ComponentType.TrafficLight) != -1)
        {
            if(topic.IndexOf(LaneType.Motorised) != -1) 
            {
                trafficLightManager.UpdateMotorisedLight(topic, (TrafficLightStatus)int.Parse(msg));
            }
            if(topic.IndexOf(LaneType.Vessel) != -1) 
            {
                trafficLightManager.UpdateVesselLight(topic, (TrafficLightStatus)int.Parse(msg));
            }
        }

        // Check if its an update warning light statement
        if(topic.IndexOf(ComponentType.WarningLight) != -1)
        {
            if(topic.IndexOf(LaneType.Vessel) != -1)
            {
                warningLightManager.UpdateWarningLight(topic, (WarningLightStatus)int.Parse(msg), LaneType.Vessel);
            }
            if (topic.IndexOf(LaneType.Track) != -1)
            {
                warningLightManager.UpdateWarningLight(topic, (WarningLightStatus)int.Parse(msg), LaneType.Track);
            }
        }
    }

    public void Publish(string _topic, string msg)
    {
        string topic = teamId + "/" + _topic;
        Debug.Log("Publishing message: \"" + msg + "\" to  \"" + topic);
        client.Publish(
            topic, Encoding.UTF8.GetBytes(msg),
            MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
    }
}
