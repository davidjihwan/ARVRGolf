using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.EnhancedTouch;
// using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class LogicController : MonoBehaviour
{
    public GameObject cubePrefab, spherePrefab, rectanglePrefab, cylinderPrefab;
    public GameObject ballPrefab, holePrefab, refImagePrefab;
    private GameObject ballPrefabInstance, holePrefabInstance, refImagePrefabInstance;
    public enum inputTypes {spawned, deleted, moved, rotated};
    inputTypes inputType;
    public Material blueHighlight;
    public Material redHighlight;
    public Material unhighlight;
    public Material refTexture;
    private bool noCollisions = true;
    // private Vector2 lastHitPosition;
    public Camera ARCamera;
    public GameObject cantSaveMessage;
    public GameObject cantSaveButton;

    public struct userInput {
        public GameObject gameObject;
        public inputTypes inputType;
        public Vector3 position;
        public Quaternion rotation;
    }

    private GameObject currSelected = null;
    public List<userInput> userInputs = new List<userInput>();
    private int inputIndex = -1;

    private ARRaycastManager raycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // void OnEnable(){
    //     EnhancedTouchSupport.Enable();
    // }

    // Start is called before the first frame update
    
    void Start()
    {   
        raycastManager = GetComponent<ARRaycastManager>();
        cantSaveMessage.SetActive(false);
        cantSaveButton.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        // Selecting object happens only on the first frame of touch
        // if (Input.touchCount == 1){
        if (Input.touchCount == 1){
            Touch touch = Input.GetTouch(0);
//here
            if (touch.phase == UnityEngine.TouchPhase.Began){
                Ray hitRay = ARCamera.ScreenPointToRay(touch.position);
                // lastHitPosition = touch.screenPosition;
                RaycastHit hit;
                Physics.Raycast(hitRay, out hit);
                GameObject hitObject = null;
                if (hit.collider){
                    hitObject = hit.collider.gameObject;
                }

                if (hit.collider && hitObject.CompareTag("prefab") && hitObject.GetComponent<CollisionLogic>().IsMoveable()){
                    // a prefab has been hit
                    // Only change something if the user selects an object that is not already selected.
                    if (!currSelected){
                        currSelected = hitObject;
                        currSelected.GetComponent<MeshRenderer>().material = blueHighlight;
                    } else if (currSelected.GetInstanceID() != hitObject.GetInstanceID()){
                        if (!currSelected.GetComponent<CollisionLogic>().IsIntersected()){
                            if (!(currSelected.name == "RefImage")){
                                currSelected.GetComponent<MeshRenderer>().material = unhighlight;
                            } else {
                                currSelected.GetComponent<MeshRenderer>().material = refTexture;
                            }
                        } 
                        currSelected = hitObject;
                        currSelected.GetComponent<MeshRenderer>().material = blueHighlight;
                    } else {
                        currSelected.GetComponent<MeshRenderer>().material = blueHighlight;
                    }

                    // hitObject.GetComponent<CollisionLogic>().cubes.Clear();
                    //currSelected.transform.parent.gameObject.transform.position -= new Vector3(0f,currSelected.transform.parent.gameObject.transform.position.y,0f);
                } 
                // else if (.CompareTag("UI")){
                //     continue;
                // } 
                else {
                    // Debug.Log("A prefab has not been selected");
                    if (currSelected){
                        // unhighlight objects.
                        if (!currSelected.GetComponent<CollisionLogic>().IsIntersected()){
                            if (!(currSelected.name.Contains("RefImage"))){
                                currSelected.GetComponent<MeshRenderer>().material = unhighlight;
                            } else {
                                currSelected.GetComponent<MeshRenderer>().material = refTexture;
                            }
                        }
                        currSelected = null;
                    }
                }

                // if (currSelected){
                //     Debug.Log("Current selected: " + currSelected.name);
                // } else {
                //     Debug.Log("Current selected: None");
                // }

            }
            //^Selection done

            //Moving and drag
            if (touch.phase == UnityEngine.TouchPhase.Moved && currSelected){
                //AR implementation
                Vector2 touchPos = touch.position;
                if (raycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon)){
                    Vector3 posePosition = hits[0].pose.position;
                    currSelected.transform.parent.gameObject.transform.position = posePosition;
                }
            }

            if (touch.phase == UnityEngine.TouchPhase.Ended){

                if (currSelected){

                    inputType = inputTypes.moved;

                    if (currSelected.gameObject.name.Contains("Cube") && 
                        currSelected.GetComponent<CollisionLogic>().CubeIntersected(currSelected)){
                        //Stack
                        currSelected.GetComponent<CollisionLogic>().Stack();
                        Debug.Log("Stacking");
                    }

                    addUserInput(inputType, currSelected);
                    // foreach (userInput item in userInputs){
                    //     printStruct(item);
                    // }
                }

                HighlightChecker();

                // Debug.Log("length of user inputs: " + userInputs.Count.ToString());
                // Debug.Log("Input idx: " + inputIndex.ToString());
                
                //Handle the array of user inputs    
                //TODO:      
                currSelected = null;       
            }
        }
    }

    void addUserInput(inputTypes inputType, GameObject inputObject){
        if (inputIndex == 19){
            userInputs.RemoveAt(0);
            userInputs.Add(getCurrInput(inputType, inputObject));
        } else {
            userInputs.Add(getCurrInput(inputType, inputObject));
            inputIndex += 1;
        }
    }

    // void shiftInputsLeft(List<userInput> inputs){
    //     // for (int i = 0; i < (inputs.length - 1); i++){
    //     //     inputs[i] = inputs[i+1];
    //     // }
    //     for (int i = 0; i < (inputs.Count - 1); i++){
    //         inputs[i] = inputs[i+1];
    //     }
    // }

    userInput getCurrInput(inputTypes type, GameObject inputObject){
        //TODO:
        userInput returnStruct;
        // struct userInput {
        //     public GameObject gameObject;
        //     public inputTypes inputType;
        //     public Vector3 position;
        //     public Vector3 rotation;
        // }
        returnStruct.gameObject = inputObject;
        returnStruct.inputType = type;
        returnStruct.position = inputObject.transform.parent.gameObject.transform.position;
        returnStruct.rotation = inputObject.transform.parent.gameObject.transform.rotation;
        return returnStruct;
    }

    void printStruct(userInput input){
        Debug.Log(input.gameObject.name);
        Debug.Log(input.inputType);
        Debug.Log(input.position.ToString());
        Debug.Log(input.rotation.ToString());
    }

    public void Undo(){
        // public struct userInput {
        //     public GameObject gameObject;
        //     public inputTypes inputType;
        //     public Vector3 position;
        //     public Quaternion rotation;
        //     public int numCollisions;
        // }

        //ex. 
        // public struct userInput {
        //     gameobject = cube;
        //     inputtype = spawned;
        //     position = 5,5,5;
        //     rotation = 0,0,0;
        //     numCollisions = 0;
        // }
        // public struct userInput {
        //     public GameObject gameObject;
        //     public inputTypes inputType;
        //     public Vector3 position;
        //     public Quaternion rotation;
        //     public int numCollisions;
        // }

        //if inputType is moved
        Debug.Log("undo clicked");
        if (inputIndex > 0){
            userInput currUserInput = userInputs[inputIndex];
            userInput lastUserInput = userInputs[inputIndex - 1];
            GameObject currObject = currUserInput.gameObject;
            if (currUserInput.inputType == inputTypes.moved){
                currObject.transform.parent.gameObject.transform.position = lastUserInput.position;
                Debug.Log("moved.");
            } else if (currUserInput.inputType == inputTypes.rotated){
                currObject.transform.parent.gameObject.transform.rotation = lastUserInput.rotation;
                Debug.Log("rotated.");
            } 
            inputIndex -= 1;
        }
    }

    public void Redo(){
        if (inputIndex != 19 && inputIndex < userInputs.Count){
            userInput currUserInput = userInputs[inputIndex];
            userInput nextUserInput = userInputs[inputIndex + 1];
            GameObject currObject = currUserInput.gameObject;
            if (nextUserInput.inputType == inputTypes.moved){
                currObject.transform.parent.gameObject.transform.position = nextUserInput.position;
                Debug.Log("moved.");
            } else if (nextUserInput.inputType == inputTypes.rotated){
                currObject.transform.parent.gameObject.transform.rotation = nextUserInput.rotation;
                Debug.Log("rotated.");
            } 
            inputIndex += 1;
        }    
        // int i = 0;
        // foreach (userInput item in userInputs){
        //     Debug.Log("Item " + i.ToString() + ": ");
        //     printStruct(item);
        //     i += 1;
        // }
    }

    public void Reset(){
        SceneManager.LoadScene("GameScene");
    }

    public void spawn(int itemNum){
        GameObject objectToBeSpawned;
        //0 = cube, 1 = sphere, 2 = cylinder, 3 = rectangle
        if (itemNum == 0){
            objectToBeSpawned = cubePrefab;
        } else if (itemNum == 1){
            objectToBeSpawned = spherePrefab;
        } else if (itemNum == 2){
            objectToBeSpawned = cylinderPrefab;
        } else {
            objectToBeSpawned = rectanglePrefab;
        }
        Vector3 spawnPos;
        if ((spawnPos = CameraForward()) != Vector3.zero){
            Instantiate(objectToBeSpawned, CameraForward(), Quaternion.identity);
            objectToBeSpawned.SetActive(true);
            userInput newStruct;
            newStruct.gameObject = objectToBeSpawned;
            newStruct.inputType =  inputTypes.spawned;
            newStruct.position = CameraForward();
            newStruct.rotation = Quaternion.identity;
            userInputs.Add(newStruct);
        }
    }

    public void spawnOnce(int itemNum){
        GameObject objectToBeSpawned = null;
        bool spawn = false;
        // 0 = ball, 1 = hole, 2 = refimage
        Vector3 spawnPos;
        if ((spawnPos = CameraForward()) != Vector3.zero){
            if (itemNum == 0){
                if (!ballPrefabInstance){
                    spawn = true;
                    ballPrefabInstance = Instantiate(ballPrefab, CameraForward(), Quaternion.identity);
                    objectToBeSpawned = ballPrefabInstance;
                }
            } else if (itemNum == 1){
                if (!holePrefabInstance){
                    spawn = true;
                    holePrefabInstance = Instantiate(holePrefab, CameraForward(), Quaternion.identity);
                    objectToBeSpawned = holePrefabInstance;
                }
            } else {
                if (!refImagePrefabInstance){
                    spawn = true;
                    refImagePrefabInstance = Instantiate(refImagePrefab, CameraForward(), Quaternion.identity);
                    objectToBeSpawned = refImagePrefabInstance;
                }
            }
        } else {
            spawn = false;
        }
        if (spawn){
            objectToBeSpawned.SetActive(true);
            userInput newStruct;
            newStruct.gameObject = objectToBeSpawned;
            newStruct.inputType =  inputTypes.spawned;
            newStruct.position = CameraForward();
            newStruct.rotation = Quaternion.identity;
            userInputs.Add(newStruct);
        }
    }

    public void HighlightChecker(){
        GameObject[] gameObjectArr = GameObject.FindGameObjectsWithTag("prefab");
        bool noCurrentCollisions = true;
        foreach (var i in gameObjectArr){
            if (i.GetComponent<CollisionLogic>().IsIntersected()){
                i.GetComponent<MeshRenderer>().material = redHighlight;
                noCurrentCollisions = false;
            } else if (i.name.Contains("RefImage")){
                i.GetComponent<MeshRenderer>().material = refTexture;
            } else if (currSelected && currSelected.GetInstanceID() != i.GetInstanceID()){
                i.GetComponent<MeshRenderer>().material = unhighlight;
            } else if (currSelected) {
                i.GetComponent<MeshRenderer>().material = unhighlight;
            }
        }
        noCollisions = noCurrentCollisions;
    }

    public Vector3 CameraForward(){
        Ray ray = new Ray(ARCamera.transform.position, ARCamera.transform.forward);
        if (raycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon)){
            return hits[0].pose.position;
        } else {
            return Vector3.zero;
        }
    }

    public bool IsSaveable(){
        if (noCollisions && ballPrefabInstance && holePrefabInstance && refImagePrefabInstance){
            return true;
        } else {
            return false;
        }
    }

    public void Save(){
        if (IsSaveable()){
            //save
            FileSaver fileSaver = this.gameObject.GetComponent<FileSaver>();
            fileSaver.SaveFile();
            // FileSaver fileSaver = new FileSaver();
            // fileSaver.SaveFile();
        } else {
            //Show message
            cantSaveMessage.SetActive(true);
            cantSaveButton.SetActive(true);
        }
    }

    public void CloseMessage(){
        cantSaveMessage.SetActive(false);
        cantSaveButton.SetActive(false);
    }

}
