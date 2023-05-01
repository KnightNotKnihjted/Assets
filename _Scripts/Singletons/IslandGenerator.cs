using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandGenerator : SingletonBehaviour<IslandGenerator>
{
    public bool DEBUG = false;

    //Noise Config
    [SerializeReference]
    [SerializeField] private int seed;
    public NoiseConfigBase noiseConfig;
    private int width;
    private int height;
    [Range(0,1)]
    [SerializeField] private float oceanPeakHeight;

    //Tilemaps
    [SerializeField] private Tilemap oceanTilemap;
    [SerializeField] private Tilemap shallowTilemap;
    [SerializeField] private Tilemap riverStartPointTilemap;
    [SerializeField] private Tilemap landTilemap;
    [SerializeField] private Tilemap structureTilemap;
    [SerializeField] private Tilemap treesTilemap;

    //Other
    [SerializeField] private TileBase landTile;
    [SerializeField] private TileBase waterTile;
    //Structure Variables
    public bool generateRiver;
    [SerializeField] private TileBase riverTile;
    [SerializeField] private TileBase riverStartPointTile;
    [SerializeField] private int riverCount = 2;
    [SerializeField] private int riverLengthMax = 4;

    public bool generateVillages;
    [SerializeField] private int villagesCount;
    [SerializeField] private Vector2Int villageSize;
    [SerializeField] private TileBase villageEmptyHolderTile;
    [SerializeField] private TileBase villageTile;

    public bool generateTrees;
    [SerializeField] private NoiseConfigBase treesNoise;
    [SerializeField] private WoodType[] treeTypes;

    public bool generateCaves;
    [SerializeField] private int cavesCount;
    [SerializeField] private TileBase caveTile;
    [SerializeField] private int minDistFromCentre = 8;
    [SerializeField] private int maxDistFromCentre = 48;

    public bool generateFountain;
    [SerializeField] private TileBase fountainTile;
    [SerializeField] private float fountainSpawnRate;

    private System.Random random;

    private GameObject texDebug;

    private void Update()
    {
        if(noiseConfig.Update && generateVillages != true)
        {
            StartCoroutine(GenerateIsland());
        }
    }
    [ContextMenu("Generate Island")]
    public IEnumerator GenerateIsland()
    {
        random = new System.Random(seed);

        GeneratePerlinNoiseIsland();
        //Structures
        if (generateCaves)
            yield return new WaitUntil(PlaceCaves);
        if(generateVillages)
            yield return new WaitUntil(PlaceVillages);
        if (generateTrees)
            yield return new WaitUntil(GenerateTrees);
        if (generateFountain)
            yield return new WaitUntil(PlaceFountains);
        if (generateRiver)
            yield return new WaitUntil(GenerateRivers);
    }
    private bool GenerateTrees()
    {
        List<Vector3Int> emptyLandTiles = GetEmptyLandTiles();
        Texture2D tex = new (width, height);

        // Adjust these values to control the clustering of trees
        float clusterFrequency = 0.05f;
        float clusterStrength = 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int position = new (x, y, 0);
                float noise = treesNoise.GetNoiseValue(x, y);
                int clusterX = Mathf.RoundToInt(x * clusterFrequency);
                int clusterY = Mathf.RoundToInt(y * clusterFrequency);
                float clusterNoise = treesNoise.GetNoiseValue(clusterX, clusterY);

                // Multiply the noise by the cluster noise to create larger areas of higher or lower values
                float combinedNoise = noise * (1 + clusterStrength * clusterNoise);

                int i = 0;
                foreach (WoodType woodType in treeTypes)
                {
                    if (emptyLandTiles.Contains(position) && combinedNoise > woodType.ratioOfTotalLandTaken && !TreeInCardinalDirections(position))
                    {
                        if (DEBUG)
                        {
                            Color treeColor = new (combinedNoise, i * 30f, 0, 1); // You can customize this color for each tree type if you want
                            tex.SetPixel(x, y, treeColor);
                        }
                        else
                        {
                            treesTilemap.SetTile(position, woodType.treeTile);
                        }
                    }
                    i++;
                }
            }
        }

        if (DEBUG)
        {
            tex.Apply();
            if (texDebug == null)
            {
                texDebug = new GameObject("TEX-DEBUG");
                texDebug.transform.position = new Vector3(width / 2, height / 2, 0);
                texDebug.transform.localScale = new Vector3(width, height, 1);
            }
            SpriteRenderer sr = texDebug.GetComponent<SpriteRenderer>();
            if (sr == null) sr = texDebug.AddComponent<SpriteRenderer>();
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            sr.sprite = sprite;
        }

        return true;
    }
    private bool TreeInCardinalDirections(Vector3Int position)
    {
        Vector3Int[] cardinalDirections = new[]
        {
        position + Vector3Int.up,
        position + Vector3Int.up * 2,
        position + Vector3Int.up + Vector3Int.left,
        position + Vector3Int.up + Vector3Int.right,
        position + Vector3Int.down,
        position + Vector3Int.down * 2,
        position + Vector3Int.down + Vector3Int.left,
        position + Vector3Int.down + Vector3Int.right,
        position + Vector3Int.left,
        position + Vector3Int.left * 2,
        position + Vector3Int.right,
        position + Vector3Int.right * 2
    };

        foreach (Vector3Int direction in cardinalDirections)
        {
            if (treesTilemap.HasTile(direction))
            {
                return true;
            }
        }

        return false;
    }

    void GeneratePerlinNoiseIsland()
    {
        oceanTilemap.ClearAllTiles();
        shallowTilemap.ClearAllTiles();
        riverStartPointTilemap.ClearAllTiles();
        landTilemap.ClearAllTiles();
        structureTilemap.ClearAllTiles();
        treesTilemap.ClearAllTiles();

        width = noiseConfig.Width;
        height = noiseConfig.Height;


        Texture2D tex = new (width, height);
        if (GameObject.Find("TEX-DEBUG") != null && !DEBUG) Destroy(GameObject.Find("TEX-DEBUG"));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {


                float noise = noiseConfig.GetNoiseValue(x, y, seed);

                Vector3Int pos = new(x, y, 0);

                if (!DEBUG)
                {
                    /*River S*/
                    if (noise > oceanPeakHeight - 0.0125f && noise < oceanPeakHeight + 0.025f)
                    {
                        riverStartPointTilemap.SetTile(pos, riverStartPointTile);
                    }
                    /*Land */
                    if (noise > oceanPeakHeight + 0.025f)
                    {
                        landTilemap.SetTile(pos, landTile);
                    }
                    /*Ocean*/
                    else
                    {
                        oceanTilemap.SetTile(pos, waterTile);
                    }
                }
                else
                {
                    tex.SetPixel(x, y, new Color(noise, noise, noise, 1));
                }
            }
        }
        if (DEBUG)
        {
            tex.Apply();
            if (texDebug == null)
            {
                texDebug = new GameObject("TEX-DEBUG");
                texDebug.transform.position = new(width / 2, height / 2, 0);
                texDebug.transform.localScale = new(width, height, 1);
            }
            SpriteRenderer sr = texDebug.GetComponent<SpriteRenderer>();
            if (sr == null) sr = texDebug.AddComponent<SpriteRenderer>();
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            sr.sprite = sprite;
        }
    }
    List<Vector3Int> GetEmptyLandTiles()
    {
        List<Vector3Int> emptyTiles = new ();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int position = new (x, y, 0);
                if (landTilemap.GetTile(position) == landTile && structureTilemap.GetTile(position) == null && treesTilemap.HasTile(position) == false)
                {
                    emptyTiles.Add(position);
                }
            }
        }

        return emptyTiles;
    }
    List<Vector3Int> GetRiverStartPoints()
    {
        List<Vector3Int> emptyTiles = new ();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int position = new (x, y, 0);
                if (riverStartPointTilemap.GetTile(position) == riverStartPointTile)
                {
                    emptyTiles.Add(position);
                }
            }
        }

        return emptyTiles;
    }
    bool PlaceCaves()
    {
        Vector2Int center = new(width / 2, height / 2);
        int maxAttempts = 8;
        int attempts = maxAttempts;
        for (int i = 0; i < cavesCount; i++)
        {
            if (attempts <= 0)
            {
                attempts = maxAttempts;
                continue;
            }

            int x = random.Next(center.x - maxDistFromCentre, center.x + maxDistFromCentre + 1);
            int y = random.Next(center.y - maxDistFromCentre, center.y + maxDistFromCentre + 1);

            Vector3Int cavePosition = new(x, y, 0);

            // Check if the cave position is within the desired radius
            int distanceFromCenter = Mathf.RoundToInt(Vector2Int.Distance(center, new Vector2Int(x, y)));
            if (distanceFromCenter < minDistFromCentre || distanceFromCenter > maxDistFromCentre)
            {
                i--; // Try again if the cave is outside the desired radius
                attempts--;
                continue;
            }

            if (GetEmptyLandTiles().Contains(cavePosition))
            {
                structureTilemap.SetTile(cavePosition, caveTile);
                if (DEBUG)
                    print($"Generated Cave {i + 1} At: {cavePosition}");
                attempts = maxAttempts;
            }
            else
            {
                i--; // Try again if the selected position is not on land
                attempts--;
            }
        }
        return true;
    }
    bool GenerateRivers()
    {
        List<Vector3Int> riverStarts = GetRiverStartPoints();
        for (int i = 0; i < riverCount; i++)
        {
            // Check if the starting point is on land
            Vector3Int startPosition = riverStarts[random.Next(0,riverStarts.Count)];
            riverStarts.Remove(startPosition);

            int riverLength = riverLengthMax;

            // Generate the river path
            Vector3Int currentPosition = startPosition;
            int direction = 0;
            int gigAttempts = 5;
            for (int j = 0; j < riverLength; j++)
            {
                // Place the river tile
                shallowTilemap.SetTile(currentPosition, riverTile);
                landTilemap.SetTile(currentPosition, null);
                oceanTilemap.SetTile(currentPosition, null);

                // Move to the next position
                int attempts = 8;
                while (landTilemap.GetTile(currentPosition) != landTile && attempts > 0)
                {
                    int newDirection = random.Next(0, 4);
                    if (newDirection == direction)
                    {
                        newDirection++;
                    }
                    direction = newDirection;

                    switch (direction)
                    {
                        case 0: // Up
                            currentPosition += Vector3Int.up;
                            break;
                        case 1: // Down
                            currentPosition += Vector3Int.down;
                            break;
                        case 2: // Left
                            currentPosition += Vector3Int.left;
                            break;
                        case 3: // Right
                            currentPosition += Vector3Int.right;
                            break;
                    }

                    // Keep the river within bounds
                    currentPosition.x = Mathf.Clamp(currentPosition.x, 0, width - 1);
                    currentPosition.y = Mathf.Clamp(currentPosition.y, 0, height - 1);

                    //End when we hit the ocean
                    if (oceanTilemap.HasTile(currentPosition))
                    {
                        attempts = 0;
                        j = riverLength - 1;
                    }

                    attempts--;
                    if (gigAttempts > 0)
                    {
                        gigAttempts--;
                        j--;
                    }
                }

            }
        }
        return true;
    }
    bool PlaceVillages()
    {

        int placedVillages = 0;
        while (placedVillages < villagesCount)
        {
            int x = random.Next(0, width - villageSize.x);
            int y = random.Next(0, height - villageSize.y);

            // Check if the entire village can fit in the land
            bool fits = true;
            for (int i = 0; i < villageSize.x && fits; i++)
            {
                for (int j = 0; j < villageSize.y && fits; j++)
                {
                    Vector3Int position = new(x + i, y + j, 0);
                    if (GetEmptyLandTiles().Contains(position) == false)
                    {
                        fits = false;
                        i = villageSize.x - 1;
                        j = villageSize.y - 1;
                    }
                }
            }

            if (fits)
            {
                // Place the village
                for (int i = 0; i < villageSize.x; i++)
                {
                    for (int j = 0; j < villageSize.y; j++)
                    {
                        Vector3Int position = new(x + i, y + j, 0);
                        structureTilemap.SetTile(position, villageEmptyHolderTile);
                    }
                }
                Vector3Int pos = new(x, y, 0);
                if(DEBUG)
                    print($"Generated Village {placedVillages + 1} At: {pos}");
                structureTilemap.SetTile(pos, villageTile);
                placedVillages++;
            }
        }
        return true;
    }
    bool PlaceFountains()
    {
        for (int x = 0; x < width; x += 16)
        {
            for (int y = 0; y < height; y += 16)
            {
                if (random.NextDouble() < fountainSpawnRate)
                {
                    int fountainX = random.Next(x, x + 16);
                    int fountainY = random.Next(y, y + 16);

                    Vector3Int position = new(fountainX, fountainY, 0);
                    if (landTilemap.GetTile(position) == landTile)
                    {
                        structureTilemap.SetTile(position, fountainTile);
                        if(DEBUG)
                            print($"Generated Fountain At: {position}");
                    }
                }
            }
        }
        return true;
    }
}
