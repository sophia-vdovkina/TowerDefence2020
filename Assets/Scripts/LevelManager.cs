using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    // <summary>
    // Префабы для создания плит
    // </summary>
    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    private CameraMovement cameraMovement;

    [SerializeField]
    private Transform map;

    private Point greenSpawn, purpleSpawn;
    /// <summary>
    /// Префаб зеленого портала
    /// </summary>
    [SerializeField]
    private GameObject greenPortalPrefab;
    /// <summary>
    /// Префаб красного портала
    /// </summary>
    [SerializeField]
    private GameObject purplePortalPrefab;

    public Portal GreenPortal { get; set; }

    // Путь по которому должны идти монстры
    private Stack<Node> path;

    // Путь который передаем монстрам
    public Stack<Node> Path
    {
        get
        {
            if (path == null)
            {
                GeneratePath();
            }
            return new Stack<Node>(new Stack<Node>(path));
        }
    }

    /// <summary>
    /// Словарь всех тайлов в игре
    /// </summary>
    public Dictionary<Point, TileScript> Tiles { get; set; }

    public float TileSize
    {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }

    public Point GreenSpawn { get => greenSpawn; set => greenSpawn = value; }

    // Start is called before the first frame update
    void Start()
    {
        CreateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // <summary>
    // Создает уровень
    // </summary>
    private void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileScript>();

        string[] mapData = ReadLevelText();

        int mapX = mapData[0].ToCharArray().Length;
        int mapY = mapData.Length;

        Vector3 maxTile = Vector3.zero;

        // производит расчет начала генерации мира
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        for (int y = 0; y < mapY; y++)
        {
            char[] newTiles = mapData[y].ToCharArray();
            for (int x = 0; x < mapX; x++) {

                PlaceTile(newTiles[x].ToString(), x, y, worldStart);
            }
        }

        maxTile = Tiles[new Point(mapX - 1, mapY - 1)].transform.position;

        // создаем пределы камеры до позиции максимального тайла
        cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));

        SpawnPortals();
    }

    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        int tileIndex = int.Parse(tileType);

        bool walkable;

        if (tileIndex == 1)
        {
            walkable = false;
        }
        else
        {
            walkable = true;
        }
        // Создаем новый тайл и помещаем ссылку на этот тайл в переменную
        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();
    
        // Использует новую переменную тайла для изменения позиции тайла
        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + TileSize * x, worldStart.y - TileSize * y, 0), map, walkable);

        

    }

    private string[] ReadLevelText() 
    {
        TextAsset bindData = Resources.Load("Level") as TextAsset;
        string data = bindData.text.Replace(Environment.NewLine, string.Empty);
        return data.Split('-');
    }

    private void SpawnPortals()
    {
        GreenSpawn = new Point(0, 0);

        GameObject tmp = (GameObject) Instantiate(greenPortalPrefab, Tiles[GreenSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
        GreenPortal = tmp.GetComponent<Portal>();
        GreenPortal.name = "GreenPortal";
        purpleSpawn = new Point(11, 6);


        Instantiate(purplePortalPrefab, Tiles[purpleSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
    }

    public void GeneratePath()
    {
        path = AStar.GetPath(GreenSpawn, purpleSpawn);
    }
}
