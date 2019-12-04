using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class is the brain of a path user
/// </summary>
public class Move : MonoBehaviour
{
    #region Public variables
    public float CollisionDistance;
    public float MaxDistanceToGoal = .1f;
    public PathLayout Path;
    public float RotationSpeedMultiplier = 7;
    public float Speed;
    #endregion

    #region Private variables
    private Transform currentNode;
    private int CurrentNodeId = 0;
    private string lightName;
    private string pathName;
    private bool PauseMoving = false;
    private bool previousNodeUnpressed = true;
    private SensorManager sensorManager;
    private TrafficLightManager trafficLightManager;
    private WarningLightManager warningLightManager;
    #endregion

    #region Properties
    /// <summary>
    /// Checks if the current object is touching the node
    /// </summary>
    private bool CloseEnoughToCurrentNode
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
    /// Checks if the current object is touching the previous node
    /// </summary>
    private bool CloseEnoughToPreviousNode
    {
        get
        {
            var distanceSquared = (transform.position - Path.PathSequence[CurrentNodeId - 1].position).sqrMagnitude;

            if (distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// The type of the sensor the object is currently moving towards
    /// </summary>
    private SensorType CurrentSensorType
    {
        get
        {
            return sensorManager.GetSensorType(currentNode.name);
        }
    }

    /// <summary>
    /// The type of the sensor the object was previously moving towards
    /// </summary>
    private SensorType PreviousSensorType
    {
        get
        {
            return sensorManager.GetSensorType(Path.PathSequence[CurrentNodeId - 1].name);
        }
    }

    /// <summary>
    /// Checks if its safe to continue driving forwards
    /// </summary>
    private bool SafeToContinue
    {
        get
        {
            if (PauseMoving)
            {
                SensorType sensorType = sensorManager.GetSensorType(currentNode.name);
                if (trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Green && (sensorType == SensorType.FirstSensorNode || sensorType == SensorType.ThirdSensorNode))
                {
                    return true;
                }
                if (sensorType != SensorType.FirstSensorNode && sensorType != SensorType.ThirdSensorNode && !IsColliding())
                {
                    return true;
                }
                return false;
            }
            return true;
        }
    }
    #endregion

    #region Public methods
    #endregion

    #region Private methods
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
        List<RaycastHit2D> hits = Physics2D.RaycastAll(transform.position, transform.TransformDirection(Vector3.up), CollisionDistance).ToList();

        string objectName = gameObject.name.ToLower();
        if (objectName.Contains("bike") || objectName.Contains("foot"))
        {
            hits.RemoveAll(x => x.collider.gameObject.GetComponent<Move>().Path != Path);
        }

        if (hits.Count > 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gradually moves towards the next node in the path
    /// </summary>
    /// <param name="node"></param>
    private void MoveTowardsNode(Transform node)
    {
        transform.position =
            Vector3.MoveTowards(transform.position,
                                node.position,
                                Time.deltaTime * Speed);

        if (CloseEnoughToCurrentNode)
        {
            // Selfdestruct if last node was hit
            if (Path.PathSequence.Length - 1 == CurrentNodeId)
            {
                Destroy(gameObject);
            }

            if (CurrentSensorType != SensorType.NotASensor && CurrentSensorType != SensorType.WarningNode)
            {
                PressCurrentSensor();

                if ((CurrentSensorType == SensorType.FirstSensorNode || CurrentSensorType == SensorType.ThirdSensorNode) &&
                    (trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Red || trafficLightManager.CheckLightStatus(lightName) == TrafficLightStatus.Orange))
                {
                    // Light is red
                    PauseMoving = true;
                    return;
                }
            }
            CurrentNodeId++;
            previousNodeUnpressed = false;
        }
        if (!CloseEnoughToPreviousNode && !previousNodeUnpressed)
        {
            UnPressPreviousSensor();
            previousNodeUnpressed = true;
        }
    }

    /// <summary>
    /// Indicates that the current sensor is pressed if the current node is actually a sensor
    /// </summary>
    private void PressCurrentSensor()
    {
        if (CurrentSensorType != SensorType.WarningNode && CurrentSensorType != SensorType.NotASensor)
        {
            string fullSensorName = currentNode.parent.parent.parent.parent.name + "/" + pathName;
            sensorManager.UpdateSensor(fullSensorName, (int)CurrentSensorType, 1);
        }
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

    /// <summary>
    /// Runs on the initial load of this script
    /// </summary>
    private void Start()
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

    /// <summary>
    /// Indicates that the current sensor is no longer pressed if the current node is actually a sensor
    /// </summary>
    private void UnPressPreviousSensor()
    {
        if (PreviousSensorType != SensorType.WarningNode && PreviousSensorType != SensorType.NotASensor)
        {
            string fullSensorName = currentNode.parent.parent.parent.parent.name + "/" + pathName;
            sensorManager.UpdateSensor(fullSensorName, (int)PreviousSensorType, 0);
        }
    }

    /// <summary>
    /// Gets called every frame
    /// </summary>
    private void Update()
    {
        currentNode = Path.PathSequence[CurrentNodeId];

        RotateTowardsNode(currentNode);
        if (IsColliding())
        {
            PauseMoving = true;
        }

        if (!PauseMoving)
        {
            MoveTowardsNode(currentNode);
        }
        else
        {
            if (SafeToContinue)
            {
                PauseMoving = false;
            }
        }
    }
    #endregion
}