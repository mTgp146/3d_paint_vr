using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VoxelArt : MonoBehaviour
{
    [Header("Pen Properties")]
    public Transform pos;
    public Material drawingMaterial;
    public Material tipMaterial;
    public Transform ld;
    public Transform ru;


    [Range(0.01f, 01f)]
    private float cubeSize;

    [Header("XR Interaction")]
    private XRGrabInteractable grabInteractable;
    private bool leftController = false;
    private bool rightController = false;

    private GameObject newCube;
    private GameObject shownCube;
    private bool start = false; 
    private bool activated = false;
    private bool selected = false;
    private bool colorChanged = false;
    private bool colorChanging = false;
    private int xSpace;
    private int zSpace;


    void Start() {
        tipMaterial.color = drawingMaterial.color;
        cubeSize = 0.05f;

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
            shownCube = null;
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
            if(pos.position.x > ld.position.x && pos.position.x < ru.position.x && pos.position.z > ld.position.y && pos.position.z < ru.position.z && pos.position.y > ld.position.y) {

                Material newMaterial = new Material(drawingMaterial);
                newMaterial.color = drawingMaterial.color;

                newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newCube.transform.position = new Vector3(getX(), pos.position.y, getZ());
                //newCube.transform.rotation = pos.rotation;
                newCube.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                newCube.GetComponent<Renderer>().material = newMaterial;
                
                shownCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shownCube.transform.position = new Vector3(-5.5f+(float)xSpace, (pos.position.y-ld.position.y)*10, -5.5f+(float)zSpace);
                //shownCube.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                shownCube.GetComponent<Renderer>().material = newMaterial;
                start = true;

            }
        } 
    }

    private float getX() {
        float xDifference = pos.position.x - ld.position.x;
        float xSpaceDiv10 = (ru.position.x - ld.position.x)/10;
        float x;
        if (xDifference < xSpaceDiv10) {
            x = ld.position.x;
            xSpace = 1;
        } else if (xDifference < xSpaceDiv10*2) {
            x = ld.position.x + xSpaceDiv10;
            xSpace = 2;
        } else if (xDifference < xSpaceDiv10*3) {
            x = ld.position.x + xSpaceDiv10*2;
            xSpace = 3;
        } else if (xDifference < xSpaceDiv10*4) {
            x = ld.position.x + xSpaceDiv10*3;
            xSpace = 4;
        } else if (xDifference < xSpaceDiv10*5) {
            x = ld.position.x + xSpaceDiv10*4;
            xSpace = 5;
        } else if (xDifference < xSpaceDiv10*6) {
            x = ld.position.x + xSpaceDiv10*5;
            xSpace = 6;
        } else if (xDifference < xSpaceDiv10*7) {
            x = ld.position.x + xSpaceDiv10*6;
            xSpace = 7;
        } else if (xDifference < xSpaceDiv10*8) {
            x = ld.position.x + xSpaceDiv10*7;
            xSpace = 8;
        } else if (xDifference < xSpaceDiv10*9) {
            x = ld.position.x + xSpaceDiv10*8;
            xSpace = 9;
        } else {
            x = ld.position.x + xSpaceDiv10*9;
            xSpace = 10;
        }
        return x + xSpaceDiv10/2;
    }

    private float getZ() {
        float zDifference = pos.position.z - ld.position.z;
        float zSpaceDiv10 = (ru.position.z - ld.position.z)/10;
        float z;
        if (zDifference < zSpaceDiv10) {
            z = ld.position.z;
            zSpace = 1;
        } else if (zDifference < zSpaceDiv10*2) {
            z = ld.position.z + zSpaceDiv10;
            zSpace = 2;
        } else if (zDifference < zSpaceDiv10*3) {
            z = ld.position.z + zSpaceDiv10*2;
            zSpace = 3;
        } else if (zDifference < zSpaceDiv10*4) {
            z = ld.position.z + zSpaceDiv10*3;
            zSpace = 4;
        } else if (zDifference < zSpaceDiv10*5) {
            z = ld.position.z + zSpaceDiv10*4;
            zSpace = 5;
        } else if (zDifference < zSpaceDiv10*6) {
            z = ld.position.z + zSpaceDiv10*5;
            zSpace = 6;
        } else if (zDifference < zSpaceDiv10*7) {
            z = ld.position.z + zSpaceDiv10*6;
            zSpace = 7;
        } else if (zDifference < zSpaceDiv10*8) {
            z = ld.position.z + zSpaceDiv10*7;
            zSpace = 8;
        } else if (zDifference < zSpaceDiv10*9) {
            z = ld.position.z + zSpaceDiv10*8;
            zSpace = 9;
        } else {
            z = ld.position.z + zSpaceDiv10*9;
            zSpace = 10;
        }
        return z + zSpaceDiv10/2;
    }

    void SwitchColor() {
        Deactivate();
        colorChanging = true;
        start = false;
        ColorPicker.Create(cubeSize, drawingMaterial.color, "Choose a new color", SetColor, ColorFinished, true);
    }

    private void SetColor(Color currentColor, float rad) {
        drawingMaterial.color = currentColor;
        tipMaterial.color = drawingMaterial.color;
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
