using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Predefined driveable path
/// </summary>
public class PathLayout : MonoBehaviour
{

    #region Public Variables
    public Transform[] PathSequence; //Array of all points in the path
    #endregion //Public Variables

    #region Private Variables

    #endregion //Private Variables

    // (Unity Named Methods)
    #region Main Methods
    //Update is called by Unity every frame
    void Update()
    {

    }

    //OnDrawGizmos will draw lines between our points in the Unity Editor
    //These lines will allow us to easily see the path that
    //our moving object will follow in the game
    public void OnDrawGizmos()
    {
        //Make sure that your sequence has points in it
        //and that there are at least two points to constitute a path
        if(PathSequence == null || PathSequence.Length < 2)
        {
            return; //Exits OnDrawGizmos if no line is needed
        }

        //Loop through all of the points in the sequence of points
        for(var i=1; i < PathSequence.Length; i++)
        {
            //Draw a line between the points
            Gizmos.DrawLine(PathSequence[i - 1].position, PathSequence[i].position);
        }
    }
    #endregion //Main Methods
}
