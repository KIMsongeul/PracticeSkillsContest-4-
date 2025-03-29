using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagData
{
    /// <summary>
    /// 아이템 이름
    /// </summary>
    public string name;
    /// <summary>
    /// 아이템 개수
    /// </summary>
    public int num;
    /// <summary>
    /// 아이템 개당 무게
    /// </summary>
    public int weight;
    /// <summary>
    /// 가방에 들어있는 순번
    /// </summary>
    public int bagNum;
}

public enum AttackType { Melee = 0, Ranged = 1 };
public class GameData : MonoBehaviour
{
    #region 게임 내부에서 기록
    /// <summary>
    /// 현재 스테이지 번호
    /// </summary>
    public int stageNum = 0;
    /// <summary>
    /// 0 : 4칸(기본), 1: 6칸(대형), 8(초대형)
    /// </summary>
    public int bagUpgradeNum = 0;
    /// <summary>
    /// 게임중인지 확인, 게임 중이라면 정보 기억
    /// </summary>
    public bool isGame = true;
    /// <summary>
    /// 현재 스테이지 사이즈 기억
    /// </summary>
    public int stageSize = 0;
    /// <summary>
    /// 가방 데이터 기록
    /// </summary>
    //public List<Item> bagData = new List<Item>();
    /// <summary>
    /// 보물 개수
    /// </summary>
    public List<bool> chestData = new List<bool>();
    /// <summary>
    /// 게임진행시간
    /// </summary>
    public float gameTime = 0f;
    /// <summary>
    /// 현재 가진 돈
    /// </summary>
    public int cost = 0;
    /// <summary>
    /// 아이템 구매 가능여부
    /// </summary>
    public bool[] isBuyItem = { true, true, true, true, true };
    /// <summary>
    /// 마우스 상태(true:일반, false:잠금)
    /// </summary>
    public bool isMouse = false;
    /// <summary>
    /// 산소통 넘버
    /// </summary>
    public int bagO2 = 0;
    
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #region 세팅 후 변하지 않는 값들
    /// <summary>
    /// 싱글톤
    /// </summary>
    public static GameData Instance;
    /// <summary>
    /// 스테이지별 처음 시작 위치
    /// </summary>
    public static List<Vector3> StagePos = new List<Vector3>
    {
        new Vector3 (1, 0, 1),
        new Vector3 (1, 0, 1),
        new Vector3 (1, 0, 1),
        new Vector3 (1, 0, 1),
        new Vector3 (1, 0, 1)
    };

    /// <summary>
    /// 텍스쳐 사이즈 width
    /// </summary>
    public static int textureWidth = 256;
    /// <summary>
    /// 텍스쳐 사이즈 height
    /// </summary>
    public static int textureHeight = 256;

    /// <summary>
    /// 스테이지별 산소 감소량
    /// </summary>
    public static int[] stageDeO2 = { 1, 1, 2, 2, 4 };
    /// <summary>
    /// 샌드색깔 베이스
    /// </summary>
    public static Color baseSandColor = new Color(0.85f, 0.7f, 0.4f);
    /// <summary>
    /// 샌드색깔 믹스
    /// </summary>
    public static Color mixSandColor = new Color(0.98f, 0.85f, 0.66f);

    /// <summary>
    /// 잔디색깔 베이스
    /// </summary>
    public static Color baseGrassColor = new Color(0.30f, 0.55f, 0.16f);
    /// <summary>
    /// 잔디색깔 믹스
    /// </summary>
    public static Color mixGrassColor = new Color(0.16f, 0.36f, 0.03f);
    /// <summary>
    /// 부술수 없는 블럭(바닥만 존재,모래)
    /// </summary>
    public static Color ColorNoneSandBlock = Color.white;
    /// <summary>
    /// 부술수 없는 블럭(바닥만 존재, 잔디)
    /// </summary>
    public static Color ColorNoneGrassBlock = new Color(34f/255f,177f/255f,76/255f);
    /// <summary>
    /// 장애물 블럭
    /// </summary>
    public static Color ColorBlock = Color.red;
    /// <summary>
    /// 북쪽으로 패트롤 이동하는 적
    /// </summary>
    public static Color ColorPatrolEnemyTop = Color.blue;
    /// <summary>
    /// 특정 거리에서 따라오는 적 
    /// </summary>
    public static Color ColorEnemy = Color.green;
    /// <summary>
    /// 보물
    /// </summary>
    public static Color ColorTreasure = Color.black;
    /// <summary>
    /// 함정
    /// </summary>
    public static Color ColorTrap = Color.magenta;
    /// <summary>
    /// 피라미드 생성(공간 사이즈* 사이즈 필요)
    /// </summary>
    public static Color ColorPyramid = Color.cyan;
    /// <summary>
    /// 랜덤 아이템 생성
    /// </summary>
    public static Color colorRandom = new Color(185f / 255f, 122f / 255f, 87f / 255f);
    /// <summary>
    /// 로비 돌아가기 
    /// </summary>
    public static Color colorRobby = new Color(255f / 255f , 174f / 255f, 201f / 255f);
    /// <summary>
    /// 시작지점
    /// </summary>
    public static Color colorStart = new Color(127f / 255f , 127f / 255f, 127f / 255f);
    /// <summary>
    /// 시작지점
    /// </summary>
    public static Color colorEND = new Color(0f / 255f, 162f / 255f, 232f / 255f);
    /// <summary>
    /// 구매 아이템 가격
    /// </summary>
    public static int[] itemCost = { 10, 100, 1000, 100, 1000 };
    /// <summary>
    /// 구매 아이템 내용
    /// </summary>
    public static string[] itemText =
    {
        "저압용 산소통\n$10",
        "중압용 산소통\n$100",
        "고압용 산소통\n$1000",
        "대형 가방\n$100",
        "초대형 가방\n$1000"
    };
    public static string buyItemText = "구매 완료";
    public static string buyFileText = "구매 불가능";

    /// <summary>
    /// 아이템 이름들
    /// </summary>
    public static string[] sName = { "FIND", "HP", "O2", "NOTRECOG", "DOUBLEFAST", "FAST",  };
    /// <summary>
    /// 아이템 무게들
    /// </summary>
    public static int[] iWeight = { 10, 10, 10, 10, 10, 10};
    public static int[] iBagSize = { 4, 6, 8 };
    /// <summary>
    /// 가방 무게
    /// </summary>
    public static int[] iBagWeight = { 100, 250, 400 };
    /// <summary>
    /// 체력 값
    /// </summary>
    public static int MaxHp = 10;
    /// <summary>
    /// 산소 값
    /// </summary>
    public static int[] MaxO2 = {60, 70, 80, 100 };
    /// <summary>
    /// 스테이지별 보물상자 가격
    /// </summary>
    public static int[] chestCost = { 100, 10, 100, 500, 1000 };
    #endregion

}
