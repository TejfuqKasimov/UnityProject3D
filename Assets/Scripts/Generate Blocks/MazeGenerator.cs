using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class MazeGeneratorCell
{
    public int X;
    public int Y;
    public bool WallLeft = true;
    public bool WallBottom = true;
    public bool Visited = false;
}

public class MazeGenerator
{
    int Width;
    int Height;
    public MazeGeneratorCell[,] GenerateMaze(int TmpWidth, int TmpHeight)
    {
        Width = TmpWidth;
        Height = TmpHeight;
        MazeGeneratorCell[,] maze = new MazeGeneratorCell[Width, Height];

        for (int x = 0; x < maze.GetLength(0); ++x)
        {
            for (int y = 0; y < maze.GetLength(1); ++y)
            {
                maze[x, y] = new MazeGeneratorCell { X = x, Y = y };
            }
        }

        RemoveWallsWithBacktracker(maze);

        maze[maze.GetLength(0) - 1, (maze.GetLength(1) - 1) / 2 + (maze.GetLength(0) - 1) % 2].WallBottom = false;

        return maze;
    }

    private void RemoveWallsWithBacktracker(MazeGeneratorCell[,] maze)
    {
        MazeGeneratorCell cur = maze[0, (maze.GetLength(1) - 1) / 2 + (maze.GetLength(0) - 1) % 2];
        cur.Visited = true;

        Stack<MazeGeneratorCell> stack = new Stack<MazeGeneratorCell>();

        do
        {
            List<MazeGeneratorCell> unvisitedNeighbours = new List<MazeGeneratorCell>();

            if (cur.X > 0 && !maze[cur.X - 1, cur.Y].Visited)
            {
                unvisitedNeighbours.Add(maze[cur.X - 1, cur.Y]);
            }
            if (cur.Y > 0 && !maze[cur.X, cur.Y - 1].Visited)
            {
                unvisitedNeighbours.Add(maze[cur.X, cur.Y - 1]);
            }
            if (cur.Y + 1 < Width && !maze[cur.X, cur.Y + 1].Visited)
            {
                unvisitedNeighbours.Add(maze[cur.X, cur.Y + 1]);
            }
            if (cur.X + 1 < Height && !maze[cur.X + 1, cur.Y].Visited)
            {
                unvisitedNeighbours.Add(maze[cur.X + 1, cur.Y]);
            }


            if (unvisitedNeighbours.Count > 0)
            {
                MazeGeneratorCell chosen = unvisitedNeighbours[UnityEngine.Random.Range(0, unvisitedNeighbours.Count)];
                RemoveWall(cur, chosen);
                chosen.Visited = true;
                cur = chosen;
                stack.Push(cur);
            }
            else
            {
                cur = stack.Pop();
            }
        } while (stack.Count > 0);
    }

    private void RemoveWall(MazeGeneratorCell cur, MazeGeneratorCell chosen)
    {
        if (cur.X == chosen.X)
        {
            if (cur.Y < chosen.Y) cur.WallLeft = false;
            else chosen.WallLeft = false;
        }
        else
        {
            if (cur.X < chosen.X) cur.WallBottom = false;
            else chosen.WallBottom = false;
        }
    }
}
