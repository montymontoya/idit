using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.Linq;

public struct FenrirEventArgs
{
    public uint controllerIndex;
    public uint flags;
    public float padX, padY;
}

public delegate void FenrirEventHandler(object sender, FenrirEventArgs e);

public class FenrirVRController : MonoBehaviour {

    /*
    
    Device.GetPressDown(), Device.GetPressUp(), and device.GetPress() will return a bool letting us know if the button given is being pressed.
    Device.GetTouchDown(), Device.GetTouchUp(), and Device.GetTouch() will return a bool letting us know if the button given is being touched. This is similar to GetPress in some cases, for example the trigger, GetTouch will return true before the trigger button is all the way down.
    Device.GetAxis() returns the location of the user’s thumb on the touchpad (zero by default).
    Device.Velocity and Device.AngularVelocity are properties that the device has denoting the controller’s current velocity and angular velocity.
    Device.TriggerhapticPulse() can be used to send haptic feedback to the controllers.
*/

    public event FenrirEventHandler PadClicked;
    public event FenrirEventHandler PadUnclicked;
    protected Dictionary<EVRButtonId, List<FenrirInteractableObject>> pressDownObjects;
   
    protected List<FenrirInteractableObject> heldObjects;
    protected List<EVRButtonId> buttonsTracked;
    public Transform pickupPoint;
    public FenrirInteractableObject currentHeldObject;

    //Controller References
    protected SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device
    {
        get
        {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }

    void Awake()
    {
        //Instantiate lists
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        heldObjects = new List<FenrirInteractableObject>();
        pressDownObjects = new Dictionary<EVRButtonId, List<FenrirInteractableObject>>();

        buttonsTracked = new List<EVRButtonId>()
        {
            EVRButtonId.k_EButton_SteamVR_Trigger,
            EVRButtonId.k_EButton_Grip,
            EVRButtonId.k_EButton_SteamVR_Touchpad
        };
    }


    void Update()
    {

        //Check through all desired buttons to see if any have been released
        /*for (int i = 0; i < buttonsTracked.Count; i++)
        {
            if (device.GetPressDown(buttonsTracked[i]))
            {
                Debug.Log("Hemos presionado el boton " + buttonsTracked[i].ToString());
            }
        }*/
        EVRButtonId[] pressKeys = pressDownObjects.Keys.ToArray();
        for (int i = 0; i < pressKeys.Length; i++)
        {
            //If tracked button is released
           
            if (device.GetPressUp(pressKeys[i]))
            {
                //Get all tracked objects in that button's "pressed" list
                List<FenrirInteractableObject> releaseObjects = pressDownObjects[pressKeys[i]];
                for (int j = 0; j < releaseObjects.Count; j++)
                {
                    //Send button release through to interactable script
                    releaseObjects[j].ButtonPressUp(pressKeys[i], this);
                }

                //Clear 
                pressDownObjects[pressKeys[i]].Clear();
                currentHeldObject = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If rigidbody's object has interactable item scripts, iterate through them
        FenrirInteractableObject[] interactables = other.GetComponents<FenrirInteractableObject>();
        for (int i = 0; i < interactables.Length; i++)
        {
            FenrirInteractableObject interactable = interactables[i];
            interactable.TriggerEnter(this);
        }
        if (other.attachedRigidbody != null)
        {
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FenrirInteractableObject[] interactables = other.GetComponents<FenrirInteractableObject>();
        for (int i = 0; i < interactables.Length; i++)
        {
            FenrirInteractableObject interactable = interactables[i];
            interactable.TriggerExit(this);
        }
        if (other.attachedRigidbody != null)
        {
            //If rigidbody's object has interactable item scripts, iterate through them
            
        }
    }

    void OnTriggerStay(Collider collider)
    {
        //If collider has a rigid body to report to
        if (collider.attachedRigidbody != null)
        {
            //If rigidbody's object has interactable item scripts, iterate through them
            FenrirInteractableObject[] interactables = collider.attachedRigidbody.GetComponents<FenrirInteractableObject>();
            if (interactables.Length > 0)
            {
                for (int i = 0; i < interactables.Length; i++)
                {
                    if (!interactables[i].isStatic)
                    {
                        currentHeldObject = interactables[i];
                    }
                }     
            }
            for (int i = 0; i < interactables.Length; i++)
            {
                FenrirInteractableObject interactable = interactables[i];
                for (int b = 0; b < buttonsTracked.Count; b++)
                {
                    //If a tracked button is pressed
                    EVRButtonId button = buttonsTracked[b];
                    if (device.GetPressDown(button))
                    {
                        //If we haven't already sent the button press message to this interactable
                        //Safeguard against objects that have multiple colliders for one interactable script
                        if (!pressDownObjects.ContainsKey(button) || !pressDownObjects[button].Contains(interactable))
                        {
                            //Send button press through to interactable script
                            interactable.ButtonPressDown(button, this);

                            //Add interactable script to a dictionary flagging it to recieve notice
                            //when that same button is released
                            if (!pressDownObjects.ContainsKey(button))
                                pressDownObjects.Add(button, new List<FenrirInteractableObject>());

                            pressDownObjects[button].Add(interactable);
                        }
                    }
                }
            }
        }
    }

    public virtual void OnPadClicked(FenrirEventArgs e)
    {
        if (PadClicked != null)
            PadClicked(this, e);
    }

    public virtual void OnPadUnclicked(FenrirEventArgs e)
    {
        if (PadUnclicked != null)
            PadUnclicked(this, e);
    }
}
