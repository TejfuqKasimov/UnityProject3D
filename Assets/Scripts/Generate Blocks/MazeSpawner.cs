using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public GameObject cellPrefab;  // Cell Prefab
    public GameObject blockPrefab;  // Block Prefab
    private GameObject currentBlock;
    private GameObject previousBlock;
    private GameObject previousPrBlock;
    public GameObject startRoomPrefab;  // Start Room Prefab
    private GameObject startRoom;
    private float Rast = 5.5f;
    private float PlayerRast = 5.5f;
    private float PlayerPrRast = 5.5f;
    private const float Wight = 3.4f;
    private const float Height = 3.4f;
    private int CountWight = 5;
    private int CountHeight = 5;
    private GameObject[,] currentGOMaze = new GameObject[5, 5];
    private GameObject[,] previousGOMaze = new GameObject[5, 5];
    private GameObject[,] previousPrGOMaze = new GameObject[5, 5];
    public Transform Character;
    private int LevelCount = 0;
    private MazeGeneratorCell[,] previousMaze = new MazeGeneratorCell[5, 5];
    private MazeGeneratorCell[,] currentMaze = new MazeGeneratorCell[5, 5];
    [SerializeField] private bool isEnd = false;
    public GameObject WayPrefab;
    private GameObject[,] currentPath = new GameObject[11, 11];
    private GameObject[,] previousPath = new GameObject[11, 11];
    private GameObject[,] previousPrPath = new GameObject[11, 11];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startRoom = Instantiate(startRoomPrefab, Vector3.zero, quaternion.identity);
        currentBlock = createBlock();
        currentPath = GenerateWay();
        currentGOMaze = Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Character.position.x >= PlayerRast)
        {
            previousMaze = currentMaze;
            PlayerPrRast = PlayerRast;
            PlayerRast = Rast;
            LevelCount++;
            previousPrPath = previousPath;
            previousPath = currentPath;
            currentPath = GenerateWay();
            previousPrGOMaze = previousGOMaze;
            previousGOMaze = currentGOMaze;
            currentGOMaze = Generate();
            previousPrBlock = previousBlock;
            previousBlock = currentBlock;
            currentBlock = createBlock();
            int tmp = LevelCount / 4;
            CountWight = 5 + 2 * tmp;
            CountHeight = 5 + 2 * tmp;
            // Destroy Previous Block
            if (previousPrBlock != null)
            {
                Destroy(previousPrBlock);
            }
            // Destroy Start Room if it doesn't Destroyedin previous frame
            if (startRoom != null)
            {
                Destroy(startRoom);
            }
            // Destroy Previous Path and Maze
            for (int x = 0; x < previousPrGOMaze.GetLength(0); ++x)
            {
                for (int z = 0; z < previousPrGOMaze.GetLength(1); ++z)
                {
                    if (previousPrGOMaze[x, z] != null)
                    {
                        Destroy(previousPrGOMaze[x, z]);
                    }
                    if (previousPrPath[x, z] != null)
                    {
                        Destroy(previousPrPath[x, z]);
                    }
                }
            }
        }
        // if level is end show way to next level
        if (isEnd)
        {
            ShowWay();
        }
    }
    // Generate Maze
    public GameObject[,] Generate()
    {
        MazeGenerator generator = new MazeGenerator();
        MazeGeneratorCell[,] maze = generator.GenerateMaze(CountWight, CountHeight);
        currentMaze = maze;
        GameObject[,] GOMaze = new GameObject[maze.GetLength(0), maze.GetLength(1)];
        for (int x = 0; x < maze.GetLength(0); ++x)
        {
            for (int z = 0; z < maze.GetLength(1); ++z)
            {
                GOMaze[x, z] = Instantiate(cellPrefab, new Vector3(PlayerRast + 1.7f + x * Wight, 0, -(Height * CountHeight / 2 - 1.7f) + z * Height), quaternion.identity);
                
                Cell c = GOMaze[x, z].GetComponent<Cell>();
                c.WallLeft.SetActive(maze[x, z].WallLeft);
                c.WallBottom.SetActive(maze[x, z].WallBottom);
                // if (maze[x, z].WallLeft) currentPath[x, z].GetComponent<Way>().Left.SetActive(false);
                // else                     currentPath[x, z].GetComponent<Way>().Left.SetActive(true);

                // if (maze[x, z].WallBottom) currentPath[x, z].GetComponent<Way>().Up.SetActive(false);
                // else                       currentPath[x, z].GetComponent<Way>().Up.SetActive(true);

                // if (x > 0)
                //     if (maze[x - 1, z].WallBottom) currentPath[x, z].GetComponent<Way>().Bottom.SetActive(false);
                //     else                         currentPath[x, z].GetComponent<Way>().Bottom.SetActive(true);
                
                // if (z > 0)
                //     if (maze[x, z - 1].WallLeft) currentPath[x, z].GetComponent<Way>().Right.SetActive(false);
                //     else                           currentPath[x, z].GetComponent<Way>().Right.SetActive(true);
            }
        }
        return GOMaze;
    }
    // Generate Way
    public GameObject[,] GenerateWay()
    {
        GameObject[,] WayObj = new GameObject[CountHeight, CountWight];
        for (int x = 0; x < CountHeight; ++x)
        {
            for (int z = 0; z < CountWight; ++z)
            {
                WayObj[x, z] = Instantiate(WayPrefab, new Vector3(PlayerRast + 1.7f + x * Wight, 0, -(Height * CountHeight / 2 - 1.7f) + z * Height), quaternion.identity);
                Way c = WayObj[x, z].GetComponent<Way>();
                c.Left.SetActive(false);
                c.Right.SetActive(false);
                c.Up.SetActive(false);
                c.Bottom.SetActive(false);
                c.StraightAhead.SetActive(false);
            }
        }
        return WayObj;
    }
    // Generate Level wihout Maze
    private GameObject createBlock() 
    {
        Rast += 1f * Height * CountHeight / 2;
        GameObject curObj = Instantiate(blockPrefab, new Vector3(Rast, 0, 0), quaternion.identity);
        Block curBlock = curObj.GetComponent<Block>();

        curBlock.Floor.transform.localScale = new Vector3(Height * CountHeight / 10, 1, Height * CountHeight / 10);

        curBlock.WallLeft.transform.localScale = new Vector3(Height * CountHeight, 4.4f, 0.35f);
        curBlock.WallLeft.transform.position = new Vector3(Rast, 2.2f, Height * CountHeight / 2  + 0.175f);

        curBlock.WallRight.transform.localScale = new Vector3(Height * CountHeight, 4.4f, 0.35f);
        curBlock.WallRight.transform.position = new Vector3(Rast, 2.2f, (-1) * Height * CountHeight / 2  - 0.175f);


        curBlock.WallWithDoorLeft.transform.localScale = new Vector3(0.35f, 4.4f, Height * CountHeight / 2 - 1.6f);
        curBlock.WallWithDoorLeft.transform.position = new Vector3(Rast - 1f * Height * CountHeight / 2 - 0.175f, 2.2f, (Height * CountHeight / 2 - 1.6f) / 2 + 1.6f);
        
        curBlock.WallWithDoorRight.transform.localScale = new Vector3(0.35f, 4.4f, Height * CountHeight / 2 - 1.7f);
        curBlock.WallWithDoorRight.transform.position = new Vector3(Rast - 1f * Height * CountHeight / 2 - 0.175f, 2.2f, -(Height * CountHeight / 2 - 1.7f) / 2 - 1.7f);
        
        curBlock.WallWithDoorUp.transform.position = new Vector3(Rast - 1f * Height * CountHeight / 2 - 0.175f, 3.9f, -0.05f);
        
        Rast += 1f * Height * CountHeight / 2;

        return curObj;
    }
    // Find Way to next level whit DFS
    private void ShowWay()
    {
        Tuple<int,int>[,] Path = new Tuple<int, int>[previousGOMaze.GetLength(0),previousGOMaze.GetLength(1)];
        Stack<Tuple<int, int>> q = new Stack<Tuple<int, int>>();
        isEnd = false;

        Tuple<int, int> start = WhereIsCharacter();
        q.Push(start);
        bool [,] visited = new bool[previousGOMaze.GetLength(0),previousGOMaze.GetLength(1)];
        visited[start.Item1, start.Item2] = true;
        for (int i = 0; i < previousGOMaze.GetLength(0); ++i)
            for (int j = 0; j < previousGOMaze.GetLength(1); ++j)
                visited[i, j] = false;
        

        while (q.Count > 0)
        {
            Tuple<int,int> cur = q.Peek();
            q.Pop();
            if (cur.Item1 == previousGOMaze.GetLength(0) - 1 && cur.Item2 == previousGOMaze.GetLength(1) /2 )
            {
                break;
            }
            // Debug.Log(cur.Item1+ " " + cur.Item2);
            if (cur.Item1 + 1 < previousGOMaze.GetLength(0))
            {
                if (!previousMaze[cur.Item1, cur.Item2].WallBottom)
                {
                    if (!visited[cur.Item1 + 1, cur.Item2])
                    {
                        Path[cur.Item1 + 1, cur.Item2] = Tuple.Create(cur.Item1, cur.Item2);
                        q.Push(new Tuple<int, int>(cur.Item1 + 1, cur.Item2));
                        visited[cur.Item1 + 1, cur.Item2] = true;
                    }
                }
            }
            if (cur.Item2 + 1 < previousGOMaze.GetLength(0))
            {

                if (!previousMaze[cur.Item1, cur.Item2].WallLeft)
                {
                    if (!visited[cur.Item1, cur.Item2 + 1])
                    {
                        Path[cur.Item1, cur.Item2 + 1] = Tuple.Create(cur.Item1, cur.Item2);
                        q.Push(new Tuple<int, int>(cur.Item1, cur.Item2 + 1));
                        visited[cur.Item1, cur.Item2 + 1] = true;
                    }
                }
            }
            if (cur.Item1 > 0)
            {
                if (!previousMaze[cur.Item1 - 1, cur.Item2].WallBottom)
                {
                    if (!visited[cur.Item1 - 1, cur.Item2])
                    {
                        Path[cur.Item1 - 1, cur.Item2] = Tuple.Create(cur.Item1, cur.Item2);
                        q.Push(new Tuple<int, int>(cur.Item1 - 1, cur.Item2));
                        visited[cur.Item1 - 1, cur.Item2] = true;
                    }
                }
            }
            if (cur.Item2 > 0)
            {
                if (!previousMaze[cur.Item1, cur.Item2 - 1].WallLeft)
                {
                    if (!visited[cur.Item1, cur.Item2 - 1])
                    {
                        Path[cur.Item1, cur.Item2 - 1] = Tuple.Create(cur.Item1, cur.Item2);
                        q.Push(new Tuple<int, int>(cur.Item1, cur.Item2 - 1));
                        visited[cur.Item1, cur.Item2 - 1] = true;
                    }
                }
            }
        }

        Tuple<int,int> curM = new Tuple<int, int>(previousGOMaze.GetLength(0) - 1, previousGOMaze.GetLength(1) / 2);
        previousPath[curM.Item1, curM.Item2].GetComponent<Way>().StraightAhead.SetActive(true);
        while (curM.Item1 != start.Item1 || curM.Item2 != start.Item2)
        {
            if (Path[curM.Item1, curM.Item2].Item1 > curM.Item1)
            {
                previousPath[curM.Item1, curM.Item2].GetComponent<Way>().Up.SetActive(true);
                previousPath[Path[curM.Item1, curM.Item2].Item1, Path[curM.Item1, curM.Item2].Item2].GetComponent<Way>().Bottom.SetActive(true);
            }
            else if (Path[curM.Item1, curM.Item2].Item1 < curM.Item1)
            {
                previousPath[curM.Item1, curM.Item2].GetComponent<Way>().Bottom.SetActive(true);
                previousPath[Path[curM.Item1, curM.Item2].Item1, Path[curM.Item1, curM.Item2].Item2].GetComponent<Way>().Up.SetActive(true);
            }
            else if (Path[curM.Item1, curM.Item2].Item2 > curM.Item2)
            {
                previousPath[curM.Item1, curM.Item2].GetComponent<Way>().Left.SetActive(true);
                previousPath[Path[curM.Item1, curM.Item2].Item1, Path[curM.Item1, curM.Item2].Item2].GetComponent<Way>().Right.SetActive(true);
            }
            else
            {
                previousPath[curM.Item1, curM.Item2].GetComponent<Way>().Right.SetActive(true);
                previousPath[Path[curM.Item1, curM.Item2].Item1, Path[curM.Item1, curM.Item2].Item2].GetComponent<Way>().Left.SetActive(true);
            }
            curM = Path[curM.Item1, curM.Item2];
        }
    }

    private Tuple<int,int> WhereIsCharacter()
    {
        int x = (int)((Character.position.x - PlayerPrRast) / Height);
        int y = (int)((Character.position.z + (Height * previousGOMaze.GetLength(0) / 2)) / Height);
        return new Tuple<int, int>(x, y);
    }
}
