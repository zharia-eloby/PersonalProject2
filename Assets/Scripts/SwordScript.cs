using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwordState { IDLE, LUNGE, MOVE_TO_START, SWING_UP, SWING_LEFT };
public class SwordScript : MonoBehaviour
{
    protected SwordState state = SwordState.IDLE;
    float swordSpeed;
    Vector3 swordStartPos;
    Quaternion swordStartRotation;
    Quaternion swordSwingUpEndRotation;
    Quaternion swordSwingLeftEndRotation;
    Vector3 startRotation;
    Vector3 swingUpEndRotation;
    Vector3 swingLeftEndRotation;
    Vector3 lungeEndPos;
    Vector3 swingUpEndPos;
    Vector3 swingLeftEndPos;
    string lungeKey;
    string swingKey;

    void Start()
    {
        swordSpeed = 2.5f;

        swordStartPos = transform.localPosition;

        swordStartRotation = transform.localRotation;
        swordSwingUpEndRotation = new Quaternion(-30.82f, -118.264f, -2.667f, 0.0f);
        swordSwingLeftEndRotation = new Quaternion(-14.19f, -145.036f, -59.164f, 0.0f);

        startRotation = transform.localEulerAngles;
        swingUpEndRotation = new Vector3(-30.82f, -118.264f, -2.667f);
        swingLeftEndRotation = new Vector3(-14.19f, -145.036f, -59.164f);

        lungeEndPos = new Vector3(0.022f, -0.144f, 0.663f);
        swingUpEndPos = new Vector3(0.228f, -0.031f, 0.313f);
        swingLeftEndPos = new Vector3(-0.142f, -0.121f, 0.663f); //(-0.142f, -0.121f, 0.385f); 

        lungeKey = "b";
        swingKey = "v";
    }

    void Update()
    {
        switch (state)
        {
            case SwordState.IDLE:
                if (Input.GetKeyDown(swingKey))
                {
                    state = SwordState.SWING_UP;
                }
                else if (Input.GetKeyDown(lungeKey))
                {
                    state = SwordState.LUNGE;
                }
                break;
            case SwordState.LUNGE:
                LungeSword();
                if (transform.localPosition == lungeEndPos)
                {
                    state = SwordState.MOVE_TO_START;
                }
                break;
            case SwordState.MOVE_TO_START:
                MoveToStartPos();
                if (transform.localPosition == swordStartPos)
                {
                    state = SwordState.IDLE;
                }
                break;
            case SwordState.SWING_UP:
                SwingUp();
                if (transform.localPosition == swingUpEndPos)
                {
                    state = SwordState.SWING_LEFT;
                }
                break;
            case SwordState.SWING_LEFT:
                SwingLeft();
                if (transform.localPosition == swingLeftEndPos)
                {
                    state = SwordState.MOVE_TO_START;
                }
                break;
            default:
                break;
        }
    }

    public void LungeSword()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, lungeEndPos, Time.deltaTime * swordSpeed);
    }

    public void MoveToStartPos()
    {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, swordStartRotation, 360.0f);
        //transform.localRotation = Quaternion.FromToRotation(transform.localEulerAngles, startRotation);

        /**Quaternion targetRotation = Quaternion.FromToRotation(transform.localEulerAngles, startRotation); //Quaternion.RotateTowards(transform.localRotation, swordStartRotation, 360.0f);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Time.deltaTime * 10);*/

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, swordStartPos, Time.deltaTime * swordSpeed);
    }

    public void SwingLeft()
    {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, swordStartRotation, 360.0f); //swordSwingLeftEndRotation, 360.0f);
        //transform.localRotation = Quaternion.FromToRotation(transform.localEulerAngles, swingLeftEndRotation);

        /**Quaternion targetRotation = Quaternion.FromToRotation(transform.localEulerAngles, swingLeftEndRotation); //Quaternion.RotateTowards(transform.localRotation, swordSwingLeftEndRotation, 360.0f);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Time.deltaTime * 10);*/

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, swingLeftEndPos, Time.deltaTime * swordSpeed);
    }

    public void SwingUp()
    {
       ///transform.localRotation = Quaternion.FromToRotation(transform.localEulerAngles, swingUpEndRotation);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, swordSwingUpEndRotation, 360.0f);

        /**Quaternion targetRotation = Quaternion.FromToRotation(transform.localEulerAngles, swingUpEndRotation); //Quaternion.RotateTowards(transform.localRotation, swordSwingUpEndRotation, 360.0f); 
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Time.deltaTime * 10);*/

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, swingUpEndPos, Time.deltaTime * swordSpeed);
    }
}
