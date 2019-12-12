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
    public float MinFootSpeed;
    public float MaxFootSpeed;
    public float MinCycleSpeed;
    public float MaxCycleSpeed;

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
    private TrafficSpawnManager TrafficSpawnManager;
    private System.Random rnd = new System.Random();

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
                if (IsFoot || IsBike)
                {
                    if (TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Green &&
                        (sensorType == SensorType.FirstSensorNode || sensorType == SensorType.SecondSensorNode))
                        return true;
                    if (sensorType != SensorType.FirstSensorNode && sensorType != SensorType.SecondSensorNode &&
                        !IsColliding() && sensorType != SensorType.TrackWarningNode &&
                        sensorType != SensorType.DeckBarrierNode)
                        return true;
                    if (sensorType == SensorType.TrackWarningNode &&
                        SpecialObjectManager.CheckLightStatus("track/0/warning_light/0") == WarningLightStatus.Off)
                        return true;
                    if (sensorType == SensorType.DeckBarrierNode &&
                        SpecialObjectManager.CheckLightStatus("vessel/0/warning_light/0") == WarningLightStatus.Off)
                        return true;
                    return false;
                }
                else if (IsBoat || IsTrain)
                {
                    if (TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Green &&
                        (sensorType == SensorType.FirstSensorNode || sensorType == SensorType.ThirdSensorNode))
                        return true;
                    if (sensorType != SensorType.FirstSensorNode && sensorType != SensorType.ThirdSensorNode &&
                        !IsColliding())
                        return true;
                    return false;
                }
                else
                {
                    if (TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Green && 
                        (sensorType == SensorType.FirstSensorNode || sensorType == SensorType.ThirdSensorNode))
                        return true;
                    if (sensorType != SensorType.FirstSensorNode && sensorType != SensorType.ThirdSensorNode && 
                        !IsColliding() && sensorType != SensorType.WarningNode && 
                        sensorType != SensorType.DeckBarrierNode)
                        return true;
                    if ((sensorType == SensorType.WarningNode || sensorType == SensorType.DeckBarrierNode) && 
                        SpecialObjectManager.CheckLightStatus("vessel/0/warning_light/0") == WarningLightStatus.Off)
                        return true;
                    return false;
                }
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
        if (IsBike || IsFoot)
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
            //hits.RemoveAll(x => x.collider.gameObject.GetComponent<Movement>().Path != Path);
            hits.RemoveAll(x => x.collider.gameObject.GetComponent<Movement>().Path);


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
                    TrafficSpawnManager.TrainHasSpawned = false;
                if (IsFoot)
                {
                    RespawnFoot();
                    return;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            if (CurrentSensorType != SensorType.WarningNode && 
                CurrentSensorType != SensorType.DeckBarrierNode && 
                CurrentSensorType != SensorType.TrackWarningNode)
            {
                PressCurrentSensor();

                if (IsTrain || IsBoat)
                {
                    if (CurrentSensorType == SensorType.FirstSensorNode 
                        && TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Red) {
                        PauseMoving = true;
                        return;
                    }
                }
                else if ((IsFoot || IsBike) && 
                    (TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Red || 
                    TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Orange))
                {
                    Transform currentNode = Path.PathSequence[CurrentNodeId];

                    if (currentNode.parent.parent.name == "path0" && CurrentSensorType == SensorType.FirstSensorNode)
                    {
                        PauseMoving = true;
                        return;
                    }
                    else if (currentNode.parent.parent.name == "path1" && CurrentSensorType == SensorType.SecondSensorNode)
                    {
                        PauseMoving = true;
                        return;
                    }
                }
                else if ((CurrentSensorType == SensorType.FirstSensorNode || 
                    CurrentSensorType == SensorType.ThirdSensorNode) &&
                    (TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Red || 
                    TrafficLightManager.CheckLightStatus(LightName) == TrafficLightStatus.Orange))
                {
                    PauseMoving = true;
                    return;
                }
            }
            else if ((CurrentSensorType == SensorType.WarningNode || CurrentSensorType == SensorType.DeckBarrierNode) 
                && SpecialObjectManager.CheckLightStatus("vessel/0/warning_light/0") == WarningLightStatus.Flashing)
            {
                PauseMoving = true;
                return;
            }
            else if (CurrentSensorType == SensorType.TrackWarningNode 
                && SpecialObjectManager.CheckLightStatus("track/0/warning_light/0") == WarningLightStatus.Flashing)
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
    /// This function first checks if a Foot instance is at the end of a group that continues into another group
    /// If so it resets the path variables to the ones of the continueing group
    /// </summary>
    private void RespawnFoot()
    {
        // If previous to last node was a sensor, respawn this foot in the connecting group
        if (PreviousSensorType == SensorType.FirstSensorNode || PreviousSensorType == SensorType.SecondSensorNode)
        {
            Transform currentNode = Path.PathSequence[CurrentNodeId];

            if (currentNode.parent.parent.name == "path0")
            {
                // Group 0 pad 0
                if (PathName.Equals("0"))
                {
                    // Group 1 pad 1
                    Path = TrafficSpawnManager.FootRespawnPaths[1].GetComponent<MovementPath>();
                }
                // Group 1 pad 0
                else if (PathName.Equals("1"))
                {
                    // Group 0 pad 1
                    Path = TrafficSpawnManager.FootRespawnPaths[0].GetComponent<MovementPath>();
                }
                // Group 3 pad 0
                else if (PathName.Equals("3"))
                {
                    // Group 4 pad 0
                    Path = TrafficSpawnManager.FootRespawnPaths[3].GetComponent<MovementPath>();
                }
                // Group 4 pad 0
                else if (PathName.Equals("4"))
                {
                    // Group 5 pad 1
                    Path = TrafficSpawnManager.FootRespawnPaths[5].GetComponent<MovementPath>();
                }
                // Group 5 pad 0
                else if (PathName.Equals("5"))
                {
                    // Group 4 pad 1
                    Path = TrafficSpawnManager.FootRespawnPaths[4].GetComponent<MovementPath>();
                }
            }
            else
            {
                // Group 4 pad 1
                if (PathName.Equals("4"))
                {
                    // Group 3 pad 1
                    Path = TrafficSpawnManager.FootRespawnPaths[2].GetComponent<MovementPath>();
                }
            }
            transform.position = Path.PathSequence[0].position;
            PathName = Path.PathSequence[0].parent.parent.parent.name;
            CurrentNodeId = 0;
            LightName = GetCurrentTrafficlight();
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Indicates that the current sensor is pressed if the current node is actually a sensor
    /// </summary>
    private void PressCurrentSensor()
    {
        string currentPath = CurrentNode.parent.parent.parent.parent.name + "/" + PathName;
        if (CurrentSensorType == SensorType.FirstSensorNode || 
            CurrentSensorType == SensorType.SecondSensorNode || 
            CurrentSensorType == SensorType.ThirdSensorNode || 
            CurrentSensorType == SensorType.FourthSensorNode)
        {
            if (IsTrain && CurrentNode.parent.parent.name != "path0")
            {
                if (CurrentSensorType == SensorType.FirstSensorNode)
                    SensorManager.UpdateSensor(currentPath, (int)SensorType.ThirdSensorNode, 1);
                else if (CurrentSensorType == SensorType.ThirdSensorNode)
                    SensorManager.UpdateSensor(currentPath, (int)SensorType.FirstSensorNode, 1);
                else if (CurrentSensorType == SensorType.SecondSensorNode)
                    SensorManager.UpdateSensor(currentPath, (int)SensorType.SecondSensorNode, 1);
            }
            else if (IsBoat && CurrentNode.parent.parent.name != "path0")
            {
                if (CurrentSensorType == SensorType.FirstSensorNode)
                    SensorManager.UpdateSensor(currentPath, (int)SensorType.ThirdSensorNode, 1);
            }
            else
            {
                SensorManager.UpdateSensor(currentPath, (int)CurrentSensorType, 1);
            }
        }
        if (CurrentSensorType == SensorType.RemoveDeckNode)
        {
            SpecialObjectManager.RemoveVehicleFromDeck();
        }
        if (CurrentSensorType == SensorType.RemoveUnderDeckNode)
        {
            SpecialObjectManager.RemoveBoatUnderneathDeck();
        }
    }

    /// <summary>
    /// Indicates that the current sensor is no longer pressed if the current node is actually a sensor
    /// </summary>
    private void UnPressPreviousSensor()
    {
        if (PreviousSensorType == SensorType.FirstSensorNode ||
            PreviousSensorType == SensorType.SecondSensorNode ||
            PreviousSensorType == SensorType.ThirdSensorNode ||
            PreviousSensorType == SensorType.FourthSensorNode)
        {
            string currentPath = CurrentNode.parent.parent.parent.parent.name + "/" + PathName;
            if (IsTrain && Path.PathSequence[CurrentNodeId - 1].parent.parent.name != "path0")
            {
                if (PreviousSensorType == SensorType.FirstSensorNode)
                    SensorManager.UpdateSensor(currentPath, (int)SensorType.ThirdSensorNode, 0);
                else if (PreviousSensorType == SensorType.ThirdSensorNode)
                    SensorManager.UpdateSensor(currentPath, (int)SensorType.FirstSensorNode, 0);
                else if (PreviousSensorType == SensorType.SecondSensorNode)
                    SensorManager.UpdateSensor(currentPath, (int)SensorType.SecondSensorNode, 0);
            }
            else if (IsBoat && CurrentNode.parent.parent.name != "path0")
            {
                if (CurrentSensorType == SensorType.FirstSensorNode)
                    SensorManager.UpdateSensor(currentPath, (int)SensorType.ThirdSensorNode, 0);
            }
            else
            {
                SensorManager.UpdateSensor(currentPath, (int)PreviousSensorType, 0);
            }
        }
        if (PreviousSensorType == SensorType.DeckBarrierNode)
        {
            //Pressed the deck sensor
            SpecialObjectManager.AddVehicleToDeck();
        }
        if (IsBoat && PreviousSensorType == SensorType.UnderDeckNode)
        {
            //Pressed underneath deck sensor
            SpecialObjectManager.AddBoatUnderneathDeck();
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
        TrafficSpawnManager = TrafficSpawnManager.Instance;
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