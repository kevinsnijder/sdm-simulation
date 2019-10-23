using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ButtonPressListener : MonoBehaviour
{
    TrafficSpawnManager trafficSpawnManager;

    // Start is called before the first frame update
    void Start()
    {
        this.trafficSpawnManager = TrafficSpawnManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            print("space key was pressed");
            trafficSpawnManager.SpawnRandomCar();
        }
    }
}
