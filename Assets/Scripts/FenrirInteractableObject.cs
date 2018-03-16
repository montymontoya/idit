using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//[RequireComponent(typeof(Rigidbody))]
public class FenrirInteractableObject : MonoBehaviour {
    public Rigidbody rigidBody;
    public bool originalKinematicState;
    public bool isStatic;
    protected Transform originalParent;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        //Capture object's original parent and kinematic state
        if (rigidBody!=null)
        {
            originalParent = transform.parent;
            originalKinematicState = rigidBody.isKinematic;
        }
        
    }

    public void Pickup(FenrirVRController controller)
    {
        //Make object kinematic
        //(Not effected by physics, but still able to effect other objects with physics)
        Debug.Log(gameObject.name);
        //rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;

        //Parent object to hand
        transform.SetParent(controller.gameObject.transform);
    }

    public void Release(FenrirVRController controller)
    {
        //Make sure the hand is still the parent. 
        //Could have been transferred to anothr hand.
        if (transform.parent == controller.gameObject.transform)
        {
            //Return previous kinematic state
            rigidBody.isKinematic = originalKinematicState;

            //Set object's parent to its original parent
            if (originalParent != controller.gameObject.transform)
            {
                //Ensure original parent recorded wasn't somehow the controller (failsafe)
                transform.SetParent(originalParent);
            }
            else
            {
                transform.SetParent(null);
            }

            //Throw object
            rigidBody.velocity = controller.device.velocity;
            rigidBody.angularVelocity = controller.device.angularVelocity;
        }

    }

    public virtual void TriggerEnter(FenrirVRController controller)
    {                                
                                     
    }                                
    public virtual void TriggerExit(FenrirVRController controller)
    {

    }

    public virtual void ButtonPressDown(EVRButtonId button, FenrirVRController controller)
    {

    }
    public virtual void ButtonPressUp(EVRButtonId button, FenrirVRController controller)
    {

    }

    public virtual void RayEnter(RaycastHit hit, FenrirVRController controller = null)
    {

    }
    public virtual void RayStay(RaycastHit hit, FenrirVRController controller = null)
    {

    }
    public virtual void RayExit(FenrirVRController controller = null)
    {

    }
}
