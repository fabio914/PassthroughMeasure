using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measure : MonoBehaviour {
    public Transform leftController;
    public Transform rightController;

    public GameObject cylinderReference;
    private List<GameObject> cylinders;

    private GameObject cylinderCopy;
    private Vector3 initialPoint;
    private bool isLeft;

    void Start() {
        cylinders = new List<GameObject>();
    }

    void Update() {
        if (cylinderCopy == null) {
            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
                foreach(GameObject go in cylinders) {
                    Destroy(go);
                }

                cylinders.Clear();
                return;
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)) {
                initialPoint = leftController.position;
                cylinderCopy = Instantiate<GameObject>(cylinderReference);
                cylinders.Add(cylinderCopy);
                isLeft = true;
            } else if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
                initialPoint = rightController.position;
                cylinderCopy = Instantiate<GameObject>(cylinderReference);
                cylinders.Add(cylinderCopy);
                isLeft = false;
            }
        } else {
            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
                Destroy(cylinderCopy);
                cylinderCopy = null;
                return;
            }

            if (isLeft) {
                UpdateCylinder(cylinderCopy, initialPoint, leftController.position);

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)) {
                    cylinderCopy = null;
                }
            } else {
                UpdateCylinder(cylinderCopy, initialPoint, rightController.position);

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
                    cylinderCopy = null;
                }
            }
        }
    }

    private void UpdateCylinder(GameObject reference, Vector3 initial, Vector3 final) {
        float distance = Vector3.Distance(initial, final);

        foreach (Transform child in reference.transform) {
            if (child.name == "CylinderParent") {
                Vector3 newChildScale = child.localScale;
                newChildScale.z = distance/2.0f;
                child.transform.localScale = newChildScale;

                child.transform.position = initial;
                child.transform.LookAt(final);
            } else if (child.name == "FrontCanvas") {
                child.transform.position = initial + (final - initial)/2.0f;
                child.transform.LookAt(final);
                child.transform.Rotate(0.0f, -90.0f, 0.0f, Space.Self);
                child.transform.Translate(0.0f, 0.075f, 0.0f, Space.Self);
                UpdateDistanceLabel(child.gameObject, distance);
            } else if (child.name == "BackCanvas") {
                child.transform.position = initial + (final - initial)/2.0f;
                child.transform.LookAt(final);
                child.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
                child.transform.Translate(0.0f, 0.075f, 0.0f, Space.Self);
                UpdateDistanceLabel(child.gameObject, distance);
            }
        }

        reference.SetActive(true);
    }

    private void UpdateDistanceLabel(GameObject canvas, float distance) {
        UnityEngine.UI.Text label = canvas.GetComponentInChildren<UnityEngine.UI.Text>();
        label.text = $"{(int)System.Math.Floor(distance * 100.0f)} cm";
    }
}
