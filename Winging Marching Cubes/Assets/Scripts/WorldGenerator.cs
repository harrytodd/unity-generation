using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public Material material;
    public Transform player;

    public Vector3 spawnPosition;

    public static int chunkWidth = 16;
    public static int chunkHeight = 64;
    public static int worldSeed = 12;
    public static float frequency = 4f;
    public static float amplitude = 0.1f;
    public static float scale = 1f;

    public static int worldSizeInChunks = 100;
    public static int worldSizeInBlocks
    {
        get { return worldSizeInChunks * chunkWidth; }
    }

    public static int viewDistanceInChunks = 5;

    public static float[,,] terrainMap = new float[chunkWidth + 1, chunkHeight + 1, chunkWidth + 1];

    Chunk[,] chunks = new Chunk[worldSizeInChunks, worldSizeInChunks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    private void Start()
    {
        spawnPosition = new Vector3((worldSizeInChunks * chunkWidth) / 2f, chunkHeight, (worldSizeInChunks * chunkWidth) / 2f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);

        if (!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();
    }

    void GenerateWorld ()
    {
        for (int x = (worldSizeInChunks / 2) - viewDistanceInChunks; x < (worldSizeInChunks / 2) + viewDistanceInChunks; x++)
        {
            for (int z = (worldSizeInChunks / 2) - viewDistanceInChunks; z < (worldSizeInChunks / 2) + viewDistanceInChunks; z++)
            {
                CreateNewChunk(new ChunkCoord(x, z));
            }
        }

        player.position = spawnPosition;
    }

    ChunkCoord GetChunkCoordFromVector3 (Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / chunkWidth);
        int z = Mathf.FloorToInt(pos.z / chunkWidth);

        return new ChunkCoord(x, z);
    }

    void CheckViewDistance ()
    {
        Debug.Log("CHECKING VIEW DISTANCE");
        int chunkX = Mathf.FloorToInt(player.position.x / chunkWidth);
        int chunkZ = Mathf.FloorToInt(player.position.z / chunkWidth);

        List<ChunkCoord> prevActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = chunkX - viewDistanceInChunks; x < chunkX + viewDistanceInChunks; x++)
        {
            for (int z = chunkZ - viewDistanceInChunks; z < chunkZ + viewDistanceInChunks; z++)
            {
                if (IsChunkInWorld(x, z))
                {
                    ChunkCoord thisChunk = new ChunkCoord(x, z);

                    if (chunks[x, z] == null)
                        CreateNewChunk(thisChunk);
                    else if (!chunks[x, z].IsActive)
                    {
                        chunks[x, z].IsActive = true;
                        activeChunks.Add(new ChunkCoord(x, z));
                    }
                }

                for (int i = 0; i < prevActiveChunks.Count; i++)
                {
                    if (prevActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        prevActiveChunks.RemoveAt(i);
                }


            }
        }

        foreach (ChunkCoord c in prevActiveChunks)
        {
            chunks[c.x, c.z].IsActive = false;
        }

        playerLastChunkCoord = playerChunkCoord;
    }

    private void CreateNewChunk(ChunkCoord coord)
    {
        chunks[coord.x, coord.z] = new Chunk(new ChunkCoord(coord.x, coord.z), this);
        activeChunks.Add(coord);
    }

    public float GetBlock(Vector3 pos)
    {
        if (IsBlockInWorld(pos))
        {
            int x = (int)pos.x;
            int y = (int)pos.y;
            int z = (int)pos.z;

            float perlinValue = Noise.GetPerlinValue(x, z, worldSeed, frequency, amplitude, scale);

            float thisHeight = chunkHeight * perlinValue;

            return y - thisHeight;
        }
        else
            return 0;
    }

    bool IsChunkInWorld(int x, int z)
    {

        if (x > 0 && x < worldSizeInChunks - 1 && z > 0 && z < worldSizeInChunks - 1)
            return true;
        else
            return false;

    }

    bool IsBlockInWorld(Vector3 pos)
    {
        if (pos.x <= 0 && pos.x > worldSizeInBlocks && pos.y <= 0 && pos.y > chunkHeight && pos.z <= 0 && pos.z > worldSizeInBlocks)
            return false;
        else
            return true;
    }
}