using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// This class is the brain of a path user
/// </summary>
public class Move : MonoBehaviour
{
    public PathLayout Path; // Reference to Movement Path Used
    private SensorManager sensorManager;
    private TrafficLightManager trafficLightManager;
    private WarningLightManager warningLightManager;
    private int CurrentNodeId = 0;
    public float MaxDistanceToGoal = .1f; // How close does it have to be to the point to be considered at point
    public float Speed;
    public float RotationSpeedMultiplier = 7; // Easing rotations
    private bool PauseMoving = false;
    public float CollisionDistance;

    // Start is called before the first frame update
    void Start()
    {
        sensorManager = SensorManager.Instance;
        trafficLightManager = TrafficLightManager.Instance;
        warningLightManager = WarningLightManager.Instance;

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

        // --- Vehicle rotation --- //
        Vector3 vectorToTarget = currentNode.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * (Speed * RotationSpeedMultiplier));

        // --- Hit registration --- //
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.TransformDirection(Vector3.up), CollisionDistance);
        if (hits.Length > 1)
        {
            PauseMoving = true;
        }

        // --- Path string builder --- //
        string pathName = currentNode.parent.parent.parent.name + "/" + currentNode.parent.parent.name;
        string lightName = currentNode.parent.parent.parent.parent.name + "/" + pathName + "/traffic_light/0";
        string warningLightName = "vessel" + "/" + "warning_light";

        // --- Moving brain --- //
        if (!PauseMoving)
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
                string currentNodeName = currentNode.name.ToLower();
                if (currentNodeName == "sensor0" || currentNodeName == "sensor1" || currentNodeName == "nodewarning")
                {
                    Sensor sensor = Sensor.NotASensor;
                    if (currentNodeName == "sensor0") // TODO: Fix this garbage sensor detection system
                    {
                        sensor = Sensor.FirstSensorNode;
                    }
                    else if(currentNodeName == "sensor1")
                    {
                        sensor = Sensor.SecondSensorNode;
                    }
                    else if(currentNodeName == "nodewarning")
                    {
                        sensor = Sensor.WarningNode;
                    }

                    // --- Keep driving if no sensor or warning light --- //
                    if (sensor!=Sensor.NotASensor)
                    {
                        // --- Update sensors and stop driving if required --- //
                        string sensorName = currentNode.parent.parent.parent.parent.name + "/" + pathName;

                        sensorManager.UpdateSensor(sensorName, (int)sensor, 1);

                        if (sensor == Sensor.FirstSensorNode && trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Red || trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Orange)
                        {
                            // Light is red
                            string previoussensorname = currentNode.parent.parent.parent.parent.name + "/" + pathName;
                            sensorManager.UpdateSensor(previoussensorname, 1, 0);
                            PauseMoving = true;
                            return;
                        }
                        else if (sensor == Sensor.FirstSensorNode)
                        {
                            // Light is green
                            sensorManager.UpdateSensor(sensorName, (int)sensor, 0);
                        }
                        else if (sensor == Sensor.WarningNode)
                        {
                            //Warning light is on
                            if(warningLightManager.CheckLightStatus(warningLightName) == WarningLightStatus.Flashing)
                            {
                                PauseMoving = true;
                                return;
                            }
                        }
                    }
                }
                CurrentNodeId++; // Get next point in MovementPath
            }
        }
        else
        {
            // Currently not driving
            string currentNodeName = currentNode.name.ToLower();

            // Check if the light in front of you is green
            if (trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Green && currentNodeName == "sensor0")
            {
                PauseMoving = false;
                CurrentNodeId++;
            }
            if(warningLightManager.CheckLightStatus(warningLightName) == WarningLightStatus.Off && currentNodeName == "nodewarning")
            {
                PauseMoving = false;
                CurrentNodeId++;
            }
            // Check if you are not colliding with another car
            if (currentNodeName != "sensor0" && hits.Length == 1)
            {
                PauseMoving = false;
            }
        }
    }
}
