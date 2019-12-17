using System;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

/// <summary>
/// Manages all connections with the MQTT server
/// </summary>
public class MqttManager : MonoBehaviour
{
    #region Public variables

    public string BrokerHostname = "arankieskamp.com";
    public int TeamId = 10;

    #endregion Public variables

    #region Private variables

    private MqttClient Client;
    private TrafficLightManager TrafficLightManager;
    private SpecialObjectManager WarningLightManager;

    #endregion Private variables

    #region Singleton pattern

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

    #endregion Singleton pattern

    #region Public methods

    public void Publish(string _topic, string msg)
    {
        string topic = TeamId + "/" + _topic;
        //Debug.Log("Publishing message: \"" + msg + "\" to  \"" + topic);
        Client.Publish(
            topic, Encoding.UTF8.GetBytes(msg),
            MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
    }

    #endregion Public methods

    #region Private methods

    private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string msg = Encoding.UTF8.GetString(e.Message);
        Debug.Log("Received message from " + e.Topic + " : " + msg);

        string topic = e.Topic.Substring(e.Topic.IndexOf('/') + 1);

        // Check if its an update traffic statement
        if (topic.IndexOf(ComponentType.TrafficLight) != -1 || topic.IndexOf(ComponentType.TrainLight) != -1 || topic.IndexOf(ComponentType.BoatLight) != -1)
        {
            if (topic.IndexOf(LaneType.Motorised) != -1 || topic.IndexOf(LaneType.Cycle) != -1 || topic.IndexOf(LaneType.Foot) != -1)
            {
                TrafficLightManager.UpdateLight(topic, (TrafficLightStatus)int.Parse(msg));
            }
            if (topic.IndexOf(LaneType.Vessel) != -1 || topic.IndexOf(LaneType.Track) != -1)
            {
                TrafficLightManager.UpdateAlternativeLight(topic, (BoatTrainLightStatus)int.Parse(msg));
            }
        }

        // Check if its an update warning light statement
        if (topic.IndexOf(ComponentType.WarningLight) != -1)
        {
            if (topic.IndexOf(LaneType.Vessel) != -1)
            {
                WarningLightManager.UpdateWarningLight((WarningLightStatus)int.Parse(msg), LaneType.Vessel);
            }
            if (topic.IndexOf(LaneType.Track) != -1)
            {
                WarningLightManager.UpdateWarningLight((WarningLightStatus)int.Parse(msg), LaneType.Track);
            }
        }

        // Check if its a barrier statement
        if (topic.IndexOf(ComponentType.Barrier) != -1)
        {
            if (topic.IndexOf(LaneType.Vessel) != -1)
            {
                WarningLightManager.UpdateBarriers((BarrierStatus)int.Parse(msg), LaneType.Vessel);
            }
            if (topic.IndexOf(LaneType.Track) != -1)
            {
                WarningLightManager.UpdateBarriers((BarrierStatus)int.Parse(msg), LaneType.Track);
            }
        }

        if (topic.IndexOf(ComponentType.Deck) != -1)
        {
            WarningLightManager.UpdateDeck((DeckStatus)int.Parse(msg));
        }
    }

    private void Connect()
    {
        Debug.Log("About to connect on '" + BrokerHostname + "'");
        Client = new MqttClient(BrokerHostname);
        string clientId = "team-10-simulation";
        try
        {
            Client.Connect(clientId);
            Debug.Log("Success!");
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        TrafficLightManager = TrafficLightManager.Instance;
        WarningLightManager = SpecialObjectManager.Instance;
        Debug.Log("Connecting to " + BrokerHostname);
        Connect();
        Client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
        Client.Subscribe(new string[] { TeamId + "/#" }, qosLevels);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private methods
}