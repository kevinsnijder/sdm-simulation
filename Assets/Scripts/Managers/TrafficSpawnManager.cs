using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TrafficSpawnManager : MonoBehaviour
{
    public GameObject CarPrefab;

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
        GameObject path = GameObject.Find("Paths/motorised/6");
        MovementPath movementPath = path.GetComponent<MovementPath>();
        movementPath.movingTo = 0;
        movementPath.movementDirection = 1;
        car.GetComponent<FollowPath>().MyPath = movementPath;

        Debug.Log("Created car at (" + car.transform.position.x + ", " + car.transform.position.y + ", " + car.transform.position.z + ")");
        Debug.Log("Node positions:");
        foreach (Transform c in car.GetComponent<FollowPath>().MyPath.PathSequence)
        {
            Debug.Log("(" + c.position.x + ", " + c.position.y + ", " + c.position.z + ")");
        }

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
