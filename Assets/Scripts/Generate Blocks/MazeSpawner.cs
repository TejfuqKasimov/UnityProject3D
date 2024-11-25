using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public GameObject cellPrefab;
    private float Wight = 3.4f;
    private float Height = 3.4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MazeGenerator generator = new MazeGenerator();
        MazeGeneratorCell[,] maze = generator.GenerateMaze();
        for (int x = 0; x < maze.GetLength(0); ++x)
        {
            for (int z = 0; z < maze.GetLength(1); ++z)
            {
                Cell c = Instantiate(cellPrefab, new Vector3(7.2f + x * Wight, 0, -17f + z * Height), quaternion.identity).GetComponent<Cell>();
                c.name = x + ", " + z;
                c.WallLeft.SetActive(maze[x, z].WallLeft);
                c.WallBottom.SetActive(maze[x, z].WallBottom);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
