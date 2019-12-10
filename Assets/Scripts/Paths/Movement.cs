using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class is the brain of a path user
/// </summary>
public class Movement : MonoBehaviour
{
    #region Public variables

    public float CollisionDistance;
    public float MaxDistanceToGoal = .1f;
    public MovementPath Path;
    public float RotationSpeedMultiplier = 7;
    public float Speed;

    #endregion Public variables

    #region Private variables

    private Transform CurrentNode;
    private int CurrentNodeId = 0;
    private string LightName;
    private string PathName;
    private bool PauseMoving = false;
    private bool PreviousNodeUnpressed = true;
    private SensorManager SensorManager;
    private TrafficLightManager TrafficLightManager;
    private SpecialObjectManager SpecialObjectManager;

    #endregion Private variables

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
                return true;
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
            return SensorManager.GetSensorType(CurrentNode.name);
        }
    }

    /// <summary>
    /// The type of the sensor the object was previously moving towards
    /// </summary>
    private SensorType PreviousSensorType
    {
        get
        {
            return SensorManager.GetSensorType(Path.PathSequence[CurrentNodeId - 1].name);
        }
    }

    /// <summary>
    /// Checks if this object is a boat
    /// </summary>
    private bool IsBoat
    {
        get
        {
            if (this.gameObject.name.ToLower().Contains("boat"))
                return true;
            return false;
        }
    }

    /// <summary>
    /// Checks if this object is a train
    /// </summary>
    private bool IsTrain
    {
        get
        {
            if (this.gameObject.name.ToLower().Contains("train"))
                return true;
            return false;
        }
    }

    /// <summary>
    /// Checks if this object is a bike
    /// </summary>
    private bool IsBike
    {
        get
        {
            if (this.gameObject.name.ToLower().Contains("bike"))
                return true;
            return false;
        }
    }

    /// <summary>
    /// Checks if this object is a pedestrian
    /// </summary>
    private bool IsFoot
    {
        get
        {
            if (this.gameObject.name.ToLower().Contains("foot"))
                return true;
            return false;
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
                SensorType sensorType = SensorManager.GetSensorType(CurrentNode.name);
                if (TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Green && (sensorType == SensorType.FirstSensorNode || sensorType == SensorType.ThirdSensorNode))
                    return true;
                if (sensorType != SensorType.FirstSensorNode && sensorType != SensorType.ThirdSensorNode && !IsColliding() && sensorType != SensorType.WarningNode && sensorType != SensorType.BarrierNode)
                    return true;
                if ((sensorType == SensorType.WarningNode || sensorType == SensorType.BarrierNode) && SpecialObjectManager.CheckLightStatus("vessel/0/warning_light/0") == WarningLightStatus.Off)
                    return true;
                return false;
            }
            return true;
        }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the name of the trafficlight this object has to check
    /// </summary>
    /// <returns></returns>
    private string GetCurrentTrafficlight()
    {
        Transform currentNode = Path.PathSequence[CurrentNodeId];

        string lightName = currentNode.parent.parent.parent.parent.name + "/" + PathName + "/traffic_light/0";

        // Override lightname when this object is a bike or a train
        if (IsBike)
        {
            if (currentNode.parent.parent.name == "path0")
                lightName = currentNode.parent.parent.parent.parent.name + "/" + PathName + "/traffic_light/0";
            else
                lightName = currentNode.parent.parent.parent.parent.name + "/" + PathName + "/traffic_light/1";
        }
        if (IsTrain)
        {
            PathName = "0";
            if (currentNode.parent.parent.name == "path0")
                lightName = currentNode.parent.parent.parent.parent.name + "/0/train_light/0";
            else
                lightName = currentNode.parent.parent.parent.parent.name + "/0/train_light/1";
        }
        if (IsBoat)
        {
            PathName = "0";
            if (currentNode.parent.parent.name == "path0")
                lightName = currentNode.parent.parent.parent.parent.name + "/" + PathName + "/boat_light/0";
            else
                lightName = currentNode.parent.parent.parent.parent.name + "/" + PathName + "/boat_light/1";
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
        if (IsBike || IsFoot)
            hits.RemoveAll(x => x.collider.gameObject.GetComponent<Movement>().Path != Path);

        if (hits.Count > 1)
            return true;
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
                if (IsTrain)
                    TrafficSpawnManager.Instance.TrainHasSpawned = false;

                Destroy(gameObject);
            }

            if (CurrentSensorType != SensorType.NotASensor && CurrentSensorType != SensorType.WarningNode && CurrentSensorType != SensorType.BarrierNode)
            {
                PressCurrentSensor();

                if ((CurrentSensorType == SensorType.FirstSensorNode || CurrentSensorType == SensorType.ThirdSensorNode) &&
                    (TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Red || TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Orange))
                {
                    // Light is red
                    PauseMoving = true;
                    return;
                }
            }
            else if ((CurrentSensorType == SensorType.WarningNode || CurrentSensorType == SensorType.BarrierNode) && SpecialObjectManager.CheckLightStatus("vessel/0/warning_light/0") == WarningLightStatus.Flashing)
            {
                PauseMoving = true;
                return;
            }
            CurrentNodeId++;
            PreviousNodeUnpressed = false;
        }
        if (!CloseEnoughToPreviousNode && !PreviousNodeUnpressed)
        {
            UnPressPreviousSensor();
            PreviousNodeUnpressed = true;
        }
    }

    /// <summary>
    /// Indicates that the current sensor is pressed if the current node is actually a sensor
    /// </summary>
    private void PressCurrentSensor()
    {
        string trackName = CurrentNode.parent.parent.parent.parent.name + "/" + PathName;
        if (CurrentSensorType != SensorType.WarningNode && CurrentSensorType != SensorType.NotASensor && CurrentSensorType != SensorType.RemoveWarningNode && CurrentSensorType != SensorType.BarrierNode)
        {
            if ((IsTrain || IsBoat) && CurrentNode.parent.parent.name != "path0")
            {
                if (CurrentNode.name.ToLower().Contains("sensor0"))
                    SensorManager.UpdateSensor(trackName, (int)SensorType.ThirdSensorNode, 1);
                else if (CurrentNode.name.ToLower().Contains("sensor2"))
                    SensorManager.UpdateSensor(trackName, (int)SensorType.FirstSensorNode, 1);
                else if (CurrentNode.name.ToLower().Contains("sensor1"))
                    SensorManager.UpdateSensor(trackName, (int)SensorType.SecondSensorNode, 1);
            }
            else
            {
                SensorManager.UpdateSensor(trackName, (int)CurrentSensorType, 1);
            }
        }
        if (CurrentSensorType == SensorType.RemoveWarningNode)
        {
            if (IsBoat)
                SpecialObjectManager.RemoveBoatUnderneathDeck();
            else
                SpecialObjectManager.RemoveVehicleFromDeck();
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
        SensorManager = SensorManager.Instance;
        TrafficLightManager = TrafficLightManager.Instance;
        SpecialObjectManager = SpecialObjectManager.Instance;
        PathName = Path.PathSequence[0].parent.parent.parent.name;
        LightName = GetCurrentTrafficlight();

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
        if (PreviousSensorType != SensorType.WarningNode && PreviousSensorType != SensorType.NotASensor && PreviousSensorType != SensorType.RemoveWarningNode && PreviousSensorType != SensorType.BarrierNode)
        {
            string trackName = CurrentNode.parent.parent.parent.parent.name + "/" + PathName;
            if ((IsTrain || IsBoat) && Path.PathSequence[CurrentNodeId - 1].parent.parent.name != "path0")
            {
                if (Path.PathSequence[CurrentNodeId - 1].name.ToLower().Contains("sensor0"))
                    SensorManager.UpdateSensor(trackName, (int)SensorType.ThirdSensorNode, 0);
                else if (Path.PathSequence[CurrentNodeId - 1].name.ToLower().Contains("sensor2"))
                    SensorManager.UpdateSensor(trackName, (int)SensorType.FirstSensorNode, 0);
                else if (Path.PathSequence[CurrentNodeId - 1].name.ToLower().Contains("sensor1"))
                    SensorManager.UpdateSensor(trackName, (int)SensorType.SecondSensorNode, 0);
            }
            else
            {
                SensorManager.UpdateSensor(trackName, (int)PreviousSensorType, 0);
            }
        }
        if (PreviousSensorType == SensorType.BarrierNode)
        {
            //Press the deck sensor
            SpecialObjectManager.AddVehicleToDeck();
        }
        if (IsBoat && PreviousSensorType == SensorType.FirstSensorNode)
        {
            //Press underneeth deck sensor
            SpecialObjectManager.AddBoatUnderneathDeck();
        }
    }

    /// <summary>
    /// Gets called every frame
    /// </summary>
    private void Update()
    {
        CurrentNode = Path.PathSequence[CurrentNodeId];

        if (IsColliding())
            PauseMoving = true;

        if (!PauseMoving)
        {
            RotateTowardsNode(CurrentNode);
            MoveTowardsNode(CurrentNode);
        }
        else
        {
            if (SafeToContinue)
                PauseMoving = false;
        }
    }

    #endregion Private methods
}