using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float originalSpeed;

    public float speedTimer = 5.0f;
    public float speedTiming = 0;
    public bool isSpeedUp = false;

    public float dbSpeedTimer = 5.0f;
    public float dbSpeedTiming = 0;
    public bool isDbSpeedUp = false;

    public float recogTimer = 5.0f;
    public float recogTiming = 0;
    public bool isRecog = false;

    public GameObject ChestArrow;
    public GameObject tempChest;

    public float rotSpeed = 2.0f;

    public Transform trCamera;

    private CharacterController characterController;
    private float rotX = 0f;
    private float rotY = 0f;

    private GameObject prefabKnife;
    private GameObject knife;
    private Vector3 knifeOriginalPos;
    private Quaternion knifeOriginalRot;

    public int O2 = 60;
    public float O2Timer = 10;
    public float O2Timing = 0;
    public bool isO2 = true;

    public int hp = 10;

    public GameManger gameManger;
    //public inventoryControl inventoryControl;
    private void Update()
    {
        if (!GameData.Instance.isMouse)
        {
            Move();
            LookAround();
            Attack();
        }
    }
    private void Start()
    {
        SetPlayer();
    }

    public void SetPlayer()
    {
        originalSpeed = moveSpeed;
        gameManger = GameObject.Find("GameManager").GetComponent<GameManger>();
        this.transform.position = GameData.StagePos[GameData.Instance.stageNum];
        trCamera = GameObject.Find("Main Camera").transform;
        ChestArrow = Resources.Load<GameObject>("Prefabs/ChestArrow");
        characterController = this.AddComponent<CharacterController>();
        //inventoryControl = this.AddComponent<InventoryControl>();
        CreateKnife();
    }
    void CreateKnife()
    {
        prefabKnife = Resources.Load<GameObject>("Prefabs/Knife");
        knife = Instantiate(prefabKnife);

        knife.transform.SetParent(trCamera);
        knife.transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f);
        knifeOriginalPos = knife.transform.localPosition;
        knifeOriginalRot = knife.transform.localRotation;
    }
    IEnumerator AnimateKnifeAttack()
    {
        Vector3 attackPos = knifeOriginalPos + new Vector3(-0.2f, 0, 0.2f);
        Quaternion attackRot = Quaternion.Euler(knifeOriginalRot.eulerAngles + new Vector3(0, -20, 0));

        float attackTime = 0.1f;
        float elapsedTime = 0f;
        while (elapsedTime < attackTime)
        {
            knife.transform.localPosition = Vector3.Lerp(knifeOriginalPos, attackPos, elapsedTime / attackTime);
            knife.transform.localRotation = Quaternion.Lerp(knifeOriginalRot, attackRot, elapsedTime / attackTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < attackTime)
        {
            knife.transform.localPosition = Vector3.Lerp(attackPos, knifeOriginalPos, elapsedTime / attackTime);
            knife.transform.localRotation = Quaternion.Lerp(attackRot, knifeOriginalRot, elapsedTime / attackTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.forward * v + transform.right * h;
        characterController.Move(move * moveSpeed * Time.deltaTime);
        trCamera.position = transform.position + new Vector3(0, 0.5f, 0);
    }
    void LookAround()
    {
        float x = Input.GetAxis("Mouse X") * rotSpeed;
        float y = Input.GetAxis("Mouse Y") * rotSpeed;
        rotX -= y;
        rotY += x;

        rotX = Mathf.Clamp(rotX, -90f, 90f);
        transform.rotation = Quaternion.Euler(0, rotY, 0);
        trCamera.localRotation = Quaternion.Euler(rotX, rotY, 0);

    }
    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectObjectInFront();
            StartCoroutine(AnimateKnifeAttack());
        }
    }
    void DetectObjectInFront()
    {
        RaycastHit hit;
        Vector3 forward = transform.forward;
        Vector3 rayOrigin = transform.position;

        if (Physics.BoxCast(rayOrigin, new Vector3(0.5f,1.0f,0.5f), forward, out hit, Quaternion.identity, 1.0f))
        {
            Debug.Log("감지 오브젝트 : " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.CompareTag("Chest"))
            {
                int num = gameManger.mapControl.chests.FindIndex(a => a == hit.collider.gameObject);
                if (num > -1)
                {
                    GameData.Instance.chestData[num] = false;
                    GameData.Instance.cost += GameData.chestCost[GameData.Instance.stageNum];
                    hit.collider.gameObject.SetActive(false);
                }
                //gameManger.SetChest(GameData.Instance.chestData.FindAll(a => a).Count);
            }
        }
    }
}
