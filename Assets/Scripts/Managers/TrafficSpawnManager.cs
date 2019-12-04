using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Knows where vehicles can spawn
/// </summary>
public class TrafficSpawnManager : MonoBehaviour
{
    #region Public variables
    public List<GameObject> BikePaths;
    public GameObject BikePrefab;
    public List<GameObject> BoatPaths;
    public GameObject BoatPrefab;
    public List<GameObject> CarPaths;
    public GameObject CarPrefab;
    #endregion

    #region Private variables
    private System.Random rnd = new System.Random();
    #endregion

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

    #endregion SINGLETON PATTERN

    #region Public methods
    public void SpawnRandom()
    {
        //SpawnRandomCycle();
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
            //// Track
            //case :
            //    break;
            //// Cycle
            case 11:
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
                //// Foot
                //case :
                //    break;
        }
    }
    #endregion

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

        Debug.Log("Created bike at (" + bike.transform.position.x + ", " + bike.transform.position.y + ", " + bike.transform.position.z + ")");
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

        Debug.Log("Created motorised at (" + motorised.transform.position.x + ", " + motorised.transform.position.y + ", " + motorised.transform.position.z + ")");
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

        Debug.Log("Created boat at (" + vessel.transform.position.x + ", " + vessel.transform.position.y + ", " + vessel.transform.position.z + ")");
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
    #endregion
}