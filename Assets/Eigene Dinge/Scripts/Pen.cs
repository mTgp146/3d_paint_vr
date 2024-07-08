using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Pen : MonoBehaviour
{
    [Header("Pen Properties")]
    public Transform tip;
    public Material drawingMaterial;
    [Range(0.01f, 01f)]
    public float penWidth = 0.05f;

    [Header("XR Interaction")]
    private XRGrabInteractable grabInteractable;
    private bool leftController = false;
    private bool rightController = false;

    private LineRenderer drawing;
    private int indexPosCount = 0;
    private bool start = false; 
    private bool activated = false;
    private bool selected = false;
    private bool colorChanged = false;
    private bool colorChanging = false;


    void Start() {

        // Hole die XRGrabInteractable Komponente vom Stift
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Abonniere die Events
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    void Update() {
        bool leftHandSecondaryButtonDown = false;
        bool rightHandSecondaryButtonDown = false;

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
            Draw();
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
            drawing = null;
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
        }
    }

    public void Draw() {
        if (start == false) {
            indexPosCount = 0;
            drawing = new GameObject().AddComponent<LineRenderer>();
            drawing.tag = "Painted";

            // Create a new material instance for the LineRenderer
            Material newMaterial = new Material(drawingMaterial);
            drawing.material = newMaterial;
            newMaterial.color = drawingMaterial.color;

            // Set the initial color based on the drawingMaterial's color
            drawing.startColor = drawing.endColor = newMaterial.color;
            drawing.startWidth = drawing.endWidth = penWidth/5;
            drawing.positionCount = 1;
            drawing.SetPosition(0, tip.position);
            start = true;
        } else {
            var currentPosition = drawing.GetPosition(indexPosCount);
            if (Vector3.Distance(currentPosition, tip.position) > 0.01f) {
                indexPosCount++;
                drawing.positionCount = indexPosCount + 1;
                drawing.SetPosition(indexPosCount, tip.position);
            }
        }
    }

    void SwitchColor() {
        Deactivate();
        colorChanging = true;
        drawing = null;
        start = false;
        ColorPicker.Create(penWidth, drawingMaterial.color, "Choose a new color", SetColor, ColorFinished, true);
    }

    private void SetColor(Color currentColor, float rad) {
        drawingMaterial.color = currentColor;
        penWidth = rad;
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
