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
    public List<GameObject> FootSpawnPaths;
    public List<GameObject> FootRespawnPaths;
    public GameObject FootPrefab;

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
        int r = rnd.Next(31);

        if (r < 15)
            SpawnRandomMotorised();
        else if (r > 14 && r < 23)
            SpawnRandomCycle();
        else if (r > 22 && r < 29)
            SpawnRandomFoot();
        else if (r > 28 && r < 30)
            SpawnRandomVessel();
        else if (!TrainHasSpawned)
        {
            TrainHasSpawned = true;
            SpawnRandomTrain();
        }
    }

    #endregion Public methods

    #region Private methods
    private float GetRandomSpeed(float min, float max)
    {
        var temp = rnd.Next(0, 2);
        if (temp == 0)
            return min;
        if (temp == 1)
            return min + ((max - min) / 2);
        if (temp == 2)
            return max;
        else
            return max;
    }

    private void SpawnRandomCycle()
    {
        var bike = Instantiate(BikePrefab);

        int r = rnd.Next(BikePaths.Count);

        GameObject path = BikePaths[r];
        MovementPath movementPath = path.GetComponent<MovementPath>();
        var bikeMovement = bike.GetComponent<Movement>();
        bikeMovement.Path = movementPath;
        bike.transform.position = movementPath.PathSequence[0].position;
        bikeMovement.Speed = GetRandomSpeed(bikeMovement.MinCycleSpeed, bikeMovement.MaxCycleSpeed);
    }

    private void SpawnRandomMotorised()
    {
        var motorised = Instantiate(CarPrefab);

        int r = rnd.Next(CarPaths.Count);

        GameObject path = CarPaths[r];
        MovementPath movementPath = path.GetComponent<MovementPath>();
        motorised.GetComponent<Movement>().Path = movementPath;
        motorised.transform.position = movementPath.PathSequence[0].position;
    }

    private void SpawnRandomFoot()
    {
        var foot = Instantiate(FootPrefab);

        int r = rnd.Next(FootSpawnPaths.Count);

        GameObject path = FootSpawnPaths[r];
        MovementPath movementPath = path.GetComponent<MovementPath>();
        var footMovement = foot.GetComponent<Movement>();
        footMovement.Path = movementPath;
        foot.transform.position = movementPath.PathSequence[0].position;
        footMovement.Speed = GetRandomSpeed(footMovement.MinFootSpeed, footMovement.MaxFootSpeed);
    }

    private void SpawnRandomTrain()
    {
        var train = Instantiate(TrainPrefab);

        int r = rnd.Next(TrainPaths.Count);

        GameObject path = TrainPaths[r];
        MovementPath movementPath = path.GetComponent<MovementPath>();
        train.GetComponent<Movement>().Path = movementPath;
        train.transform.position = movementPath.PathSequence[0].position;
    }

    private void SpawnRandomVessel()
    {
        var vessel = Instantiate(BoatPrefab);

        int r = rnd.Next(BoatPaths.Count);

        GameObject path = BoatPaths[r];
        MovementPath movementPath = path.GetComponent<MovementPath>();
        vessel.GetComponent<Movement>().Path = movementPath;
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