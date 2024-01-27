using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWall : MonoBehaviour
{
    [SerializeField] GatitoManager player;
    [SerializeField] CameraScript cam;

    [SerializeField] GameObject cube;

    List<List<Vector3>> squarePositions = new();
    [SerializeField] int rdmSpawnPrc;

    public GameObject SpawnObstacle(GameObject obstacle)
    {
        // Spawning obstacles randomly is pretty hard to do. In this case there are basically 3 steps to do it, each of them being a function by themselves.
        // First we must know where the obstacles are allowed to spawn
        GetPotentialSquares();

        // Then remove a line so the player can 100% fit through (if it was completely random there could be some cases where it's impossible to dodge)
        RemoveLine();

        // For other blocks make it random so it looks more natural
        SpawnBlocksRandomly();

        // Now we can finally spawn the blocks
        foreach (List<Vector3> row in squarePositions)
        {
            foreach (Vector3 col in row)
            {
                GameObject newCube = Instantiate(cube, col + obstacle.transform.position, Quaternion.identity);
                newCube.transform.parent = obstacle.transform;
            }
        }

        return obstacle;
    }

    void GetPotentialSquares()
    {
        // Getting all rows that'll have obstacles
        List<int> rowPositions = new();
        for (int i = 1; i < player.length + 2; i++)
        {
            rowPositions.Add(i);
        }

        // Same thing for columns (the base position is different)
        float basePos = -3;
        List<float> columnPositions = new();
        for (float i = basePos; i <= -basePos; i++)
        {
            columnPositions.Add(i);
        }

        // Now, we can get store every single square in an organized 2D list
        squarePositions = new();
        foreach (int row in rowPositions)
        {
            squarePositions.Add(new List<Vector3>());

            foreach (float column in columnPositions)
            {
                squarePositions[^1].Add(new Vector3(column, row));
            }
        }
    }

    void RemoveLine()
    {
        int emptySpace = Mathf.CeilToInt(player.length) + 1;
        int blocksDestroyed = 0;

        // Pick either 0, 1 or 2; 0 or 1 means horizontal and 2 means vertical
        int direction = Random.Range(0, 3);

        if (direction == 1 || direction == 2)
        {
            // Choose a random column (excluding the last one)
            int indexToRemoveAt = Random.Range(0, squarePositions[0].Count - 1);
            foreach (List<Vector3> row in squarePositions)
            {
                if (blocksDestroyed >= emptySpace)
                {
                    break;
                }
                // for each row remove the block located at the index, as well as the one to its right
                squarePositions[squarePositions.IndexOf(row)].RemoveAt(indexToRemoveAt);
                squarePositions[squarePositions.IndexOf(row)].RemoveAt(indexToRemoveAt);

                blocksDestroyed++;
            }
            return;
        }

        float endingPoint = 3;

        // With these variables we can guess which blocks are allowed to be at the middle of a gap (we store them in a list)
        List<float> middleBlocks = new();
        for (float midGap = -endingPoint; midGap <= endingPoint; midGap++)
        {
            middleBlocks.Add(midGap);
        }

        // Choose a random value in the list, and remove the blocks around it
        float randBlock = middleBlocks[Random.Range(0, middleBlocks.Count - 1)];
        float blockOffset = (Mathf.Approximately(randBlock, (int)randBlock)) ? 0 : -.5f;

        for (blocksDestroyed = 0; blocksDestroyed < emptySpace; blocksDestroyed++)
        {
            Vector3 blockToRemove = new(randBlock + blockOffset, 1);
            squarePositions[0].Remove(blockToRemove);
            blockOffset = (blockOffset >= 0) ? -blockOffset - 1 : -blockOffset;
        }
    }

    void SpawnBlocksRandomly()
    {
        // Note : I can't remove the blocks inside the foreach loop, because it modifies the length of the squarePositions list and messes up the program
        List<int> rowToRemoveAt = new();
        List<Vector3> blockToRemove = new();

        foreach (List<Vector3> row in squarePositions)
        {
            foreach (Vector3 block in row)
            {
                // Pick a random value between 0 and 99, if this value is higher than the spawn percentage then we destroy the block
                int rdmPrc = Random.Range(0, 100);
                if (rdmPrc >= rdmSpawnPrc)
                {
                    rowToRemoveAt.Add(squarePositions.IndexOf(row));
                    blockToRemove.Add(block);
                }
            }
        }

        for (int i = 0; i < blockToRemove.Count; i++)
        {
            squarePositions[rowToRemoveAt[i]].Remove(blockToRemove[i]);
        }
    }
}
