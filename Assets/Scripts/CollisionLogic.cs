using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionLogic : MonoBehaviour
{
    public List<GameObject> cubes = new List<GameObject>();
    public bool isIntersected = false;
    public bool cubeIntersect = false;
    public int numCollisions = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider){
        if (!collider.gameObject.CompareTag("ground")){
            isIntersected = true;
            numCollisions += 1;
            if (collider.gameObject.transform.parent.gameObject.CompareTag("cubeParent")){
                cubes.Add(collider.gameObject);
                cubeIntersect = true;
                Debug.Log("cube intersect is true");
            }
            //Debug.Log("Collision detected");
        }
    }

    void OnTriggerExit(Collider collider){
        if (!collider.gameObject.CompareTag("ground")){
                numCollisions -= 1;
                if (collider.gameObject.transform.parent.gameObject.CompareTag("cubeParent")){
                    cubes.Remove(collider.gameObject);
                    if (cubes.Count == 0){
                        cubeIntersect = false;
                        Debug.Log("cube intersect is false");
                    }
                }
            if (numCollisions == 0){
                isIntersected = false;
                //Debug.Log("Exited all collisions.");
            } else if (numCollisions < 0){
                //Debug.Log("Number of collisions is less than zero. This should never happen.");
            }
        }
    }

    public void Stack(){
        // GameObject baseCube = FindCubeToStackOn();
        GameObject baseCube = cubes[0];
        transform.parent.gameObject.transform.position = baseCube.transform.parent.gameObject.transform.position;
        transform.parent.gameObject.transform.rotation = baseCube.transform.parent.gameObject.transform.rotation;
        Vector3 colliderPosition = transform.parent.gameObject.transform.position;
        colliderPosition += new Vector3(0f, 0.05f, 0f);
        while (Physics.OverlapBox(colliderPosition, transform.localScale/2).Length != 0){
            colliderPosition += new Vector3(0f, 0.1f, 0f);
        }
        transform.parent.gameObject.transform.position = colliderPosition - new Vector3(0f, 0.05f, 0f);
        isIntersected = false;
        cubeIntersect = false;
        cubes.Clear();

        //Check if base cube is still intersected
        if (Physics.OverlapBox(baseCube.transform.position, baseCube.transform.localScale/3).Length < 3){
            baseCube.GetComponent<CollisionLogic>().isIntersected = false;
            baseCube.GetComponent<CollisionLogic>().cubeIntersect = false;
            // baseCube.GetComponent<CollisionLogic>().cubes.Clear();
        }
        Debug.Log("base cube intersects: " + Physics.OverlapBox(baseCube.transform.position, baseCube.transform.localScale/3).Length.ToString());

    }

    public bool IsMoveable(){
        if (!transform.gameObject.name.Contains("Cube")){
            return true;
        } else {
            Vector3 checkPosition = transform.position + new Vector3(0f,0.1f,0f);
            //Debug.Log(Physics.OverlapBox(checkPosition, transform.localScale/3).Length.ToString());
            if (Physics.OverlapBox(checkPosition, transform.localScale/3).Length == 0){
                return true;
            } else {
                return false;
            }
        }
    }

    public bool IsIntersected(){
        // if (!transform.parent.gameObject.CompareTag("cubeParent")){
        //     return (numCollisions > 0);
        // } else {
            //bounding box
        return (Physics.OverlapBox(transform.position, transform.localScale/2).Length > 2);
        // }
    }

    public bool CubeIntersected(GameObject currSelected){
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale/2);
        foreach (Collider collider in colliders){
            if (currSelected.GetInstanceID() != collider.gameObject.GetInstanceID() && 
            collider.gameObject.name.Contains("Cube")){
                cubes.Add(collider.gameObject);
                return true;
            }
        }
        return false;
    }

    // GameObject FindCubeToStackOn(){
    //     foreach (GameObject gameObject in cubes){
    //         if (gameObject.transform.position.y <= 0.1){
    //             return gameObject;
    //         }
    //     }
    // }

}

