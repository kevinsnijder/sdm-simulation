using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Knows where vehicles can spawn
/// </summary>
public class TrafficSpawnManager : MonoBehaviour
{
    public GameObject CarPrefab;
    public GameObject BoatPrefab;
    public List<GameObject> CarPaths;
    public List<GameObject> BoatPaths;
    private System.Random rnd = new System.Random();


    #region SINGLETON PATTERN
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
    #endregion

    public void SpawnRandomCar()
    {
        Debug.Log("Spawning car");

        var car = Instantiate(CarPrefab);

        int r = rnd.Next(CarPaths.Count);

        GameObject path = CarPaths[r];
        PathLayout movementPath = path.GetComponent<PathLayout>();
        car.GetComponent<Move>().Path = movementPath;
        car.transform.position = movementPath.PathSequence[0].position;

        Debug.Log("Created car at (" + car.transform.position.x + ", " + car.transform.position.y + ", " + car.transform.position.z + ")");
    }

    public void SpawnRandomBoat() 
    {
        Debug.Log("Spawning boat");

        var boat = Instantiate(BoatPrefab);

        int r = rnd.Next(BoatPaths.Count);

        GameObject path = BoatPaths[r];
        PathLayout movementPath = path.GetComponent<PathLayout>();
        boat.GetComponent<Move>().Path = movementPath;
        boat.transform.position = movementPath.PathSequence[0].position;

        Debug.Log("Created boat at (" + boat.transform.position.x + ", " + boat.transform.position.y + ", " + boat.transform.position.z + ")");
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
