using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Knows where vehicles can spawn
/// </summary>
public class TrafficSpawnManager : MonoBehaviour
{
    public int SpawnCooldownSeconds = 1;

    #region Public variables

    public List<GameObject> BikePaths;
    public GameObject BikePrefab;
    public List<GameObject> BoatPaths;
    public GameObject BoatPrefab;
    public List<GameObject> CarPaths;
    public GameObject CarPrefab;
    public List<GameObject> TrainPaths;
    public GameObject TrainPrefab;

    #endregion Public variables

    internal bool TrainHasSpawned = false;

    #region Private variables

    private System.Random rnd = new System.Random();

    #endregion Private variables

    #region Singleton pattern

    public static TrafficSpawnManager _instance;

    public static TrafficSpawnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TrafficSpawnManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("TrafficSpawnSingleton");
                    _instance = container.AddComponent<TrafficSpawnManager>();
                }
            }

            return _instance;
        }
    }

    #endregion Singleton pattern

    #region Public methods

    public void SpawnRandom()
    {
        int r = rnd.Next(21);

        switch (r)
        {
            // Motorised
            //TODO: Dit vieze spul opruimen
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                SpawnRandomMotorised();
                break;

            // Vessel
            case 20:
                //SpawnRandomVessel();
                break;

            // Track
            case 11:
                if (!TrainHasSpawned)
                {
                    TrainHasSpawned = true;
                    SpawnRandomTrain();
                }
                break;

            // Cycle
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            case 18:
            case 19:
                SpawnRandomCycle();
                break;

                // Foot
                //case :
                //    break;
        }
    }

    #endregion Public methods

    #region Private methods

    private void SpawnRandomCycle()
    {
        Debug.Log("Spawning bike");

        var bike = Instantiate(BikePrefab);

        int r = rnd.Next(BikePaths.Count);

        GameObject path = BikePaths[r];
        PathLayout movementPath = path.GetComponent<PathLayout>();
        bike.GetComponent<Move>().Path = movementPath;
        bike.transform.position = movementPath.PathSequence[0].position;
    }

    private void SpawnRandomMotorised()
    {
        Debug.Log("Spawning motorised");

        var motorised = Instantiate(CarPrefab);

        int r = rnd.Next(CarPaths.Count);

        GameObject path = CarPaths[r];
        PathLayout movementPath = path.GetComponent<PathLayout>();
        motorised.GetComponent<Move>().Path = movementPath;
        motorised.transform.position = movementPath.PathSequence[0].position;
    }

    private void SpawnRandomTrain()
    {
        Debug.Log("Spawning train");

        var train = Instantiate(TrainPrefab);

        int r = rnd.Next(TrainPaths.Count);

        GameObject path = TrainPaths[r];
        PathLayout movementPath = path.GetComponent<PathLayout>();
        train.GetComponent<Move>().Path = movementPath;
        train.transform.position = movementPath.PathSequence[0].position;
    }

    private void SpawnRandomVessel()
    {
        Debug.Log("Spawning vessel");

        var vessel = Instantiate(BoatPrefab);

        int r = rnd.Next(BoatPaths.Count);

        GameObject path = BoatPaths[r];
        PathLayout movementPath = path.GetComponent<PathLayout>();
        vessel.GetComponent<Move>().Path = movementPath;
        vessel.transform.position = movementPath.PathSequence[0].position;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Spawn random
        InvokeRepeating("SpawnRandom", 0f, SpawnCooldownSeconds);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private methods
}