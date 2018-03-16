using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class FenrirHandInput : MonoBehaviour {
    public EVRButtonId radarButton = EVRButtonId.k_EButton_SteamVR_Trigger;
    public EVRButtonId invokeButton = EVRButtonId.k_EButton_SteamVR_Trigger;
    
    public FenrirVRController cont;

    public GameObject radarCanvas;
    public Image cursorImage;
    RectTransform cursorRT;
    public LineRenderer lr;
    public GameObject player;
    public float speed;
    public GameObject cameraObject;

    public void Start()
    {
        cont = GetComponent<FenrirVRController>();
        cursorRT = cursorImage.GetComponent<RectTransform>();
    }

    Vector2 touchpad;
    private void Update()
    {
        Vector3 localForward = cameraObject.transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(cameraObject.transform.position, localForward * 10, Color.blue);
        player.transform.Translate(localForward * speed * Time.deltaTime);
        if (cont.device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (lr != null)
            {
                lr.SetPosition(0, transform.position);
                RaycastHit rayHit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, 100))
                {

                    if (lr != null)
                    {
                        lr.SetPosition(1, rayHit.point);
                    }
                }
                else
                { 
                    if (lr != null)
                    {
                        lr.SetPosition(1, transform.TransformDirection(Vector3.forward) * 100);
                    }
                }
            }

            touchpad = (cont.device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            cursorRT.anchoredPosition = new Vector2(touchpad.x * 100, touchpad.y * 100);
            

            

            /*float dist = Vector2.Distance(touchpad, new Vector2(0, 0));
            // Debug.Log("Estamos levantando el dedo del touchpad");
            Vector3 dirVector = new Vector3(touchpad.x, 0, touchpad.y).normalized;
            if (dist > 0.5f)
            {
                player.transform.Translate((localForward + dirVector) * speed * Time.deltaTime);
            }*/
            
            
        }
        /*if (cont.device.GetPressDown(radarButton))
        {
            radarCanvas.SetActive(true);
            lr.enabled = true;
        }
        else if (cont.device.GetPressUp(radarButton))
        {
            lr.enabled = false;
            Debug.Log(touchpad.x + " " + touchpad.y);
            float dist = Vector2.Distance(touchpad, new Vector2(0, 0));
            // Debug.Log("Estamos levantando el dedo del touchpad");
            if (dist>0.6f)
            {
                Summon(); 
            }
            radarCanvas.SetActive(false);
        }*/

    }

    public void Summon()
    {
        //GameObject instance = Instantiate(inventory[CurMenuItem], transform.position, Quaternion.identity) as GameObject;
        //Dogdeball db = instance.GetComponent<Dogdeball>();
        //db.manager = DodgeballManager.GetInstance();
        //db.isHolded = false;
        //db.isReady = true;
        //instance.GetComponent<Rigidbody>().AddForce(cont.transform.TransformDirection(Vector3.forward) * 10, ForceMode.Impulse);
    
    }

    Vector2 fromVector2 = new Vector2(0.5f, 1f);
    Vector2 centerCircle = new Vector2(0.5f, 0.5f);
}
