using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class FollowPath : MonoBehaviour
{
    private SensorManager sensorManager;
    private TrafficLightManager trafficLightManager;
    private string pathName;
    private bool pauseDriving = false;

    #region Enums
    public enum MovementType  //Type of Movement
    {
        MoveTowards,
        LerpTowards
    }
    #endregion //Enums

    #region Public Variables
    public MovementType Type = MovementType.MoveTowards; // Movement type used
    public MovementPath MyPath; // Reference to Movement Path Used
    public float Speed = 1; // Speed object is moving
    public float RotationSpeedMultiplier = 7; // Easing rotations
    public float MaxDistanceToGoal = .1f; // How close does it have to be to the point to be considered at point
    #endregion //Public Variables

    #region Private Variables
    private IEnumerator<Transform> pointInPath; //Used to reference points returned from MyPath.GetNextPathPoint
    #endregion //Private Variables

    // (Unity Named Methods)
    #region Main Methods
    public void Start()
    {
        this.sensorManager = SensorManager.Instance;
        this.trafficLightManager = TrafficLightManager.Instance;

        //Make sure there is a path assigned
        if (MyPath == null)
        {
            Debug.LogError("Movement Path cannot be null, I must have a path to follow.", gameObject);
            return;
        }
        this.pathName = MyPath.name;

        //Sets up a reference to an instance of the coroutine GetNextPathPoint
        pointInPath = MyPath.GetNextPathPoint();
        //Get the next point in the path to move to (Gets the Default 1st value)
        pointInPath.MoveNext();

        //Make sure there is a point to move to
        if (pointInPath.Current == null)
        {
            Debug.LogError("A path must have points in it to follow", gameObject);
            return; //Exit Start() if there is no point to move to
        }

        //Set the position of this object to the position of our starting point
        transform.position = pointInPath.Current.position;
    }

    //Update is called by Unity every frame
    public void Update()
    {

        if (pauseDriving)
        {
            string lightname = pointInPath.Current.parent.parent.name + "/" + pathName;
            if(trafficLightManager.CheckLightStatus(lightname) == TrafficLightStatus.Green)
            {
                pauseDriving = false;
            }
            else
            {
                //Thread.Sleep(1000);
                //trafficLightManager.UpdateMotorizedLight(lightname, TrafficLightStatus.Green);
            }
        }
        else
        {
            //Validate there is a path with a point in it
            if (pointInPath == null || pointInPath.Current == null)
            {
                Debug.Log("No path is found");
                return; //Exit if no path is found
            }

            var currentNode = pointInPath.Current;


            //if (Physics.Raycast(transform.position, currentNode.position, 10))
            //    Debug.Log("There is something in front of the object!");

            if (Type == MovementType.MoveTowards) //If you are using MoveTowards movement type
            {
                //Move to the next point in path using MoveTowards
                transform.position =
                    Vector3.MoveTowards(transform.position,
                                        currentNode.position,
                                        Time.deltaTime * Speed);
            }
            else if (Type == MovementType.LerpTowards) //If you are using LerpTowards movement type
            {
                //Move towards the next point in path using Lerp
                transform.position = Vector3.Lerp(transform.position,
                                                    currentNode.position,
                                                    Time.deltaTime * Speed);
            }
            Vector3 vectorToTarget = currentNode.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle-90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * (Speed*RotationSpeedMultiplier));


            var distanceSquared = (transform.position - currentNode.position).sqrMagnitude;
            if (distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal) //If you are close enough
            {
                string currentSensorName = currentNode.name.ToLower();
                if (currentSensorName == "sensor0" || currentSensorName == "sensor1")
                {
                    int sensor;
                    if (currentSensorName == "sensor0")
                    {
                        sensor = 0;
                    }
                    else
                    {
                        sensor = 1;
                    }

                    string lightName = currentNode.parent.parent.name + "/" + pathName;
                    // && als het stoplicht op rood staat OF er een auto voor je stil staat (dus licht aan + 1e sensor ingedrukt)
                    if (trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Red || trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Orange)
                    {
                        if (sensor == 0)
                        {
                            sensorManager.UpdateSensor(lightName, sensor, 1);

                            pauseDriving = true; // debug stop car
                        }

                    }
                }
                pointInPath.MoveNext(); //Get next point in MovementPath
            }
        }
    }
    #endregion //Main Methods

    //(Custom Named Methods)
    #region Utility Methods 

    #endregion //Utility Methods

    //Coroutines run parallel to other fucntions
    #region Coroutines

    #endregion //Coroutines
}
