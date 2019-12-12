using UnityEngine;

/// <summary>
/// Used to take control of all the special objects in the scene like bridges and barriers
/// </summary>
public class SpecialObjectManager : MonoBehaviour
{
    #region Public variables

    #endregion

    #region Private variables

    private MqttManager MqttManager;
    private readonly Deck Deck = new Deck() { Name = "vessel/0/deck/0", Status = DeckStatus.Closed };
    private readonly Barrier TrackBarriers = new Barrier() { Name = "track/0/barrier/0", Status = BarrierStatus.Open };
    private readonly WarningLight TrackWarningLight = new WarningLight() { Name = "track/0/warning_light/0", Status = WarningLightStatus.Off };
    private readonly Barrier VesselBarriers = new Barrier() { Name = "vessel/0/barrier/0", Status = BarrierStatus.Open };
    private readonly WarningLight VesselWarningLight = new WarningLight() { Name = "vessel/0/warning_light/0", Status = WarningLightStatus.Off };
    private int TotalVehiclesOnDeck = 0;
    private int TotalBoatsUnderneathBridge = 0;

    #endregion Private variables

    #region Singleton pattern

    public static SpecialObjectManager _instance;

    public static SpecialObjectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SpecialObjectManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("WarningSingleton");
                    _instance = container.AddComponent<SpecialObjectManager>();
                }
            }

            return _instance;
        }
    }

    #endregion Singleton pattern

    #region Public methods

    /// <summary>
    /// Adds a vehicle to the total count of vehicles on top of the deck
    /// </summary>
    public void AddVehicleToDeck()
    {
        if(TotalVehiclesOnDeck == 0)
        {
            MqttManager.Publish("vessel/0/sensor/3", "1");
        }
        TotalVehiclesOnDeck++;
    }

    /// <summary>
    /// Removes a vehicle from the total count of vehicles on top of the deck
    /// </summary>
    public void RemoveVehicleFromDeck()
    {
        TotalVehiclesOnDeck--;
        if (TotalVehiclesOnDeck == 0)
        {
            MqttManager.Publish("vessel/0/sensor/3", "0");
        }
    }

    /// <summary>
    /// Adds a boat to the total boat count underneath the deck
    /// </summary>
    public void AddBoatUnderneathDeck()
    {
        if(TotalBoatsUnderneathBridge == 0)
        {
            Debug.Log("Not safe to close bridge");
            MqttManager.Publish("vessel/0/sensor/1", "1");
        }
        TotalBoatsUnderneathBridge++;
    }

    /// <summary>
    /// Removes a boat from the total boat count underneath the deck
    /// </summary>
    public void RemoveBoatUnderneathDeck()
    {
        TotalBoatsUnderneathBridge--;
        if (TotalBoatsUnderneathBridge == 0)
        {
            Debug.Log("Safe to close bridge");
            MqttManager.Publish("vessel/0/sensor/1", "0");
        }
    }

    /// <summary>
    /// Gets the status of a light
    /// </summary>
    /// <param name="lightName">Ex. motorised/6/traffic_light/0</param>
    /// <returns></returns>
    public WarningLightStatus CheckLightStatus(string lightName)
    {
        if (lightName == VesselWarningLight.Name)
        {
            return VesselWarningLight.Status;
        }
        else if (lightName == TrackWarningLight.Name)
        {
            return TrackWarningLight.Status;
        }
        else
        {
            return WarningLightStatus.Off;
        }
    }

    /// <summary>
    /// Updates a barrier to the preferred status
    /// </summary>
    /// <param name="lightName">Ex. vessel/warning_light</param>
    /// <param name="status">Status of the barrier</param>
    public void UpdateBarriers(BarrierStatus status, string laneType)
    {
        if (laneType == LaneType.Vessel)
        {
            VesselBarriers.Status = status;
            VesselBarriers.UpdateRequired = true;
        }
        if (laneType == LaneType.Track)
        {
            TrackBarriers.Status = status;
            TrackBarriers.UpdateRequired = true;
        }
    }

    /// <summary>
    /// Updates a deck to the preferred status
    /// </summary>
    /// <param name="lightName">Ex. vessel/warning_light</param>
    /// <param name="status">Status of the barrier</param>
    public void UpdateDeck(DeckStatus status)
    {
        Deck.Status = status;
        Deck.UpdateRequired = true;
    }

    /// <summary>
    /// Updates a  warning light to the preferred status
    /// </summary>
    /// <param name="lightName">Ex. vessel/warning_light</param>
    /// <param name="status">Status of the light</param>
    public void UpdateWarningLight(WarningLightStatus status, string laneType)
    {
        if (laneType == LaneType.Vessel)
        {
            VesselWarningLight.Status = status;
            VesselWarningLight.UpdateRequired = true;
        }
        if (laneType == LaneType.Track)
        {
            TrackWarningLight.Status = status;
            TrackWarningLight.UpdateRequired = true;
        }
    }

    #endregion Public methods

    #region Private methods

    // Start is called before the first frame update
    private void Start()
    {
        this.MqttManager = MqttManager.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (VesselWarningLight.UpdateRequired)
        {
            UpdateSprite(VesselWarningLight);
        }
        if (TrackWarningLight.UpdateRequired)
        {
            UpdateSprite(TrackWarningLight);
        }
        if (TrackBarriers.UpdateRequired)
        {
            UpdateSprite(TrackBarriers);
        }
        if (VesselBarriers.UpdateRequired)
        {
            UpdateSprite(VesselBarriers);
        }
        if (Deck.UpdateRequired)
        {
            UpdateSprite(Deck);
        }
    }

    /// <summary>
    /// Updates the sprite for a group of warning lights
    /// </summary>
    /// <param name="warningLight"></param>
    private void UpdateSprite(WarningLight warningLight)
    {
        warningLight.UpdateRequired = false;
        var gameObject = GameObject.Find(warningLight.Name);
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i).gameObject;
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            switch (warningLight.Status)
            {
                case WarningLightStatus.Off:
                    spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/warningOff");
                    break;

                case WarningLightStatus.Flashing:
                    spriteRenderer.sprite = Resources.Load<Sprite>("Images/Lights/WarningOn");
                    break;
            }
        }
    }

    /// <summary>
    /// Updates the sprite for a group of barriers
    /// </summary>
    /// <param name="barrier"></param>
    private void UpdateSprite(Barrier barrier)
    {
        barrier.UpdateRequired = false;
        var gameObject = GameObject.Find(barrier.Name);
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i).gameObject;
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            switch (barrier.Status)
            {
                case BarrierStatus.Open:
                    spriteRenderer.sprite = Resources.Load<Sprite>("Images/Barriers/barrierOpen");
                    break;

                case BarrierStatus.Closed:
                    spriteRenderer.sprite = Resources.Load<Sprite>("Images/Barriers/barrierClosed");
                    break;
            }
        }
    }

    /// <summary>
    /// Updates the sprite of a bridge
    /// </summary>
    /// <param name="deck"></param>
    private void UpdateSprite(Deck deck)
    {
        deck.UpdateRequired = false;
        var gameObject = GameObject.Find(deck.Name);
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        switch (deck.Status)
        {
            case DeckStatus.Open:
                spriteRenderer.sprite = Resources.Load<Sprite>("Images/Deck/deckOpen");
                break;

            case DeckStatus.Closed:
                spriteRenderer.sprite = Resources.Load<Sprite>("Images/Deck/deckClosed");
                break;
        }
    }

    #endregion Private methods
}