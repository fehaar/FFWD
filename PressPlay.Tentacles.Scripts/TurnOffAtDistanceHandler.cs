using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class TurnOffAtDistanceHandler : MonoBehaviour {
        //public float turnOffDistance = 12;
        //public Bounds turnOffBounds;
        //[HideInInspector]
        //private float turnOffDistanceSqrt;

        //private TurnOffAtDistance[] turnOffAtDistanceObjects;

        //private TurnOffAtDistance[,][] turnOffAtDistanceObjectGrid;
        //private GridElement[,] grid;
        //private ArrayList activeGridElements = new ArrayList();
        //private ArrayList newActiveGridElements = new ArrayList();

        //private Vector2 gridOffset;
        //private float gridElementSize = 5;

        //public int turnOffFrameSkip = 10;
        //private int turnOffCurrentFrame = 0;

        //private int gridSizeX;
        //private int gridSizeY;

        //public bool DEACTIVATE_TURN_OFF_AT_DISTANCE = false;

        //private List<TurnOffAtDistance> objectsToActivate = new List<TurnOffAtDistance>();
        //private List<TurnOffAtDistance> objectsToDeActivate = new List<TurnOffAtDistance>();

        //protected void InitializeDistanceHandling(GameObject _distanceObject)
        //{
        //    if (DEACTIVATE_TURN_OFF_AT_DISTANCE)
        //    {
        //        return;
        //    }


        //    CreateArrayOf_TurnOffAtDistanceObjects();
        //    turnOffDistanceSqrt = turnOffDistance * turnOffDistance;

        //    InitializeTurnOffAtDistanceObjects(_distanceObject);
        //}

        //private void InitializeTurnOffAtDistanceObjects(GameObject _distanceObject)
        //{
        //    if (turnOffAtDistanceObjects.Length == 0)
        //    {
        //        return;
        //    }

        //    Vector3 max = turnOffAtDistanceObjects[0].transform.position;
        //    Vector3 min = turnOffAtDistanceObjects[0].transform.position;

        //    for (int i = 0; i < turnOffAtDistanceObjects.Length; i++)
        //    {
        //        turnOffAtDistanceObjects[i].Initialize();

        //        if (turnOffAtDistanceObjects[i].transform.position.x > max.x)
        //        { max.x = turnOffAtDistanceObjects[i].transform.position.x; }
        //        if (turnOffAtDistanceObjects[i].transform.position.y > max.y)
        //        { max.y = turnOffAtDistanceObjects[i].transform.position.y; }
        //        if (turnOffAtDistanceObjects[i].transform.position.z > max.z)
        //        { max.z = turnOffAtDistanceObjects[i].transform.position.z; }

        //        if (turnOffAtDistanceObjects[i].transform.position.x < min.x)
        //        { min.x = turnOffAtDistanceObjects[i].transform.position.x; }
        //        if (turnOffAtDistanceObjects[i].transform.position.y < min.y)
        //        { min.y = turnOffAtDistanceObjects[i].transform.position.y; }
        //        if (turnOffAtDistanceObjects[i].transform.position.z < min.z)
        //        { min.z = turnOffAtDistanceObjects[i].transform.position.z; }
        //    }

        //    gridOffset.x = min.x;
        //    gridOffset.y = min.z;


        //    gridSizeX = (int)(((max.x - min.x) / gridElementSize) + 2);
        //    gridSizeY = (int)(((max.z - min.z) / gridElementSize) + 2);

        //    //Debug.DrawLine(min, max, Color.magenta);
        //    //Debug.Break();

        //    ArrayList[,] tmpGrid = new ArrayList[gridSizeX, gridSizeY];
        //    for (int x = 0; x < gridSizeX; x++)
        //    {
        //        for (int y = 0; y < gridSizeY; y++)
        //        {
        //            tmpGrid[x, y] = new ArrayList();
        //        }
        //    }

        //    ArrayList tmpDistanceObjects = new ArrayList();
        //    for (int i = 0; i < turnOffAtDistanceObjects.Length; i++)
        //    {
        //        if (turnOffAtDistanceObjects[i].markedForDestruction)
        //        {
        //            Destroy(turnOffAtDistanceObjects[i]);
        //        }
        //        else
        //        {
        //            tmpDistanceObjects.Add(turnOffAtDistanceObjects[i]);

        //            //Debug.Log("  grid x,y : "+GetGridX(turnOffAtDistanceObjects[i].transform.position)+", "+GetGridY(turnOffAtDistanceObjects[i].transform.position));

        //            if (tmpGrid[GetGridX(turnOffAtDistanceObjects[i].transform.position), GetGridY(turnOffAtDistanceObjects[i].transform.position)] == null)
        //            {
        //                tmpGrid[GetGridX(turnOffAtDistanceObjects[i].transform.position), GetGridY(turnOffAtDistanceObjects[i].transform.position)] = new ArrayList();
        //            }

        //            tmpGrid[GetGridX(turnOffAtDistanceObjects[i].transform.position), GetGridY(turnOffAtDistanceObjects[i].transform.position)].Add(turnOffAtDistanceObjects[i]);
        //        }
        //    }

        //    turnOffAtDistanceObjects = new TurnOffAtDistance[tmpDistanceObjects.Count];
        //    for (int i = 0; i < turnOffAtDistanceObjects.Length; i++)
        //    {
        //        turnOffAtDistanceObjects[i] = ((TurnOffAtDistance)tmpDistanceObjects[i]);
        //        //turnOffAtDistanceObjects[i].CheckDistance(turnOffDistanceSqrt, _distanceObject.transform.position);
        //    }
        //    turnOffBounds = new Bounds(_distanceObject.transform.position, Vector3.one * turnOffDistance * 2);

        //    //create actual grid array
        //    //turnOffAtDistanceObjectGrid = new TurnOffAtDistance[gridSizeX,gridSizeY][];

        //    grid = new GridElement[gridSizeX, gridSizeY];

        //    for (int x = 0; x < gridSizeX; x++)
        //    {
        //        for (int y = 0; y < gridSizeY; y++)
        //        {
        //            /*if (tmpGrid[x,y] == null)
        //            {
        //                tmpGrid[x,y] = new ArrayList();
        //            }*/

        //            //Debug.Log("tmpGrid["+x+","+y+"].Count : "+tmpGrid[x,y].Count);

        //            //turnOffAtDistanceObjectGrid[x,y] = new TurnOffAtDistance[tmpGrid[x,y].Count];

        //            grid[x, y] = new GridElement(x, y, tmpGrid[x, y]);
        //            grid[x, y].SetStatus(false);

        //            /*for (int i = 0; i < tmpGrid[x,y].Count; i++) {
        //                turnOffAtDistanceObjectGrid[x,y][i] = (TurnOffAtDistance)tmpGrid[x,y][i];
        //            }*/
        //        }
        //    }

        //    //Debug.Break();

        //    //turnOffBounds.SetMinMax(-Vector3.one * turnOffDistance, Vector3.one * turnOffDistance);
        //}

        ///// <summary>
        ///// 
        ///// STILL FAULTY!!! DONT USE!!
        ///// </summary>
        ///// <param name="_distanceObject">
        ///// A <see cref="GameObject"/>
        ///// </param>
        //protected void CheckDistanceUsingGrid(GameObject _distanceObject)
        //{
        //    int gridX = GetGridX(_distanceObject.transform.position);
        //    int gridY = GetGridY(_distanceObject.transform.position);

        //    //Debug.Log("grid position : "+gridX+", "+gridY);

        //    int surroundingElements = 2;

        //    newActiveGridElements.Clear();


        //    for (int x = gridX - surroundingElements; x < gridX + surroundingElements + 1; x++)
        //    {
        //        for (int y = gridY - surroundingElements; y < gridY + surroundingElements + 1; y++)
        //        {

        //            if (x < 0 || x > gridSizeX - 1 || y < 0 || y > gridSizeY - 1)
        //            { continue; }


        //            newActiveGridElements.Add(grid[x, y]);
        //        }
        //    }

        //    for (int i = 0; i < activeGridElements.Count; i++)
        //    {

        //        if (!newActiveGridElements.Contains(activeGridElements[i]))
        //        {
        //            //newlyInactiveGridElements.Add(activeGridElements[i]);

        //            ((GridElement)activeGridElements[i]).SetStatus(false);
        //            activeGridElements.Remove(activeGridElements[i]);
        //        }
        //    }

        //    activeGridElements.AddRange(newActiveGridElements);

        //    for (int i = 0; i < activeGridElements.Count; i++)
        //    {
        //        ((GridElement)activeGridElements[i]).SetStatus(true);
        //        ((GridElement)activeGridElements[i]).CheckDistanceOnObjects(turnOffDistanceSqrt, _distanceObject);
        //    }
        //}

        //public void UpdateAllObjectsImmediatly(GameObject _distanceObject)
        //{
        //    if (DEACTIVATE_TURN_OFF_AT_DISTANCE)
        //    {
        //        return;
        //    }

        //    for (int i = 0; i < turnOffAtDistanceObjects.Length; i++)
        //    {

        //        if (turnOffAtDistanceObjects[i] != null)
        //        {
        //            //turnOffAtDistanceObjects[i].CheckDistance(turnOffDistanceSqrt, _distanceObject.transform.position);

        //            turnOffBounds.center = _distanceObject.transform.position;

        //            // We remember the object we wish to alter
        //            AddObjectToQueue(turnOffAtDistanceObjects[i], turnOffAtDistanceObjects[i].CheckBounds(turnOffBounds));
        //        }
        //    }

        //    UpdateAllObjects();

        //    //turnOffCurrentFrame = (turnOffCurrentFrame+1)%turnOffFrameSkip;
        //}


        protected void UpdateTurnOffAtDistanceObjects(GameObject _distanceObject)
        {
        //    if (DEACTIVATE_TURN_OFF_AT_DISTANCE)
        //    {
        //        return;
        //    }

        //    for (int i = turnOffCurrentFrame; i < turnOffAtDistanceObjects.Length; i += turnOffFrameSkip)
        //    {

        //        if (turnOffAtDistanceObjects[i] != null)
        //        {
        //            //turnOffAtDistanceObjects[i].CheckDistance(turnOffDistanceSqrt, _distanceObject.transform.position);

        //            turnOffBounds.center = _distanceObject.transform.position;

        //            // We remember the object we wish to alter
        //            AddObjectToQueue(turnOffAtDistanceObjects[i], turnOffAtDistanceObjects[i].CheckBounds(turnOffBounds));
        //        }
        //    }

        //    UpdateObjects();

        //    turnOffCurrentFrame = (turnOffCurrentFrame + 1) % turnOffFrameSkip;
        }


        //private void UpdateAllObjects()
        //{

        //    int length = objectsToActivate.Count;
        //    int i = 0;
        //    while (i < length)
        //    {
        //        objectsToActivate[0].SetActiveState(true);
        //        objectsToActivate.RemoveAt(0);
        //        i++;
        //    }

        //    length = objectsToDeActivate.Count;
        //    i = 0;
        //    while (i < length)
        //    {
        //        objectsToDeActivate[0].SetActiveState(false);
        //        objectsToDeActivate.RemoveAt(0);
        //        i++;
        //    }
        //}

        //private void UpdateObjects()
        //{

        //    int length = Mathf.Min(1, objectsToActivate.Count);
        //    int i = 0;
        //    while (i < length)
        //    {
        //        objectsToActivate[0].SetActiveState(true);
        //        objectsToActivate.RemoveAt(0);
        //        i++;
        //    }

        //    length = Mathf.Min(1, objectsToDeActivate.Count);
        //    i = 0;
        //    while (i < length)
        //    {
        //        objectsToDeActivate[0].SetActiveState(false);
        //        objectsToDeActivate.RemoveAt(0);
        //        i++;
        //    }
        //}

        //private void AddObjectToQueue(TurnOffAtDistance obj, bool enable)
        //{
        //    // We check to see whether obj qualifies for action
        //    if ((enable && obj.gameObject.active) || (!enable && !obj.gameObject.active))
        //    {
        //        return;
        //    }

        //    if (enable && !objectsToActivate.Contains(obj))
        //    {
        //        // We check to see if we have already added the object to the deactivation list
        //        if (objectsToDeActivate.Contains(obj))
        //        {
        //            objectsToDeActivate.Remove(obj);
        //        }

        //        // We add the object to the list
        //        objectsToActivate.Add(obj);
        //    }
        //    else if (!enable && !objectsToDeActivate.Contains(obj))
        //    {
        //        // We check to see if we have already added the object to the activation list
        //        if (objectsToActivate.Contains(obj))
        //        {
        //            objectsToActivate.Remove(obj);
        //        }

        //        // We add the object to the list
        //        objectsToDeActivate.Add(obj);
        //    }
        //}

        //private void CreateArrayOf_TurnOffAtDistanceObjects()
        //{
        //    Object[] tmpTurnOffArray = FindObjectsOfType(typeof(TurnOffAtDistance));

        //    turnOffAtDistanceObjects = new TurnOffAtDistance[tmpTurnOffArray.Length];

        //    for (int i = 0; i < tmpTurnOffArray.Length; i++)
        //    {

        //        turnOffAtDistanceObjects[i] = (TurnOffAtDistance)(tmpTurnOffArray[i]);
        //    }
        //}

        //private int GetGridX(Vector3 _pos)
        //{
        //    return Mathf.Clamp((int)(((_pos.x - gridOffset.x) / gridElementSize)), 0, gridSizeX - 1);
        //}

        //private int GetGridY(Vector3 _pos)
        //{
        //    return Mathf.Clamp((int)(((_pos.z - gridOffset.y) / gridElementSize)), 0, gridSizeY - 1);
        //}

        //void OnDrawGizmos()
        //{
        //    for (int x = 0; x < gridSizeX; x++)
        //    {
        //        for (int y = 0; y < gridSizeY; y++)
        //        {

        //            if (grid[x, y].activationStatus)
        //            {
        //                Gizmos.DrawWireCube(new Vector3(gridOffset.x + x * gridElementSize, 0, gridOffset.y + y * gridElementSize), new Vector3(gridElementSize, gridElementSize, gridElementSize));
        //            }
        //        }
        //    }
        //}
    }
}