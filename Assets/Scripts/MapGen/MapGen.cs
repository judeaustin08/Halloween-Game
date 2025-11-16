using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGen : MonoBehaviour
{
    public float spacing = 3f;
    [Header("Tile Information")]
    public Tile[] tiles;
    public GameObject[] chunks;

    [Header("Map Dimensions")]
    public Vector2Int totalDimensions = new Vector2Int(3, 3);
    public Vector3 offset;

    public void getDimensions()
    {
        //totalDimensions = totalDimensions;
    }

    public GameObject[] GenerateMap()
    {
        chunks = new GameObject[totalDimensions.x * totalDimensions.y];

        GameObject environmentParent = GameObject.Find("Environment");
        if (environmentParent == null)
        {
            environmentParent = new GameObject("Environment");
        }

        for (int z = 0; z < totalDimensions.y; z++)
        {
            for (int x = 0; x < totalDimensions.x; x++)
            {
                int index = z * totalDimensions.x + x;
                Tile selectedTile = SelectRandomTileForPosition(x, z);
                if (selectedTile.prefab != null)
                {
                    GameObject chunk = Instantiate(selectedTile.prefab, new Vector3(x * spacing, 0, z * spacing), Quaternion.identity);
                    chunk.transform.SetParent(environmentParent.transform);
                    chunks[index] = chunk;

                }
            }
        }

        environmentParent.transform.position += offset;

        // Populate shelves
        GameObject[] shelfItems = GameManager.active.shelfItems;
        GameObject[] shelves = GameObject.FindGameObjectsWithTag("Shelf");
        foreach (GameObject g in shelves)
            Instantiate(shelfItems[Random.Range(0, shelfItems.Length - 1)], g.transform.position, g.transform.rotation, g.transform);

        StartCoroutine(createAStarGraph());
        return chunks;
    }
    
    public void DeleteMap()
    {
        if (chunks == null || chunks.Length == 0)
            return;

        foreach (var chunk in chunks)
        {
            if (chunk != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Object.DestroyImmediate(chunk);
                else
                    Destroy(chunk);
#else
            Destroy(chunk);
#endif
            }
        }
        chunks = new GameObject[0];
    }

    private IEnumerator createAStarGraph()
    {
        yield return new WaitForEndOfFrame();
        
        AstarPath.active.Scan();
    }

    private Tile SelectRandomTileForPosition(int x, int z)
    {
        //get all possible tiles for a given place
        List<Tile> availableTiles = new List<Tile>();
        foreach (var tile in tiles)
        {
            int positionIndex = z * totalDimensions.x + x;

            if (tile.constraints[positionIndex])
            {
                availableTiles.Add(tile);
            }
        }
        //pick a random tile from possible tiles
        if (availableTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, availableTiles.Count);
            return availableTiles[randomIndex];
        }

        return new Tile();
    }

}
