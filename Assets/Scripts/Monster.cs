using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Transform player;

    public bool isPatrol = true;
    float patrolSpeed = 2.0f;
    float speed = 2.0f;
    float distance = 10.0f;
    int attackDamage = 1;
    bool isWalk = false;

    public float attackTimer = 1.0f;
    public float attackTiming = 0;
    public bool isAttack = false;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (GameData.Instance.isGame)
        {
            if (isPatrol)
            {
                MovePatrolMonsters();
            }
            else
            {
                MoveMonster();
            }
            if (isAttack)
            {
                if (attackTiming > attackTimer)
                {
                    isAttack = false;
                    attackTiming = 0;
                }
                else
                {
                    attackTiming += Time.deltaTime;
                }
            }
        }
    }
    void MovePatrolMonsters()
    {
        Vector3 direction = this.transform.forward;
        Vector3 newPosition = this.transform.position + direction * patrolSpeed * Time.deltaTime;
        if (IsGround(newPosition))
        {
            this.transform.position = newPosition;
        }
        else
        {
            this.transform.Rotate(0, 180, 0);
        }
        SetPlayerDamage();
    }
    void MoveMonster()
    {
        if (player != null && !player.GetComponent<PlayerControl>().isRecog)
        {
            Vector3 targetPos = player.transform.position;
            targetPos.y = this.transform.position.y;
            if (Vector3.Distance(this.transform.position, targetPos) < distance && Vector3.Distance(this.transform.position, targetPos) > 1.5f)
            {
                Vector3 pos = targetPos - this.transform.position;
                this.transform.rotation = Quaternion.LookRotation(pos);
                if (!isWalk)
                {
                    isWalk = true;
                }
                Vector3 direction = this.transform.forward;
                Vector3 newPosition = this.transform.position + direction * speed * Time.deltaTime;
                if (IsGround(newPosition))
                {
                    this.transform.position = newPosition;
                }
            }
            else
            {
                if (isWalk)
                {
                    isWalk=false;
                    if (!isAttack)
                    {
                        isAttack = true;
                        player.GetComponent<PlayerControl>().SetDamage(attackDamage);
                    }
                }
            }
        }
    }
    bool IsGround(Vector3 position)
    {
        if (position.x < 0 || position.x > GameData.Instance.stageSize || position.z < 0 || position.z > GameData.Instance.stageSize)
        {
            return false;
        }
        RaycastHit[] hits;
        Vector3 rayOrigin = this.transform.position + this.transform.forward;
        Vector3 forward = this.transform.forward;
        hits = Physics.BoxCastAll(rayOrigin, new Vector3(0.5f, 2.0f, 0.5f), forward, Quaternion.identity, 1.0f);
        if (hits.Length > 0)
        {
            foreach(RaycastHit hit in hits)
            {
                if (hit.transform.CompareTag("Obstacle") || hit.transform.CompareTag("Chest"))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    void SetPlayerDamage()
    {
        Vector3 findPos = player.position;
        findPos.y = this.transform.position.y;
        if (Vector3.Distance(this.transform.position,findPos) < 1.0f)
        {
            if (!isAttack)
            {
                isAttack = true;
                player.GetComponent<PlayerControl>().SetDamage(attackDamage);
            }
        }
    }
}
