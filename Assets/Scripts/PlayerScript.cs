using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public Text playerText;
    public Text arrowText;
    public Text arrowPickUpText;
    public static GameObject currentArrowInPickUpRange;
    public Image playerHealthBar;
    int startHealth;
    float startHealthBar;
    int playerHealth;
    public static GameObject currentEnemy;
    GameObject[] weapons;
    GameObject shield;
    GameObject bow;
    GameObject sword;
    GameObject arrow;
    int currentWeapon;
    Vector3 shieldMoveUpPos;
    Vector3 shieldStartPos;
    public static int arrowCapacity;
    public static int currentArrowCount;
    bool beingAttacked;
    bool inTurtleCollision;

    void Start()
    {
        currentArrowInPickUpRange = null;
        currentEnemy = null;
        startHealth = 1000;
        playerHealth = startHealth;
        startHealthBar = playerHealthBar.gameObject.transform.localScale.x;
        shield = transform.GetChild(0).GetChild(0).gameObject;
        bow = transform.GetChild(0).GetChild(1).gameObject;
        sword = transform.GetChild(0).GetChild(2).gameObject;
        arrow = bow.transform.GetChild(2).gameObject;
        weapons = new GameObject[] { bow, sword };
        currentWeapon = 0;
        shieldStartPos = shield.transform.localPosition;
        shieldMoveUpPos = new Vector3(0.0f, -0.138f, 0.143f);
        shield.SetActive(false);
        bow.SetActive(true);
        sword.SetActive(false);
        arrowCapacity = 25;
        currentArrowCount = 10;
        beingAttacked = false;
        inTurtleCollision = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            ChangeWeapon();
        }
        else if (Input.GetKeyDown("x") && !shield.activeSelf)
        {
            ShieldUp();
        } else if (Input.GetKeyDown("x") && shield.activeSelf)
        {
            ShieldDown();
        }
        if (!beingAttacked && currentEnemy != null && !inTurtleCollision)
        {
            beingAttacked = true;
            StartCoroutine(GetAttacked());
        }
        UpdateScreen();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Quiver"))
        {
            if (currentArrowCount == 0)
            {
                GameObject nextArrow = Instantiate(arrow, ArrowScript.bow.transform, true);
                nextArrow.GetComponent<Rigidbody>().velocity = Vector3.zero;
                nextArrow.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                nextArrow.GetComponent<CapsuleCollider>().isTrigger = true;
                nextArrow.GetComponent<Rigidbody>().useGravity = false;
                nextArrow.transform.localEulerAngles = new Vector3(0, 0, 90);
                nextArrow.transform.localPosition = ArrowScript.arrowStartPos;
            }
            if (arrowCapacity - currentArrowCount >= 10)
            {
                currentArrowCount += 10;
            } else
            {
                currentArrowCount = arrowCapacity;
            }
            Destroy(other.gameObject);
        } else if (other.CompareTag("SpikedTurtle") && !inTurtleCollision) {
            StartCoroutine(TurtleCollision());
        }
    }

    public void ChangeWeapon()
    {
        weapons[currentWeapon].SetActive(false);
        if (currentWeapon == weapons.Length - 1)
        {
            currentWeapon = 0;
        } else
        {
            currentWeapon++;
        }
        weapons[currentWeapon].SetActive(true);
    }

    public void ShieldUp()
    {
        weapons[currentWeapon].SetActive(false);
        shield.SetActive(true);
        while (shield.transform.localPosition != shieldMoveUpPos)
        {
            shield.transform.localPosition = Vector3.MoveTowards(shield.transform.localPosition, shieldMoveUpPos, Time.deltaTime * 0.5f);
        }
    }

    public void ShieldDown()
    {
        shield.transform.localPosition = Vector3.MoveTowards(shield.transform.localPosition, shieldStartPos, Time.deltaTime * 1.5f);
        if (shield.transform.localPosition == shieldStartPos)
        {
            shield.SetActive(false);
            weapons[currentWeapon].SetActive(true);
        }
        else
        {
            ShieldDown();
        }
    }

    private IEnumerator TurtleCollision()
    {
        inTurtleCollision = true;
        playerHealth -= 25;
        yield return new WaitForSeconds(1.0f);
        inTurtleCollision = false;
    }

    private IEnumerator GetAttacked()
    {
        playerHealth -= 10;
        yield return new WaitForSeconds(1.0f);
        beingAttacked = false;
    }

    public void UpdateScreen()
    {
        if (currentArrowInPickUpRange != null)
        {
            arrowPickUpText.enabled = true;
        } else
        {
            arrowPickUpText.enabled = false;
        }

        if (weapons[currentWeapon] == bow) {
            arrowText.text = currentArrowCount + " Arrows Left";
        } else
        {
            arrowText.text = "";
        }

        playerText.text = "Health = " + playerHealth;
        float xScale = playerHealth * startHealthBar / startHealth;
        float yScale = playerHealthBar.gameObject.transform.localScale.y;
        float zScale = playerHealthBar.gameObject.transform.localScale.z;
        playerHealthBar.gameObject.transform.localScale = new Vector3(xScale, yScale, zScale);
    }
}
