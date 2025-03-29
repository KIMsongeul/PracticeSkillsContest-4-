using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : MonoBehaviour
{
    public GameObject patrolMonster;
    public GameObject AIMonster;

    private void Start()
    {
        patrolMonster = Resources.Load<GameObject>("Prefabs/PatrolMonster");
        AIMonster = Resources.Load<GameObject>("Prefabs/AIMonster");
    }
    public void CreatePatrolMonster(Vector3 pos)
    {
        Instantiate(patrolMonster, pos, patrolMonster.transform.rotation);
    }
    public void CreateAIMonster(Vector3 pos)
    {
        Instantiate(AIMonster, pos, AIMonster.transform.rotation);
    }
}
