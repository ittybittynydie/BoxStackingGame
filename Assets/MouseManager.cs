using System.Collections;
using UnityEngine;

public class MouseManager : MonoBehaviour {

	public bool useSpring = false;

	public LineRenderer dragLine;


	Rigidbody2D grabbedObject = null;
	SpringJoint2D springJoint = null;

	float velocityRatio = 4f; //If we aren't using a spring

	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			// We clicked, but on what? This function checks to see if the mouse button has been clicked. But nothing specifies, what has been clicked on.

			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector2 mousePos2D = new Vector2 (mouseWorldPos3D.x, mouseWorldPos3D.y);

			Vector2 dir = Vector2.zero;

			RaycastHit2D hit = Physics2D.Raycast (mousePos2D, dir);
			if (hit.collider != null) {
				// We clicked on SOMETHING that has a collider. This uses the mouse position to cast a ray of affect to find an object with a collider and change it's direction to zero, therefore making it stop on the mouse.

				if (hit.collider.GetComponent<Rigidbody2D> () != null) {
					grabbedObject = hit.collider.GetComponent<Rigidbody2D> ();

					if (useSpring) {
						springJoint = grabbedObject.gameObject.AddComponent<SpringJoint2D> ();
						//Set the anchor to the spot on the object that we clicked
						Vector3 localHitPoint = grabbedObject.transform.InverseTransformPoint (hit.point);					
						springJoint.anchor = localHitPoint;
						springJoint.connectedAnchor = mouseWorldPos3D;
						springJoint.distance = 0.25f;
						springJoint.dampingRatio = 2;
						springJoint.frequency = 3;

						//Enable this if you want to collide with objects - Unity5 version
						//This will also ENGAGE the spring
						springJoint.enableCollision = true;
						springJoint.autoConfigureDistance = false;

						//This will also ENGAGE the spring, even if it's a completely
						//redundant line because the connectedBody should already by null
						//springJoint.enableCollision= null;
					} 
					else {
						//We're using velocity instead
						grabbedObject.gravityScale = 0;
				
					}

					dragLine.enabled = true;
				}
			}
		}

		if (Input.GetMouseButtonUp (0) && grabbedObject != null) {
			if (useSpring) {
				Destroy (springJoint);
				springJoint = null;
			} else {
				grabbedObject.gravityScale = 1;
			}
			grabbedObject = null;
			dragLine.enabled = false;
			//This function tells the game to change the object gravity back to one from zero once the mouse button is unclicked and therefore in the up position.
		}
	}

	void FixedUpdate () {
		if (grabbedObject != null) {
			Vector2 mouseWorldPos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(useSpring){
				springJoint.connectedAnchor = mouseWorldPos2D;
			}
			else {
				grabbedObject.velocity = (mouseWorldPos2D - grabbedObject.position) * velocityRatio;

				}
			}
		}

	void LateUpdate () {
		if (grabbedObject != null) {
			if (useSpring) {
					Vector3 worldAnchor = grabbedObject.transform.TransformPoint (springJoint.anchor);
					dragLine.SetPosition (0, new Vector3 (worldAnchor.x, worldAnchor.y, -1));
					dragLine.SetPosition (1, new Vector3 (springJoint.connectedAnchor.x, springJoint.connectedAnchor.y, -1));
			}
				else{
					Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					dragLine.SetPosition (0, new Vector3 (grabbedObject.position.x, grabbedObject.position.y, -1));
					dragLine.SetPosition (1, new Vector3 (mouseWorldPos3D.x, mouseWorldPos3D.y, -1));
				}
		}
	}
}
