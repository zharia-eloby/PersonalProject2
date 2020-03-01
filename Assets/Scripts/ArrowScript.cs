using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ArrowState { DEAD, IDLE, SHOOT_ARROW, SHOT };

public class ArrowScript : MonoBehaviour
{
    public static GameObject bow;
    float pickUpDistance;
    float arrowMaxDistance;
    public static Vector3 arrowStartPos;
    Vector3 shotPosition;
    public static string shootKey;
    string arrowPickUpKey;

    protected ArrowState state = ArrowState.IDLE;

    void Start()
    {
        bow = transform.parent.gameObject;
        pickUpDistance = 1.5f;
        arrowMaxDistance = 30.0f;
        arrowStartPos = transform.localPosition;
        shootKey = "m";
        arrowPickUpKey = "k";
    }

    void Update()
    {
        switch (state)
        {
            case ArrowState.DEAD:
                if (PlayerScript.currentArrowInPickUpRange == null && Vector3.Distance(bow.transform.position, transform.position) < pickUpDistance)
                {
                    PlayerScript.currentArrowInPickUpRange = this.gameObject;
                } else  if (PlayerScript.currentArrowInPickUpRange == this.gameObject && Vector3.Distance(bow.transform.position, transform.position) > pickUpDistance)
                {
                    PlayerScript.currentArrowInPickUpRange = null;
                }
                if (Input.GetKeyDown(arrowPickUpKey) && (PlayerScript.currentArrowCount < PlayerScript.arrowCapacity) && Vector3.Distance(bow.transform.position, transform.position) < pickUpDistance)
                {
                    PlayerScript.currentArrowCount++;
                    Debug.Log("arrow picked up. total arrows = " + PlayerScript.currentArrowCount);
                    if (PlayerScript.currentArrowCount == 1)
                    {
                        StartCoroutine(NextArrowSetup(true));
                    }
                    else
                    {
                        if (PlayerScript.currentArrowInPickUpRange == this.gameObject)
                        {
                            PlayerScript.currentArrowInPickUpRange = null;
                        }
                        Destroy(this.gameObject);
                    }
                }
                break;
            case ArrowState.IDLE:
                if (Input.GetKeyDown(shootKey))
                {
                    state = ArrowState.SHOOT_ARROW;
                }
                break;
            case ArrowState.SHOOT_ARROW:
                PlayerScript.currentArrowCount--;
                shotPosition = transform.position;
                this.GetComponent<Rigidbody>().AddForce(-transform.up*7);
                this.GetComponent<Rigidbody>().AddTorque(transform.up*200);
                //Debug.Log("arrow shot. total arrows = " + PlayerScript.currentArrowCount);
                transform.SetParent(null);
                state = ArrowState.SHOT;
                if (PlayerScript.currentArrowCount > 0) {
                    StartCoroutine(NextArrowSetup(false));
                }
                break;
            case ArrowState.SHOT:
                if (Vector3.Distance(shotPosition, transform.position) > arrowMaxDistance)
                {
                    KillArrow();
                    state = ArrowState.DEAD;
                }
                break;
            default:
                break;
        }
    }

    private IEnumerator NextArrowSetup(bool destroyArrow)
    {
        yield return new WaitForSeconds(0.25f);
        //Debug.Log("currentArrowCount: " + PlayerScript.currentArrowCount);

        GameObject nextArrow = Instantiate(this.gameObject, bow.transform, true) as GameObject;
        nextArrow.tag = "Arrow";
        nextArrow.GetComponent<Rigidbody>().velocity = Vector3.zero;
        nextArrow.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        nextArrow.GetComponent<CapsuleCollider>().isTrigger = true;
        nextArrow.GetComponent<Rigidbody>().useGravity = false;
        nextArrow.transform.localEulerAngles = new Vector3(0, 0, 90);
        nextArrow.transform.localPosition = arrowStartPos;
        if (destroyArrow)
        {
            Destroy(this.gameObject);
        }
    }

    public void KillArrow()
    {
        Debug.Log("Kill Arrow");
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        this.GetComponent<CapsuleCollider>().isTrigger = false;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.tag = "Untagged";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            Debug.Log("2");
            state = ArrowState.DEAD;
            KillArrow();
        }
    }
}
