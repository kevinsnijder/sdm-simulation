using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// This class is the brain of a car driver
/// </summary>
public class Drive : MonoBehaviour
{
    public PathLayout Path; // Reference to Movement Path Used
    private SensorManager sensorManager;
    private TrafficLightManager trafficLightManager;
    private int CurrentNodeId = 0;
    public float MaxDistanceToGoal = .1f; // How close does it have to be to the point to be considered at point
    public float Speed = 7;
    public float RotationSpeedMultiplier = 7; // Easing rotations
    private bool PauseDriving = false;
    public float CollisionDistance;

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
    }

    // Update is called once per frame
    void Update()
    {
        Transform currentNode = Path.PathSequence[CurrentNodeId];

        // --- Car rotation --- //
        Vector3 vectorToTarget = currentNode.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * (Speed * RotationSpeedMultiplier));

        // --- Hit registration --- //
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.TransformDirection(Vector3.up), CollisionDistance);
        if (hits.Length > 1)
        {
            PauseDriving = true;
        }

        // --- Path string builder --- //
        string pathName = currentNode.parent.parent.parent.name + "/" + currentNode.parent.parent.name;
        string lightName = currentNode.parent.parent.parent.parent.name + "/" + pathName + "/traffic_light/0";

        // --- Driving brain --- //
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
                // --- Selfdestruct if last node was hit--- //
                if (Path.PathSequence.Length - 1 == CurrentNodeId)
                {
                    Destroy(gameObject);
                }

                // --- Do stuff based on the node that was hit --- //
                string currentSensorName = currentNode.name.ToLower();
                if (currentSensorName == "sensor0" || currentSensorName == "sensor1" || currentSensorName == "warningsensor")
                {
                    Sensor sensor = Sensor.NotASensor;
                    if (currentSensorName == "sensor0") // TODO: Fix this garbage sensor detection system
                    {
                        sensor = Sensor.FirstSensorNode;
                    }
                    else if(currentSensorName == "sensor1")
                    {
                        sensor = Sensor.SecondSensorNode;
                    }
                    else if(currentSensorName == "warningsensor")
                    {
                        sensor = Sensor.WarningNode;
                    }

                    // --- Keep driving if no sensor or warning light --- //
                    if (sensor!=Sensor.NotASensor)
                    {
                        // --- Update sensors and stop driving if required --- //
                        string sensorName = currentNode.parent.parent.parent.parent.name + "/" + pathName;

                        sensorManager.UpdateSensor(sensorName, (int)sensor, 1);

                        if (sensor == Sensor.FirstSensorNode && trafficLightManager.CheckLightStatus(lightName) == LightStatus.Red || trafficLightManager.CheckLightStatus(lightName) == LightStatus.Orange)
                        {
                            // Light is red
                            string previoussensorname = currentNode.parent.parent.parent.parent.name + "/" + pathName;
                            sensorManager.UpdateSensor(previoussensorname, 1, 0);
                            PauseDriving = true;
                            return;
                        }
                        else if (sensor == Sensor.FirstSensorNode)
                        {
                            // Light is green
                            sensorManager.UpdateSensor(sensorName, (int)sensor, 0);
                        }
                        else if (sensor == Sensor.WarningNode)
                        {
                            // TODO: Check if warning light is on
                        }
                    }
                }
                CurrentNodeId++; // Get next point in MovementPath
            }
        }
        else
        {
            // Currently not driving
            string currentSensorName = currentNode.name.ToLower();

            // Check if the light in front of you is green
            if (trafficLightManager.CheckLightStatus(lightName) == LightStatus.Green && currentSensorName == "sensor0")
            {
                PauseDriving = false;
                CurrentNodeId++;
            }
            // Check if you are not colliding with another car
            if (currentSensorName != "sensor0" && hits.Length == 1)
            {
                PauseDriving = false;
            }
        }
    }
}
