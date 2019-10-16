using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class MqttManager : MonoBehaviour
{
    public string brokerHostname = "arankieskamp.com";
    public int teamId = 10;


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
    }

    public void Publish(string _topic, string msg)
    {
        string topic = teamId + "/" + _topic;
        Debug.Log("Publishing message: \"" + msg + "\" to  \"" + topic + "\"");
        client.Publish(
            topic, Encoding.UTF8.GetBytes(msg),
            MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
    }
}
