using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public float noiseScale = 5f;
    public float heightMultiplier = 2f;
    public GameObject[] obstacles;
    public float obstacleSpawnChance = 0.1f;

    public Texture2D sandTexture;
    public Texture2D grassTexture;

    public Color colorResponse = new Color(64, 128, 128);

    public Transform terrain;
    public Texture2D[] mapInfo;
    public float tileSize = 1f;
    private int mapWidth;
    private int mapHeight;

    public GameObject prefabChest;
    private Transform player;
    private PlayerControl playerControl;
    List<GameObject> traps = new List<GameObject>();
    public GameManger gameManager;
    public List<GameObject> chests = new List<GameObject>();
    public GameObject[] prefabItems;
    public GameObject prefabDoor;
    public Vector3 startPos;

    private void Start()
    {
        CreateMap();
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    public void CreateMap()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManger>();
        player = GameObject.Find("Player").transform;
        prefabChest = Resources.Load<GameObject>("Prefabs/Chest_Closed");
        prefabItems = Resources.LoadAll<GameObject>("Prefabs/Items");
        prefabDoor = Resources.Load<GameObject>("Prefabs/Door");
        sandTexture = GenerateTexture(GameData.baseSandColor, GameData.mixSandColor);
        grassTexture = GenerateTexture(GameData.baseGrassColor, GameData.mixGrassColor);

        mapInfo = Resources.LoadAll<Texture2D>("MapData");
        GenerateDesertTerrain();
        if (!GameData.Instance.isGame)
        {
            GameData.Instance.stageNum = 0;
            GameData.Instance.bagUpgradeNum = 0;
            GameData.Instance.bagData.Clear();
            GameData.Instance.chestData.Clear();
            foreach(GameObject obj in chests)
            {
                GameData.Instance.chestData.Add(true);  
            }
        }
        else
        {
            for(int i = 0; i < GameData.Instance.chestData.Count; i++)
            {
                chests[i].SetActive(GameData.Instance.chestData[i]);
            }
        }
        gameManager.SetChest(GameData.Instance.chestData.FindAll(a => a).Count);
        GameData.Instance.isGame = true;      

    }
    

    void LimitPlayerMove()
    {
        if (player == null) return;

        float minX = 0;
        float maxX = mapWidth;
        float minZ = 0;
        float maxZ = mapHeight;

        Vector3 clampedPosition = player.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minZ, maxZ);
        player.position = clampedPosition;
            
    }
    Texture2D GenerateTexture(Color BaseColor, Color mixColor)
    {
        Texture2D texture = new Texture2D(GameData.textureWidth, GameData.textureHeight);
        for (int x = 0; x < GameData.textureWidth; x++)
        {
            for (int y = 0; y < GameData.textureHeight; y++)
            {
                float noise = Mathf.PerlinNoise(x / noiseScale, y / noiseScale);
                Color pixelColor = Color.Lerp(BaseColor, mixColor, noise);
                texture.SetPixel(x, y, pixelColor);
            }
        }
        texture.Apply();
        return texture;
    }
    private void Update()
    {
        LimitPlayerMove();
    }

    void GenerateDesertTerrain()
    {
        GameObject map = new GameObject("Map");
        mapWidth = mapInfo[GameData.Instance.stageNum].width;
        mapHeight = mapInfo[GameData.Instance.stageNum].height;
        GameData.Instance.stageSize = mapWidth;
        Color[] pixels = mapInfo[GameData.Instance.stageNum].GetPixels();

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                Color pixelColor = pixels[i * mapHeight + j];
                if (pixelColor == GameData.ColorNoneSandBlock)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = map.transform;
                }
                else if (pixelColor == GameData.ColorPyramid)
                {
                    GeneratePyramid(new Vector3(j * tileSize, -1f, i * tileSize));
                }
                else if (pixelColor == GameData.ColorNoneGrassBlock)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = grassTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = map.transform;
                }
                else if (pixelColor == GameData.ColorTreasure)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = map.transform;

                    GameObject chest = Instantiate(prefabChest, new Vector3
                        (j * tileSize, -1f, i * tileSize), prefabChest.transform.rotation);
                    chest.transform.position = new Vector3(j * tileSize, -0.5f, i * tileSize);
                    chest.tag = "Chest";
                    chests.Add(chest);
                }
                else if (pixelColor == GameData.ColorBlock)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = map.transform;

                    GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    block.transform.position = new Vector3(j * tileSize, 0f, i * tileSize);
                    block.transform.parent = map.transform;
                    block.tag = "Obstacle";

                }
                else if (pixelColor == GameData.ColorTrap)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = map.transform;

                    GameObject trap = GenerateTrap();
                    trap.transform.position = new Vector3(j* tileSize,-1f,i * tileSize);
                    trap.transform.parent = map.transform;
                }
                else if (pixelColor == GameData.ColorPatrolEnemyTop)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = map.transform;

                    gameManager.monsterControl.CreatePatrolMonster(new Vector3(j * tileSize, 0f, i * tileSize));
                }
                else if (pixelColor == GameData.colorRandom)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = map.transform;

                    int randNum = Random.Range(0, prefabItems.Length);
                    GameObject item = Instantiate(prefabItems[randNum], new Vector3
                        (j * tileSize, 0f, i * tileSize), prefabItems[randNum].transform.rotation);
                    item.transform.position = new Vector3
                        (j * tileSize, 0f, i * tileSize);
                    item.GetComponent<Item>().SetItem(randNum);
                    item.tag = "Item";
                }
                else if (pixelColor == GameData.colorRobby)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = map.transform;

                    GameObject robby = Instantiate(prefabDoor, new Vector3(j* tileSize, -1f, i * tileSize), prefabDoor.transform.rotation);
                    robby.transform.position = new Vector3(j* tileSize,-0.5f,i * tileSize);
                    robby.tag = "Start";

                }
                else if (pixelColor == GameData.colorEND)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = map.transform;

                    GameObject robby = Instantiate(prefabDoor, new Vector3(j * tileSize, -1f, i * tileSize), prefabDoor.transform.rotation);
                    robby.transform.position = new Vector3(j * tileSize, -0.5f, i * tileSize);
                    robby.tag = "End";
                }

            }
        }
    }
    GameObject GenerateTrap()
    {
        GameObject trap = new GameObject("trap");
        for (int i = 0; i < 5; i++)
        {
            GameObject thorn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            thorn.transform.position = new Vector3(Random.Range(-0.4f, 0.4f),0f, Random.Range(-0.4f, 0.4f));
            thorn.transform.localScale = new Vector3(0.1f, 1.1f, 0.1f);
            thorn.GetComponent<Renderer>().material.color = Color.white;
            thorn.transform.parent = trap.transform;
        }
        traps.Add(trap);
        return trap;
    }
    void GeneratePyramid(Vector3 pos)
    {
        GameObject pyramid = new GameObject("Pyramid");
        int baseSize = 8;
        float blockSize = 1f;

        for (int y = 0; y < baseSize; y++)
        {
            for (int x = 0; x < baseSize - y; x++)
            {
                for (int z = 0; z < baseSize - y; z++)
                {
                    GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.transform.position = new Vector3(x - (baseSize - y) / 2f, y * blockSize, z - (baseSize - y) / 2f);
                    block.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
                    block.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    block.transform.parent = pyramid.transform;
                }
            }
        }
        pyramid.transform.position = pos;
    }
}
