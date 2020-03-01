using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurtleState { IDLE, PATROL, PURSUIT, ATTACK, TAKE_HIT, DEAD };

public class SpikedTurtleScript : MonoBehaviour
{
    int startingHealth;
    int turtleHealth;
    int arrowDamage;
    int swordDamage;
    GameObject player;
    GameObject healthBar;
    float healthBarStart;
    Vector3 startPosition;
    static float patrolSpeed;
    static float pursuitSpeed;
    public Vector3[] patrolSpots = new Vector3[2];
    int currentPatrolSpot;
    float attackDistance;
    float pursuitDistance;
    Animator anim;
    string previousState;
    string currentState;
    bool takingHit;
    bool dying;
    TurtleState previousTurtleState;

    protected TurtleState state = TurtleState.IDLE;

    void Start()
    {
        startingHealth = 50;
        turtleHealth = startingHealth;
        arrowDamage = 10;
        swordDamage = 15;
        player = GameObject.FindWithTag("Player");
        healthBar = transform.Find("Health Bar").gameObject;
        healthBarStart = healthBar.transform.localScale.x;
        patrolSpeed = 0.75f;
        pursuitSpeed = 1.5f;
        patrolSpots[0] = GameObject.Find("Patrol1").transform.position;
        patrolSpots[1] = GameObject.Find("Patrol2").transform.position;
        startPosition = (patrolSpots[0] + patrolSpots[1]) / 2;
        currentPatrolSpot = 0;
        attackDistance = 1.0f;
        pursuitDistance = 7.0f;
        anim = GetComponent<Animator>();
        currentState = "IdleNormal";
        takingHit = false;
        dying = false;
        anim.SetBool(currentState, false);
        currentState = "WalkFWD";
        anim.SetBool(currentState, true);
    }

