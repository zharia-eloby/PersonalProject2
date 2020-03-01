using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { IDLE, PATROL, PURSUIT, ATTACK, DEAD };
public class ZorlikScript : MonoBehaviour
{
    int healthStart = 100;
    int enemyHP;
    GameObject healthBar;
    float healthBarXScaleStart;
    float pursuitDistance = 10.0f;
    float attackDistance = 2.5f;//1.5f;
    private Animator anim;
    string currentState = "Idle";
    public Vector3[] patrolSpots;
    int currentPatrolSpot;
    float patrolSpeed = 1.2f;
    float pursuitSpeed = 2.0f;
    GameObject player;
    bool inCoroutine = true; //false;

    protected EnemyState state = EnemyState.IDLE;

    void Start()
    {
        enemyHP = healthStart;
        healthBar = transform.Find("Health Bar").gameObject;
        healthBarXScaleStart = healthBar.transform.localScale.x;
        anim = GetComponent<Animator>();
        anim.SetBool(currentState, true);
        patrolSpots = new Vector3[5];
        currentPatrolSpot = Random.Range(0, patrolSpots.Length);
        setPatrolSpots();
        player = GameObject.FindWithTag("Player");
        anim.SetBool("Crouch", true);
    }


    void Update()
    {
        Debug.Log(state);
        if (enemyHP > 0)
        {
            healthBar.transform.LookAt(player.transform);
        }
        switch(state)
        {
            case EnemyState.IDLE:
                if (!inCoroutine)
                {
                    StartCoroutine(IdleToPatrol());
                }
                break;
            case EnemyState.PATROL:
                if (!inCoroutine && Vector3.Distance(player.transform.position, transform.position) < pursuitDistance)
                {
                    StartCoroutine(PatrolToPursuit());
                }
                else if (!inCoroutine && Vector3.Distance(transform.position, patrolSpots[currentPatrolSpot]) < 0.1f)
                {
                    int pastSpot = currentPatrolSpot;
                    while (pastSpot == currentPatrolSpot)
                    {
                        currentPatrolSpot = Random.Range(0, patrolSpots.Length);
                    }
                    state = EnemyState.IDLE;
                }
                else if (!inCoroutine)
                {
                    transform.LookAt(patrolSpots[currentPatrolSpot]);
                    transform.position = Vector3.MoveTowards(transform.position, patrolSpots[currentPatrolSpot], Time.deltaTime * patrolSpeed);
                }
                break;
            case EnemyState.PURSUIT:
                if (!inCoroutine && Vector3.Distance(player.transform.position, transform.position) > pursuitDistance)
                {
                    StartCoroutine(PursuitToPatrol());
                }
                else if (!inCoroutine && Vector3.Distance(player.transform.position, transform.position) < attackDistance)
                {
                    StartCoroutine(PursuitToAttack());
                }
                else if (!inCoroutine)
                {
                    transform.LookAt(player.transform.position);
                    Vector3 targetLocation = player.transform.position;
                    targetLocation.y = transform.position.y;
                    transform.position = Vector3.MoveTowards(transform.position, targetLocation, Time.deltaTime * pursuitSpeed);
                }
                break;
            case EnemyState.ATTACK:
                if (!inCoroutine && Vector3.Distance(player.transform.position, transform.position) > attackDistance)
                {
                    StartCoroutine(AttackToPursuit());
                } else if (!inCoroutine) {
                    Vector3 target = player.transform.position;
                    target.y = transform.position.y;
                    transform.LookAt(target);
                }
                break;
            case EnemyState.DEAD:
                if (!inCoroutine)
                {
                    StartCoroutine(Die());
                }
                break;
        }
    }

    void setPatrolSpots()
    {
        Transform spotsParent = transform.parent.Find("Patrol Spots");
        for (int i = 0; i < patrolSpots.Length; i++)
        {
            patrolSpots[i] = spotsParent.GetChild(i).transform.position;
        }
    }

    void UpdateHealthBar()
    {
        if (enemyHP <= 0)
        {
            state = EnemyState.DEAD;
            Destroy(healthBar);
        }
        else
        {
            float newX = enemyHP * healthBarXScaleStart / healthStart;
            float yScale = healthBar.transform.localScale.y;
            float zScale = healthBar.transform.localScale.z;
            healthBar.transform.localScale = new Vector3(newX, yScale, zScale);
        }
    }

    void ChangeStateTo(string newState)
    {
        anim.SetBool(currentState, false);
        currentState = newState;
        anim.SetBool(currentState, true);
        anim.Play(currentState, 0);
    }

    private IEnumerator IdleToPatrol()
    {
        inCoroutine = true;
        ChangeStateTo("Idle");
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        //yield return new WaitForSeconds(1.667f);
        ChangeStateTo("Idle Lookout");
        yield return new WaitForSeconds(Random.Range(4.0f, 7.0f));
        //yield return new WaitForSeconds(5.167f);
        ChangeStateTo("Walk");
        state = EnemyState.PATROL;
        inCoroutine = false;
    }

    private IEnumerator PatrolToIdle()
    {
        inCoroutine = true;
        ChangeStateTo("Idle");
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));//(1.667f);
        state = EnemyState.IDLE;
        inCoroutine = false;
    }

    private IEnumerator PatrolToPursuit()
    {
        inCoroutine = true;
        ChangeStateTo("Idle Lookout Intense");
        yield return new WaitForSeconds(3.167f);
        ChangeStateTo("Run");
        state = EnemyState.PURSUIT;
        inCoroutine = false;
    }

    private IEnumerator PursuitToPatrol()
    {
        inCoroutine = true;
        ChangeStateTo("Idle Battle");
        yield return new WaitForSeconds(Random.Range(4.0f, 5.0f));//(4.667f);
        ChangeStateTo("Walk");
        state = EnemyState.PATROL;
        inCoroutine = false;
    }

    private IEnumerator PursuitToAttack()
    {
        inCoroutine = true;
        ChangeStateTo("Attack");
        yield return new WaitForSeconds(0.0f);
        state = EnemyState.ATTACK;
        inCoroutine = false;
    }

    private IEnumerator AttackToPursuit()
    {
        inCoroutine = true;
        ChangeStateTo("Run");
        yield return new WaitForSeconds(0.0f);
        state = EnemyState.PURSUIT;
        inCoroutine = false;
    }

    private IEnumerator Die()
    {
        inCoroutine = true;
        ChangeStateTo("Dead");
        yield return new WaitForSeconds(1.033f);
        Destroy(this.gameObject);
        state = EnemyState.DEAD;
        inCoroutine = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        {
            enemyHP -= 15;
            UpdateHealthBar();
        }
    }
}
