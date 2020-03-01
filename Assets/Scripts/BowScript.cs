using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BowState {IDLE, ZOOM_IN, ZOOM_OUT, AIM};
public class BowScript : MonoBehaviour
{
    protected BowState state = BowState.IDLE;
    GameObject cam;
    Vector3 bowStartPos;
    Vector3 zoomPos;
    float zoomSpeed;
    string zoomKey;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera");
        bowStartPos = transform.localPosition;
        zoomPos = new Vector3(0.063f, -0.011f, 0.066f);
        zoomSpeed = 2.0f;
        zoomKey = "n";
    }

    // Update is called once per frame
    void Update()
    {
        /**if (Input.GetKeyDown(ArrowScript.shootKey))
        {
            state = BowState.IDLE;
        }*/
        switch (state)
        {
            case BowState.IDLE:
                if (Input.GetKeyDown(zoomKey))
                {
                    state = BowState.ZOOM_IN;
                }
                break;
            case BowState.ZOOM_IN:
                ZoomIn();
                if (transform.localPosition == zoomPos)
                {
                    state = BowState.AIM;
                }
                break;
            case BowState.ZOOM_OUT:
                ZoomOut();
                if (transform.localPosition == bowStartPos)
                {
                    state = BowState.IDLE;
                }
                break;
            case BowState.AIM:
                if (Input.GetKeyDown(zoomKey)) //zoom out
                {
                    state = BowState.ZOOM_OUT;
                }
                break;
            default:
                break;
        }
    }

    public void ZoomIn()
    {
        while (cam.GetComponent<Camera>().fieldOfView > 45)
        {
            cam.GetComponent<Camera>().fieldOfView -= 0.5f;
        }
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, zoomPos, Time.deltaTime * zoomSpeed);
    }

    public void ZoomOut()
    {
        while (cam.GetComponent<Camera>().fieldOfView < 60)
        {
            cam.GetComponent<Camera>().fieldOfView += 1.0f;
        }
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, bowStartPos, Time.deltaTime * (zoomSpeed * 2));
    }

}
