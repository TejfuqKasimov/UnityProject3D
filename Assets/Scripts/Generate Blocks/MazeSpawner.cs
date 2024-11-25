using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject blockPrefab;
    public GameObject currentBlock;
    public GameObject previousBlock;
    public GameObject previousPrBlock = null;
    public GameObject startRoomPrefab;
    public GameObject startRoom;
    GameObject[,] currentGOMaze = new GameObject[11, 11];
    GameObject[,] previousGOMaze = new GameObject[11, 11];
    GameObject[,] previousPrGOMaze = new GameObject[11, 11];
    private float Wight = 3.4f;
    private float Height = 3.4f;
    public Transform obj;
    private int LevelCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startRoom = Instantiate(startRoomPrefab, Vector3.zero, quaternion.identity);
        currentBlock = Instantiate(blockPrefab, new Vector3(24.2f + 37.4f * LevelCount, 0, 0), quaternion.identity);
        currentGOMaze = Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (obj.position.x >= LevelCount * Height * 11 + 5.5f)
        {
            previousPrBlock = previousBlock;
            previousBlock = currentBlock;
            LevelCount++;
            currentBlock = Instantiate(blockPrefab, new Vector3(24.2f + 37.4f * LevelCount, 0, 0), quaternion.identity);
            previousPrGOMaze = previousGOMaze;
            previousGOMaze = currentGOMaze;
            currentGOMaze = Generate();

            if (previousPrBlock != null)
            {
                Destroy(previousPrBlock);
            }
            if (startRoom != null)
            {
                Destroy(startRoom);
            }

            for (int x = 0; x < previousPrGOMaze.GetLength(0); ++x)
            {
                for (int z = 0; z < previousPrGOMaze.GetLength(1); ++z)
                {
                    if (previousPrGOMaze[x, z] != null)
                    {
                        Destroy(previousPrGOMaze[x, z]);
                    }
                }
            }
        }
    }

    private GameObject[,] Generate()
    {
        MazeGenerator generator = new MazeGenerator();
        MazeGeneratorCell[,] maze = generator.GenerateMaze();
        GameObject[,] GOMaze = new GameObject[maze.GetLength(0), maze.GetLength(1)];
        for (int x = 0; x < maze.GetLength(0); ++x)
        {
            for (int z = 0; z < maze.GetLength(1); ++z)
            {
                GOMaze[x, z] = Instantiate(cellPrefab, new Vector3(7.2f + LevelCount * Height * 11 + x * Wight, 0, -17f + z * Height), quaternion.identity);
                Cell c = GOMaze[x, z].GetComponent<Cell>();
                c.WallLeft.SetActive(maze[x, z].WallLeft);
                c.WallBottom.SetActive(maze[x, z].WallBottom);
            }
        }
        return GOMaze;
    }
}
