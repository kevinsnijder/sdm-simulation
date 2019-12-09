using UnityEngine;

/// <summary>
/// Predefined driveable path
/// </summary>
public class MovementPath : MonoBehaviour
{
    #region Public variables

    public Transform[] PathSequence; //Array of all points in the path

    #endregion Public variables

    #region Public methods

    /// <summary>
    /// Draws lines between nodes
    /// </summary>
    public void OnDrawGizmos()
    {
        //Make sure that your sequence has points in it
        //and that there are at least two points to constitute a path
        if (PathSequence == null || PathSequence.Length < 2)
        {
            return; //Exits OnDrawGizmos if no line is needed
        }

        //Loop through all of the points in the sequence of points
        for (var i = 1; i < PathSequence.Length; i++)
        {
            //Draw a line between the points
            Gizmos.DrawLine(PathSequence[i - 1].position, PathSequence[i].position);
        }
    }

    #endregion Public methods

    #region Private methods

    //Update is called by Unity every frame
    private void Update()
    {
    }

    #endregion Private methods
}