using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measure : MonoBehaviour {
    public Transform leftController;
    public Transform rightController;

    public GameObject tapePrefab;
    private List<GameObject> tapes;

    private GameObject tapeCopy;
    private Vector3 initialPoint;
    private bool isLeft;

    void Start() {
        tapes = new List<GameObject>();
    }

    void Update() {
        if (tapeCopy == null) {
            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
                foreach(GameObject go in tapes) {
                    Destroy(go);
                }

                tapes.Clear();
                return;
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)) {
                initialPoint = leftController.position;
                tapeCopy = Instantiate<GameObject>(tapePrefab);
                tapes.Add(tapeCopy);
                isLeft = true;
            } else if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
                initialPoint = rightController.position;
                tapeCopy = Instantiate<GameObject>(tapePrefab);
                tapes.Add(tapeCopy);
                isLeft = false;
            }
        } else {
            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
                Destroy(tapeCopy);
                tapeCopy = null;
                return;
            }

            if (isLeft) {
                UpdateTape(tapeCopy, initialPoint, leftController.position);

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)) {
                    tapeCopy = null;
                }
            } else {
                UpdateTape(tapeCopy, initialPoint, rightController.position);

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
                    tapeCopy = null;
                }
            }
        }
    }

    private void UpdateTape(GameObject reference, Vector3 initial, Vector3 final) {
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
    }

    private void UpdateDistanceLabel(GameObject canvas, float distance) {
        UnityEngine.UI.Text label = canvas.GetComponentInChildren<UnityEngine.UI.Text>();
        label.text = $"{(int)System.Math.Floor(distance * 100.0f)} cm";
    }
}
