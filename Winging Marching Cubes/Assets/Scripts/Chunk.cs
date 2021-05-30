using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    int width = WorldGenerator.chunkWidth;
    int height = WorldGenerator.chunkHeight;

	float terrainSurface = 0.5f;
	float[,,] terrainMap = WorldGenerator.terrainMap;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

	GameObject chunkObject;
	MeshFilter meshFilter;
	MeshCollider meshCollider;
	MeshRenderer meshRenderer;

	public ChunkCoord coord;

	WorldGenerator worldGen;

	public Chunk (ChunkCoord _coord, WorldGenerator _world)
    {
		coord = _coord;
		worldGen = _world;

		chunkObject = new GameObject();

		meshFilter = chunkObject.AddComponent<MeshFilter>();
		meshCollider = chunkObject.AddComponent<MeshCollider>();
		meshRenderer = chunkObject.AddComponent<MeshRenderer>();
		meshRenderer.material = worldGen.material;

		chunkObject.transform.SetParent(worldGen.transform);
		chunkObject.transform.position = new Vector3(coord.x * WorldGenerator.chunkWidth, 0f, coord.z * WorldGenerator.chunkWidth);
		chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

		PopulateTerrainMap();
		CreateMeshData();
		BuildMesh();
	}
	

	void PopulateTerrainMap()
    {
		ClearMeshData();

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
					terrainMap[x, y, z] = worldGen.GetBlock(new Vector3(x, y, z) + Position);
                }
            }
        }
    }

	void CreateMeshData()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
					float[] cube = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
						Vector3Int corner = new Vector3Int(x, y, z) + Data.CornerTable[i];

						cube[i] = terrainMap[corner.x, corner.y, corner.z];
                    }
					MarchCube(new Vector3(x, y, z), cube);
                }
            }
        }
    }

	int GetCubeConfig (float[] cube)
    {
		int configIndex = 0;

        for (int cornerNum = 0; cornerNum < 8; cornerNum++)
        {
			if (cube[cornerNum] > terrainSurface)
            {
				configIndex |= 1 << cornerNum;
            }
        }

		return configIndex;
    }

	void MarchCube(Vector3 position, float[] cube)
    {
		int configIndex = GetCubeConfig(cube);

		if (configIndex == 0 || configIndex == 255)
			return;

		int edgeIndex = 0;

        for (int tri = 0; tri < 5; tri++)
        {
            for (int p = 0; p < 3; p++)
            {
				int indice = Data.TriangleTable[configIndex, edgeIndex];

				if (indice == -1)
					return;

				Vector3 vert1 = position + Data.EdgeTable[indice, 0];
				Vector3 vert2 = position + Data.EdgeTable[indice, 1];

				Vector3 vertPosition = (vert1 + vert2) / 2;

				vertices.Add(vertPosition);
				triangles.Add(vertices.Count - 1);

				edgeIndex++;
			}
		}

    }

	void ClearMeshData()
	{
		vertices.Clear();
		triangles.Clear();
	}

	void BuildMesh()
    {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = mesh;
    }

	bool IsBlockInChunk(int x, int y, int z)
    {
		if (x < 0 || x > WorldGenerator.chunkWidth - 1 || y < 0 || y > WorldGenerator.chunkHeight - 1 || z < 0 || z > WorldGenerator.chunkWidth - 1)
			return false;
		else
			return true;
    }

	public bool IsActive
    {
		get { return chunkObject.activeSelf; }
		set { chunkObject.SetActive(value); }
    }

	public Vector3 Position
    {
		get { return chunkObject.transform.position; }
    }
}

public class ChunkCoord
{
	public int x;
	public int z;

	public ChunkCoord(int _x, int _z)
    {
		x = _x;
		z = _z;
    }

	public bool Equals (ChunkCoord other)
    {
		if (other == null)
			return false;
		else if (other.x == x && other.z == z)
			return true;
		else
			return false;
    }

} 