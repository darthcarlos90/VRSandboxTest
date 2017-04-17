using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

	private SteamVR_TrackedObject trackedObj;

	// Stores the object that is being activated by the trigger
	private GameObject collidingObject;
	
	// A reference to the object that the player is currently handling
	private GameObject objectInHand;

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	void Awake() {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	private void SetCollidingObject (Collider col)
	{
		if (collidingObject || !col.GetComponent<Rigidbody>()) return;

		collidingObject = col.gameObject;
	}

	public void OnTriggerEnter(Collider other)
	{
		SetCollidingObject(other);
	}

	public void OnTriggerStay(Collider other)
	{
		SetCollidingObject(other);
	}

	public void OnTriggerExit(Collider other)
	{
		if (!collidingObject) return;

		collidingObject = null;
	}

	private void GrabObject()
	{
		// Move the colliding object to the object in hand reference
		objectInHand = collidingObject;
		
		// Release this reference
		collidingObject = null;

		var joint = AddFixedJoint();
		joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
	}

	private void ReleaseObject()
	{
		// If there is a fixed joint with this current object
		if(GetComponent<FixedJoint>())
		{
			// Remove the connections and destroy the fixed joint
			GetComponent<FixedJoint>().connectedBody = null;
			Destroy(GetComponent<FixedJoint>());

			// Add rotation and velocity to the object, so it feels like correctly thrown
			objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
			objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;	
		}

		// remove the reference of the object
		objectInHand = null;
	}

	private FixedJoint AddFixedJoint()
	{
		FixedJoint fx = gameObject.AddComponent<FixedJoint>();
		fx.breakForce = 20000;
		fx.breakTorque = 20000;

		return fx;
	}
	
	// Update is called once per frame
	void Update () {
		if(Controller.GetHairTriggerDown())
		{
			if (collidingObject) GrabObject();
		}

		if(Controller.GetHairTriggerUp())
		{
			if (objectInHand) ReleaseObject();
		}
	}
}
