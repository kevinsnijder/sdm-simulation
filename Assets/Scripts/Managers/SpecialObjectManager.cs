using UnityEngine;

/// <summary>
/// Used to take control of all the special objects in the scene like bridges and barriers
/// </summary>
public class SpecialObjectManager : MonoBehaviour
{
    #region Private variables

    private Deck deck = new Deck() { Name = "vessel/0/deck/0", Status = DeckStatus.Closed };
    private Barrier trackBarriers = new Barrier() { Name = "track/0/barrier/0", Status = BarrierStatus.Open };
    private WarningLight trackWarningLight = new WarningLight() { Name = "track/0/warning_light/0", Status = WarningLightStatus.Off };
    private Barrier vesselBarriers = new Barrier() { Name = "vessel/0/barrier/0", Status = BarrierStatus.Open };
    private WarningLight vesselWarningLight = new WarningLight() { Name = "vessel/0/warning_light/0", Status = WarningLightStatus.Off };

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
    /// Gets the status of a light
    /// </summary>
    /// <param name="lightName">Ex. motorised/6/traffic_light/0</param>
    /// <returns></returns>
    public WarningLightStatus CheckLightStatus(string lightName)
    {
        if (lightName == vesselWarningLight.Name)
        {
            return vesselWarningLight.Status;
        }
        else if (lightName == trackWarningLight.Name)
        {
            return trackWarningLight.Status;
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
            vesselBarriers.Status = status;
            vesselBarriers.UpdateRequired = true;
        }
        if (laneType == LaneType.Track)
        {
            trackBarriers.Status = status;
            trackBarriers.UpdateRequired = true;
        }
    }

    /// <summary>
    /// Updates a deck to the preferred status
    /// </summary>
    /// <param name="lightName">Ex. vessel/warning_light</param>
    /// <param name="status">Status of the barrier</param>
    public void UpdateDeck(DeckStatus status)
    {
        deck.Status = status;
        deck.UpdateRequired = true;
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
            vesselWarningLight.Status = status;
            vesselWarningLight.UpdateRequired = true;
        }
        if (laneType == LaneType.Track)
        {
            trackWarningLight.Status = status;
            trackWarningLight.UpdateRequired = true;
        }
    }

    #endregion Public methods

    #region Private methods

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (vesselWarningLight.UpdateRequired)
        {
            UpdateSprite(vesselWarningLight);
        }
        if (trackWarningLight.UpdateRequired)
        {
            UpdateSprite(trackWarningLight);
        }
        if (trackBarriers.UpdateRequired)
        {
            UpdateSprite(trackBarriers);
        }
        if (vesselBarriers.UpdateRequired)
        {
            UpdateSprite(vesselBarriers);
        }
        if (deck.UpdateRequired)
        {
            UpdateSprite(deck);
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