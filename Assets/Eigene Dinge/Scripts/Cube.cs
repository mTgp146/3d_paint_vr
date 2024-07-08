using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Cube : MonoBehaviour
{
    [Header("Pen Properties")]
    public Transform pos;
    public Material drawingMaterial;
    [Range(0.01f, 01f)]
    private float cubeSize;

    [Header("XR Interaction")]
    private XRGrabInteractable grabInteractable;
    private bool leftController = false;
    private bool rightController = false;

    private GameObject newCube;
    private bool start = false; 
    private bool activated = false;
    private bool selected = false;
    private bool colorChanged = false;
    private bool colorChanging = false;
    private GameObject shownCube;


    void Start() {
        cubeSize = 0.05f;

        // Hole die XRGrabInteractable Komponente vom Stift
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Abonniere die Events
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);

        shownCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shownCube.transform.position = new Vector3(pos.position.x, pos.position.y, pos.position.z);
        shownCube.transform.rotation = pos.rotation;
        shownCube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
        shownCube.GetComponent<Renderer>().material = drawingMaterial;
    }

    void Update() {

        bool leftHandSecondaryButtonDown = false;
        bool rightHandSecondaryButtonDown = false;

        shownCube.transform.position = new Vector3(pos.position.x, pos.position.y, pos.position.z);
        shownCube.transform.rotation = pos.rotation;
        shownCube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        // Hole den aktuellen Status der SecondaryButton
        if (selected) {
            List<InputDevice> controllers = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, controllers);
            if (controllers.Count > 0) {
                controllers[0].TryGetFeatureValue(CommonUsages.secondaryButton, out leftHandSecondaryButtonDown);
            }
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, controllers);
            if (controllers.Count > 0) {
                controllers[0].TryGetFeatureValue(CommonUsages.secondaryButton, out rightHandSecondaryButtonDown);
            }
        }
        // Wenn der SecondaryButton gedrückt wird, wechsle die Farbe
        if (((leftHandSecondaryButtonDown && leftController) || (rightHandSecondaryButtonDown && rightController)) && !colorChanged) {
            SwitchColor();
            colorChanged = true;
        } else if (!(leftHandSecondaryButtonDown && leftController) && !(rightHandSecondaryButtonDown && rightController) && colorChanged) {
            colorChanged = false;
        }

        // Wenn der Stift aktiviert ist, zeichne
        if (activated && !colorChanging) {
            SpawnCube();
        }
    }

    public void Activate() {
        if (activated == false) {
            activated = true;
        }
    }

    public void Deactivate() {
        if (activated == true) {
            activated = false;
            newCube = null;
            start = false;
        }
    }

    public void Select() {
        if (selected == false) {
            selected = true;
        }
    }

    public void Deselect() {
        if (selected == true) {
            selected = false;
            //pos.localScale = new Vector3(1.25f, 1.25f, 0.83333f);
            cubeSize = 0.05f;
        }
    }

    public void SpawnCube() {
        if(start == false) {
            // Create a new material instance for the LineRenderer
            Material newMaterial = new Material(drawingMaterial);
            newMaterial.color = drawingMaterial.color;

            newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newCube.tag = "Painted";
            BoxCollider collider = newCube.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            
            newCube.transform.position = new Vector3(pos.position.x, pos.position.y, pos.position.z);
            newCube.transform.rotation = pos.rotation;
            newCube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
            newCube.GetComponent<Renderer>().material = newMaterial;
            start = true;
        } 
    }

    void SwitchColor() {
        Deactivate();
        colorChanging = true;
        start = false;
        ColorPicker.Create(cubeSize, drawingMaterial.color, "Choose a new color", SetColor, ColorFinished, true);
    }

    private void SetColor(Color currentColor, float rad) {
        drawingMaterial.color = currentColor;
        cubeSize = rad;
    }

    private void ColorFinished(Color finishedColor, float rad) {
        colorChanging = false;
    }

    private void OnSelectEntered(SelectEnterEventArgs args) {
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
        if (interactor != null) {
            if (interactor.gameObject.name == "Left Controller") {
                leftController = true;
            } else {
                rightController = true;
            }
        }
    }

    private void OnSelectExited(SelectExitEventArgs args) {
        leftController = false;
        rightController = false;
        colorChanging = false;
    }

    void OnDestroy() {
        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }
}
