using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TrafficSpawnManager : MonoBehaviour
{
    public GameObject CarPrefab;
    public List<GameObject> CarPaths;
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

    public void SpawnRandomCar()
    {
        Debug.Log("Spawning car");

        var car = Instantiate(CarPrefab);

        int r = rnd.Next(CarPaths.Count);

        GameObject path = CarPaths[r];
        PathLayout movementPath = path.GetComponent<PathLayout>();
        car.GetComponent<Drive>().Path = movementPath;
        car.transform.position = movementPath.PathSequence[0].position;

        Debug.Log("Created car at (" + car.transform.position.x + ", " + car.transform.position.y + ", " + car.transform.position.z + ")");
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