    void Update()
    {
        if (turtleHealth > 0)
        {
            healthBar.transform.LookAt(player.transform);
        }
        switch (state)
        {
            case TurtleState.IDLE:
                state = TurtleState.PATROL;
                break;
            case TurtleState.PATROL:
                if (Vector3.Distance(player.transform.position, transform.position) <= pursuitDistance && Vector3.Distance(player.transform.position, startPosition) < attackDistance + pursuitDistance) //PATROL to PURSUIT
                {
                    StartCoroutine(PatrolToPursuit());
                }
                else
                {
                    if (Vector3.Distance(patrolSpots[currentPatrolSpot], transform.position) < 0.5f)
                    {
                        if (currentPatrolSpot == patrolSpots.Length - 1)
                        {
                            currentPatrolSpot = 0;
                        }
                        else
                        {
                            currentPatrolSpot++;
                        }
                    }
                    else
                    {
                        transform.LookAt(patrolSpots[currentPatrolSpot]);
                        transform.position = Vector3.MoveTowards(transform.position, patrolSpots[currentPatrolSpot], Time.deltaTime * patrolSpeed);
                    }
                }
                break;
            case TurtleState.PURSUIT:
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                if (Vector3.Distance(player.transform.position, transform.position) > pursuitDistance || Vector3.Distance(transform.position, startPosition) > attackDistance + pursuitDistance) //PURSUIT to PATROL
                {
                    StartCoroutine(PursuitToPatrol());
                }
                else if (Vector3.Distance(player.transform.position, transform.position) <= attackDistance) //PURSUIT to ATTACK
                {
                    StartCoroutine(PursuitToAttack());
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), Time.deltaTime * pursuitSpeed);
                }
                break;
            case TurtleState.ATTACK:
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                if (Vector3.Distance(player.transform.position, transform.position) > attackDistance) //ATTACK to PURSUIT
                {
                    StartCoroutine(AttackToPursuit());
                }
                break;
            case TurtleState.TAKE_HIT:
                if (!takingHit)
                {
                    StartCoroutine(TakeHit());
                }
                break;
            case TurtleState.DEAD:
                if (!dying)
                {
                    StartCoroutine(Die());
                }
                break;
            default:
                break;
        }
    }

    void ChangeStateTo(string newState)
    {
        anim.SetBool(currentState, false);
        currentState = newState;
        anim.SetBool(currentState, true);
    }

    private IEnumerator PatrolToPursuit()
    {
        anim.SetBool(currentState, false);
        int whichSense = Random.Range(0, 2);
        if (whichSense == 0)
        {
            currentState = "SenseSomethingST";
        }
        else
        {
            currentState = "SenseSomethingRPT";
        }
        anim.SetBool(currentState, true);
        yield return new WaitForSeconds(3.5f);
        ChangeStateTo("RunFWD");
        state = TurtleState.PURSUIT;
    }

    private IEnumerator PursuitToPatrol()
    {
        ChangeStateTo("Taunt");
        yield return new WaitForSeconds(1.0f);
        ChangeStateTo("WalkFWD");
        state = TurtleState.PATROL;
    }

    private IEnumerator PursuitToAttack()
    {
        anim.SetBool(currentState, false);
        int whichAttack = Random.Range(0, 2);
        if (whichAttack == 0)
        {
            currentState = "Attack01";
        }
        else
        {
            currentState = "Attack02";
        }
        anim.SetBool(currentState, true);
        yield return new WaitForSeconds(0.0f);
        state = TurtleState.ATTACK;
        PlayerScript.currentEnemy = this.gameObject;
    }

    private IEnumerator AttackToPursuit()
    {
        PlayerScript.currentEnemy = null;
        yield return new WaitForSeconds(1.0f);
        ChangeStateTo("RunFWD");
        state = TurtleState.PURSUIT;
    }

    private IEnumerator TakeHit()
    {
        takingHit = true;
        string previousAnimState = currentState;
        ChangeStateTo("GetHit");
        yield return new WaitForSeconds(0.0f);
        if (turtleHealth <= startingHealth / 2)
        {
            ChangeStateTo("Defend");
            yield return new WaitForSeconds(2.5f);
        }
        ChangeStateTo(previousAnimState);
        state = previousTurtleState;
        takingHit = false;
    }

    private IEnumerator Die()
    {
        dying = true;
        ChangeStateTo("GetHit");
        yield return new WaitForSeconds(0.0f);
        ChangeStateTo("Dizzy");
        yield return new WaitForSeconds(1.5f);
        ChangeStateTo("Die");
        yield return new WaitForSeconds(2.0f);
        while (!(transform.localScale.x < 0.05f)) //Shrink Turtle
        {
            transform.localScale = transform.localScale - new Vector3(0.05f, 0.05f, 0.05f);
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.01f);
        Destroy(this.gameObject);
        dying = false;
    }

    void UpdateHealthBar()
    {
        //Debug.Log(turtleHealth);
        if (turtleHealth <= 0)
        {
            Destroy(healthBar);
        }
        else
        {
            float newXScale = turtleHealth * healthBarStart / startingHealth;
            healthBar.transform.localScale = new Vector3(newXScale, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Turtle doesn't receive damage if it's in the defend state
        //Turtle is not affected by weapons if it is in the dead state
        if (other.CompareTag("Arrow") && state != TurtleState.DEAD && !currentState.Equals("Defend"))
        {
            turtleHealth -= arrowDamage;
            if (!takingHit && turtleHealth > 0)
            {
                previousTurtleState = state;
                StopAllCoroutines();
                state = TurtleState.TAKE_HIT;
            }
            else if (turtleHealth <= 0)
            {
                StopAllCoroutines();
                state = TurtleState.DEAD;
            }
            UpdateHealthBar();
        }
        else if (other.CompareTag("Sword") && state != TurtleState.DEAD && !currentState.Equals("Defend"))
        {
            turtleHealth -= swordDamage;
            if (!takingHit && turtleHealth > 0)
            {
                previousTurtleState = state;
                StopAllCoroutines();
                state = TurtleState.TAKE_HIT;
            }
            else if (turtleHealth <= 0)
            {
                StopAllCoroutines();
                state = TurtleState.DEAD;
            }
            UpdateHealthBar();
        }
    }
}
