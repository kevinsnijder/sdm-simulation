using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public PathLayout Path; // Reference to Movement Path Used
    private SensorManager sensorManager;
    private TrafficLightManager trafficLightManager;
    private int CurrentNodeId = 0;
    public float MaxDistanceToGoal = .1f; // How close does it have to be to the point to be considered at point
    public float Speed = 7;
    public float RotationSpeedMultiplier = 7; // Easing rotations
    private string PathName;
    private bool PauseDriving = false;
    public float CollisionDistance;
    private int CurrentTrafficLightId;

    // Start is called before the first frame update
    void Start()
    {
        sensorManager = SensorManager.Instance;
        trafficLightManager = TrafficLightManager.Instance;

        //Make sure there is a path assigned
        if (Path == null)
        {
            Debug.LogError("Movement Path cannot be null, I must have a path to follow.", gameObject);
            return;
        }

        PathName = Path.name;
    }

    // Update is called once per frame
    void Update()
    {
        Transform currentNode = Path.PathSequence[CurrentNodeId];

        // Rotate the car
        Vector3 vectorToTarget = currentNode.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * (Speed * RotationSpeedMultiplier));


        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.TransformDirection(Vector3.up), CollisionDistance);
        if (hits.Length > 1)
        {
            PauseDriving = true;
        }

        string lightName = currentNode.parent.parent.parent.name + "/" + PathName + "/traffic_light/" + CurrentTrafficLightId;

        if (!PauseDriving)
        {
            // Move to the next point in path using MoveTowards
            transform.position =
                Vector3.MoveTowards(transform.position,
                                    currentNode.position,
                                    Time.deltaTime * Speed);


            var distanceSquared = (transform.position - currentNode.position).sqrMagnitude;
            if (distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal) //If you are close enough
            {
                if (Path.PathSequence.Length - 1 == CurrentNodeId) // Despawn on the last node of the path
                {
                    Destroy(gameObject);
                }

                string currentSensorName = currentNode.name.ToLower();
                if (currentSensorName == "sensor0" || currentSensorName == "sensor1")
                {
                    int sensor;
                    if (currentSensorName == "sensor0") // TODO: Fix this garbage sensor detection system
                    {
                        sensor = 0;
                    }
                    else
                    {
                        sensor = 1;
                    }

                    string sensorName = currentNode.parent.parent.parent.name + "/" + PathName;
                    sensorManager.UpdateSensor(sensorName, sensor, 1);

                    if(sensor == 0 && trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Red || trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Orange)
                    {
                        PauseDriving = true;
                        return;
                    }
                    else if(sensor==0) // If the light is green -> Drive past the light and update for the possible next light
                    {
                        CurrentTrafficLightId++;
                    }
                }
                CurrentNodeId++; // Get next point in MovementPath
            }
        }
        else
        {
            string currentSensorName = currentNode.name.ToLower();

            if (trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Green && currentSensorName == "sensor0")
            {
                PauseDriving = false;
                CurrentTrafficLightId++;
                CurrentNodeId++;
            }
            if (currentSensorName != "sensor0" && hits.Length == 1)
            {
                PauseDriving = false;
            }
        }
    }
}
