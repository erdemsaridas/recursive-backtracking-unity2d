using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject wallPrefab;           // Public and private variables for the class
    public GameObject floorPrefab;
    public Transform wallParent;
    public Transform floorParent;
    public int mazeWidth;
    public int mazeHeight;
    public Vector2 startingPoint;
    public int mazeEntrance;
    public int mazeExit;
    private Stack<GameObject> _floorStack;
    private List<GameObject> _unvisitedFloorList{get; set;}

    void Start()
    {
        _unvisitedFloorList = new List<GameObject>(); 
        _floorStack = new Stack<GameObject>();
        CreateFloorsAndWalls(startingPoint);
        CreateMaze();
    }

   public void CreateFloorsAndWalls(Vector2 startingPoint)
   {
        float floorOffsetValue = floorPrefab.transform.localScale.x/2;                // This part creates an offset value to determine exactly where to instantiate the objects,
        float wallOffsetValue = wallPrefab.transform.localScale.y/2;                  // For now it only works for " 1 " scaled objects because of the for loop
        Vector3 widthoffset = new Vector3(wallOffsetValue, wallOffsetValue, 0);       // Needs fixing in the for loop to work properly with different scaled objects.  
        Vector3 heightoffset = new Vector3(wallOffsetValue, -wallOffsetValue, 0);
        Vector3 floorOffset = new Vector3(floorOffsetValue, 0f,0f);
        
        float spawnRotation = 90f; // To initiate the vertical walls, we declare the spawn rotation.
        int countY = 0;
        
        for (int x = 0; x < mazeWidth; x++)   
            // With the double for loops we first initiate the horizontal walls and then the vertical one
            {
            for (int y = 0; y < mazeHeight; y++)
            {
                Vector3 position = new Vector3(x+startingPoint.x, y+startingPoint.y, 0);           
                GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
                wall.transform.parent = wallParent;

                if(y <mazeHeight+1 )
                {
                    wall = Instantiate(wallPrefab,position+Vector3.right, Quaternion.identity);
                    wall.transform.parent = wallParent;
                }
                GameObject floor = Instantiate(floorPrefab,position+floorOffset, Quaternion.identity);
                floor.transform.parent = floorParent;
                floor.gameObject.name = "Floor_" + floor.transform.position;
                _unvisitedFloorList.Add(floor);   
                countY = y-1;

                if((!(y == mazeHeight-1 && x == mazeEntrance)) )
                {
                    GameObject horizontalWall = Instantiate(wallPrefab, position + widthoffset, Quaternion.identity);
                    horizontalWall.transform.rotation = Quaternion.Euler(0f,0f,spawnRotation);
                    horizontalWall.transform.parent = wallParent;
                }
                
                
            }
            

            if(x < mazeWidth+1 && !(x == mazeExit))
                {
                    
                    Vector3 position = new Vector3(x+startingPoint.x, countY+startingPoint.y+(3-mazeHeight), 0);
                    GameObject lastHorizontalWall = Instantiate(wallPrefab, position+ Vector3.down+ heightoffset, Quaternion.identity);
                    lastHorizontalWall.transform.rotation = Quaternion.Euler(0f,0f,spawnRotation);
                    lastHorizontalWall.transform.parent = wallParent;
                }
        }
    }

    public void CreateMaze()                  
    {
        

        if(_unvisitedFloorList.Count!= 0)               // Check if unvisited floor list is empty.
        {
            int i = 0;
            int selectedIndex = Random.Range(1, _unvisitedFloorList.Count);
            GameObject selectedFloor = _unvisitedFloorList[selectedIndex-1].gameObject;
            _floorStack.Push(selectedFloor);
            //Debug.Log("Selected floor:" + selectedFloor.name);
            //GameObject textObject = selectedFloor.transform.GetChild(0).gameObject;
            //textObject.GetComponent<TMPro.TextMeshPro>().text = " " + 0;
            _unvisitedFloorList.RemoveAt(selectedIndex-1);
            
            
            

            while(GetRandomUnvisitedNeigbouringFloor(selectedFloor) != null)     // while there is an unvisited neighbouring floor work this script.
            {
                i++;
                GameObject randomUnvisitedFloor = GetRandomUnvisitedNeigbouringFloor(selectedFloor);
                int randomObjextIndex = _unvisitedFloorList.IndexOf(randomUnvisitedFloor);
                _unvisitedFloorList.RemoveAt(randomObjextIndex);

                //textObject = randomUnvisitedFloor.transform.GetChild(0).gameObject;  // only for to see how the script created path its way to completion.
                //textObject.GetComponent<TMPro.TextMeshPro>().text = " " + i;

                
                //Debug.Log("Selected floor is " + selectedFloor.name + "and random unvisited floor is " + randomUnvisitedFloor.name + "Iteration count :"+ i);
                DestroyWall(selectedFloor, randomUnvisitedFloor);
                
                if( GetRandomUnvisitedNeigbouringFloor(randomUnvisitedFloor) == null)
                {
                   
                    while(_unvisitedFloorList.Count != 0 && GetRandomUnvisitedNeigbouringFloor(selectedFloor) == null)
                    { 
                        selectedFloor = _floorStack.Pop();
                       
                    }

                    
                }
                else
                {
                    selectedFloor = randomUnvisitedFloor;
                    _floorStack.Push(randomUnvisitedFloor); 

                }
                
                
                
                
            }     

       }

        foreach (GameObject floor in _unvisitedFloorList)
        {
            Debug.Log("unvisited floor list : " + floor.name);
        }

    }



    public GameObject GetRandomUnvisitedNeigbouringFloor(GameObject floor)
    {
        GameObject[] neighbouringFloors = new GameObject[4];
        Vector3 position = floor.transform.position;
        Vector3 topPosition = new Vector3(position.x, position.y+1);                 // Using basic 2D vector mathematics, we can check the four sides of the current selected floor.
        Vector3 bottomPosition = new Vector3(position.x, position.y-1);
        Vector3 leftPosition = new Vector3(position.x-1, position.y);
        Vector3 rightPosition = new Vector3(position.x+1, position.y);

        neighbouringFloors[0] = GameObject.Find("Floor_" + topPosition);
        neighbouringFloors[1] = GameObject.Find("Floor_" + bottomPosition);
        neighbouringFloors[2] = GameObject.Find("Floor_" + leftPosition);
        neighbouringFloors[3] = GameObject.Find("Floor_" + rightPosition);


        List<GameObject> possibleFloors = new List<GameObject>();
        bool isEmpty = true;

        for(int i = 0; i < neighbouringFloors.Length; i++)
        {
            if(neighbouringFloors[i] != null)
            {
                if(_unvisitedFloorList.Contains(GameObject.Find(neighbouringFloors[i].name)))
                {
                    isEmpty = false;
                    possibleFloors.Add(neighbouringFloors[i]);                   
                }
                
            }
        }

        int randomInt = Random.Range(0,possibleFloors.Count);
    
        if(isEmpty)
        {
            return null;
        }
        else
        {

            return possibleFloors[randomInt]; // returns a random floor from the possible random floor list.
        }

        

       
        
    }

    public void DestroyWall(GameObject object1, GameObject object2)
    {
        Vector2 posA = object1.transform.position;
        Vector2 posB = object2.transform.position;

        // Calculate the midpoint between the two floors
        Vector2 midPoint = (posA + posB) / 2f;

        // Find all wall objects between the floors
        Collider2D[] walls = Physics2D.OverlapAreaAll(posA, posB);

        foreach (Collider2D wall in walls)
        {
            if (wall.CompareTag("Wall"))
            {
                // Check if the wall is between the bounds of midpoint
                if (wall.bounds.Contains(midPoint))
                {
                    Destroy(wall.gameObject);
                }
            }
        }
    }
}
