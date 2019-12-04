using System;
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
    private string pathName;
    private string lightName;


    // Start is called before the first frame update
    void Start()
    {
        sensorManager = SensorManager.Instance;
        trafficLightManager = TrafficLightManager.Instance;
        warningLightManager = WarningLightManager.Instance;
        pathName = Path.PathSequence[0].parent.parent.parent.name;
        lightName = GetCurrentTrafficlight();

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

        RotateTowardsNode(currentNode);
        if (IsColliding())
        {
            PauseMoving = true;
        }

        // --- Moving brain --- //
        if (!PauseMoving)
        {
            MoveTowardsNode(currentNode);
        }
        else
        {
            // Currently not driving
            string currentNodeName = currentNode.name.ToLower();
            // Check if the light in front of you is green
            if (trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Green && (currentNodeName == "sensor0" || currentNodeName == "sensor2"))
            {
                PauseMoving = false;
            }
            //if(warningLightManager.CheckLightStatus(warningLightName) == WarningLightStatus.Off && currentNodeName == "nodewarning")
            //{
            //    PauseMoving = false;
            //    CurrentNodeId++;
            //}
            // Check if you are not colliding with another car
            if (currentNodeName != "sensor0" && currentNodeName != "sensor2" && !IsColliding())
            {
                PauseMoving = false;
            }
        }
    }

    /// <summary>
    /// Checks if the current object is touching the node
    /// </summary>
    public bool CloseEnoughToNode
    {
        get
        {
            var distanceSquared = (transform.position - Path.PathSequence[CurrentNodeId].position).sqrMagnitude;

            if (distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Gradually moves towards the next node in the path
    /// </summary>
    /// <param name="node"></param>
    private void MoveTowardsNode(Transform node)
    {
        // Move to the next point in path using MoveTowards
        transform.position =
            Vector3.MoveTowards(transform.position,
                                node.position,
                                Time.deltaTime * Speed);

        if (CloseEnoughToNode)
        {
            // Selfdestruct if last node was hit
            if (Path.PathSequence.Length - 1 == CurrentNodeId)
            {
                Destroy(gameObject);
            }

            string currentNodeName = node.name.ToLower();
            SensorType currentSensorType = sensorManager.GetSensorType(currentNodeName);

            if (currentSensorType != SensorType.NotASensor)
            {
                // --- Update sensors and stop driving if required --- //
                string fullSensorName = node.parent.parent.parent.parent.name + "/" + pathName;

                if (currentSensorType != SensorType.WarningNode)
                {
                    sensorManager.UpdateSensor(fullSensorName, (int)currentSensorType, 1);
                }

                if ((currentSensorType == SensorType.FirstSensorNode || currentSensorType == SensorType.ThirdSensorNode) && (trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Red || trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Orange))
                {
                    // Light is red
                    string previoussensorname = node.parent.parent.parent.parent.name + "/" + pathName;
                    sensorManager.UpdateSensor(previoussensorname, 1, 0);
                    PauseMoving = true;
                    return;
                }
                else if (currentSensorType == SensorType.FirstSensorNode || currentSensorType == SensorType.ThirdSensorNode)
                {
                    // Light is green
                    sensorManager.UpdateSensor(fullSensorName, (int)currentSensorType, 0);
                }
                else if (currentSensorType == SensorType.WarningNode)
                {
                    // Warning light is on
                    if (warningLightManager.CheckLightStatus("vessel/warning_light") == WarningLightStatus.Flashing)
                    {
                        PauseMoving = true;
                        return;
                    }
                }
            }
            CurrentNodeId++; // Get next point in MovementPath
        }
    }

    /// <summary>
    /// Gets the name of the trafficlight this object has to check
    /// </summary>
    /// <returns></returns>
    private string GetCurrentTrafficlight()
    {
        Transform currentNode = Path.PathSequence[CurrentNodeId];

        string lightName = currentNode.parent.parent.parent.parent.name + "/" + pathName + "/traffic_light/0";

        // Override lightname when this object is a bike
        if (this.gameObject.name.ToLower().Contains("bike"))
        {
            if (currentNode.parent.parent.name == "path0")
            {
                lightName = currentNode.parent.parent.parent.parent.name + "/" + pathName + "/traffic_light/0";
            }
            else
            {
                lightName = currentNode.parent.parent.parent.parent.name + "/" + pathName + "/traffic_light/1";
            }
        }
        return lightName;
    }

    /// <summary>
    /// Checks if this object is colliding with another object
    /// </summary>
    /// <returns></returns>
    private bool IsColliding()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.TransformDirection(Vector3.up), CollisionDistance);
        if (hits.Length > 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gradually rotates the vehicle to the direction its driving
    /// </summary>
    private void RotateTowardsNode(Transform node)
    {
        Vector3 vectorToTarget = node.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * (Speed * RotationSpeedMultiplier));
    }
}
