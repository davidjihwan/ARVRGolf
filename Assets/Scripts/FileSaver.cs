using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileSaver : MonoBehaviour
{
    public void SaveFile(){
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("prefabParent");

        GameObject refImage = null;
        foreach (GameObject gameObject in gameObjects){
            Debug.Log(gameObject.name);
            if (gameObject.name.Contains("RefImageParent")){
                refImage = gameObject;
                Debug.Log("RefImageParent found");
            }
        }
        Vector3 refImagePos = Vector3.zero;
        if (refImage == null){
            Debug.Log("RefImageParent not found");
        } else {
            refImagePos = refImage.transform.position;
        }
        string path = Application.persistentDataPath + "/map.txt";
        //var file = System.IO.File.Create(path);
        // System.IO.StreamWriter sw = new System.IO.StreamWriter(file);

        // iOS: Points to /var/mobile/Containers/Data/Application/<guid>/Documents
        string entry = "";
        foreach (GameObject gameObject in gameObjects){
            entry += gameObject.name + " ";
            float posX = 0, posY = 0, posZ = 0;
            if (gameObject.name == "RefImageParent"){
                continue;
                // posX = 0;
                // posY = 0;
                // posZ = 0;
            } else {
                posX = gameObject.transform.position.x - refImagePos.x;
                posY = gameObject.transform.position.y - refImagePos.y;
                posZ = gameObject.transform.position.z - refImagePos.z;
            }
            entry += posX.ToString() + " " + posY.ToString() + " " + posZ.ToString() + "\n";
            // Debug.Log("Adding line to file: " + entry);
            
            // sw.WriteLine(entry);
            // sw.Flush();
            // file.Seek(0, System.IO.SeekOrigin.Begin);
        }
        System.IO.File.WriteAllText(path, entry);
        
        //Print out the string
        string line = "";
        using (System.IO.StreamReader sr = new System.IO.StreamReader(path)){
            while ((line = sr.ReadLine()) != null){
                Debug.Log(line);
                Debug.Log("Reading line");
            }
        }
    }
}