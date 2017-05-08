using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastingInteraction : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject collidingObject;
    private GameObject objectInHand;
    private bool grabbed = false;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Start()
    {
        //  Spawn a new laser and save a reference to it in laser.
        laser = Instantiate(laserPrefab);
        // Store the laser’s transform component.
        laserTransform = laser.transform;
    }


    void Update()
    {

        RaycastHit hit;

        // Shoot a ray from the controller.If it hits something, make it store the point where it hit and show the laser.
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
        {
            hitPoint = hit.point;
            ShowLaser(hit);
        }
        //If the trigger is held down…
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            GrabObject();
            grabbed = true;
        }
        else {
            if (grabbed == true) {
                ReleaseObject();
                grabbed = false;
            }
        }
    }

    // TO DO: Grabbing Interaction
   private void GrabObject()
    {
        Debug.Log(gameObject.name + " pressed. Grabbing Interaction should be performed.");
        //// Move the GameObject inside the player’s hand and remove it from the collidingObject variable.
        //objectInHand = collidingObject;
        //collidingObject = null;
        //// Add a new joint that connects the controller to the object using the AddFixedJoint() method below.

        //var joint = AddFixedJoint();
        //joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    // Make a new fixed joint, add it to the controller, and then set it up so it doesn’t break easily.Finally, you return it.
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        //   Make sure there’s a fixed joint attached to the controller.
        if (GetComponent<FixedJoint>())
        {
            // Remove the connection to the object held by the joint and destroy the joint.
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // Add the speed and rotation of the controller when the player releases the object, so the result is a realistic arc.
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        // Remove the reference to the formerly attached object.
        objectInHand = null;
    }


    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }


    private void ShowLaser(RaycastHit hit)
    {
        //  Show the laser.
        laser.SetActive(true);
        // Position the laser between the controller and the point where the raycast hits.
        //You use Lerp because you can give it two positions and the percent it should travel.If you pass it 0.5f, which is 50%, it returns the precise middle point.
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        // Point the laser at the position where the raycast hit.
        laserTransform.LookAt(hitPoint);
        // Scale the laser so it fits perfectly between the two positions.
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }
}