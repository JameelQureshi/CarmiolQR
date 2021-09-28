using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation.Samples;

public class ModelImporter : MonoBehaviour
{
    public static ModelImporter instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public Text progressText;
    public GameObject AR_Prefab_GameObject;
    public GameObject reffernceObject;
    public GameObject LoadingPanel;
    public GameObject ErrorImage;
    public GameObject MyObject;

    GameObject loadedObject;

    public Text progress;


    
    
    private void Start()
    {
        
        LoadingPanel.SetActive(true);
        ErrorImage.SetActive(false);
    }

    public void LoadModel()
    {
        progress.text = "Loading Model!";
        string objPath = string.Format("{0}/{1}.obj", Application.persistentDataPath, PlayerPrefs.GetString("code"));
        FileStream fileStream = null;

        try
        {
            // fileStream = File.Open(objPath, FileMode.Open);
            using (fileStream = File.Open(objPath, FileMode.OpenOrCreate))
            {
                loadedObject = new OBJLoader().Load(fileStream);
                
                progress.text = "Model Loaded!";
                LoadModelInAR();
            }

        }
        catch (Exception e)
        {
            progress.text = "" + e;
            return;
        }

    }








    IEnumerator GetTextureCoRoutine()
    {
       
        //UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://carmiolindustrial.com/store_front/ar_assets/objects/101/101.png");

         UnityWebRequest www = UnityWebRequestTexture.GetTexture(PlayerPrefs.GetString("Texture_url"));
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            ErrorImage.SetActive(true);
            ErrorImage.transform.GetChild(0).GetComponent<Text>().text = "Texture not found";
        }
        else
        {
            ErrorImage.SetActive(false);
            int count = MyObject.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                MyObject.transform.GetChild(i).GetComponent<Renderer>().material.SetTexture("_MainTex", ((DownloadHandlerTexture)www.downloadHandler).texture);
            }
            LoadingPanel.SetActive(false);
            LoadModelInAR();
        }
    }
    public void LoadModelInAR()
    {
        /*Vector3 refSize = reffernceObject.GetComponent<Renderer>().bounds.size;
        float resizeX = refSize.x / MyObject.GetComponent<Renderer>().bounds.size.x;
        float resizeY = refSize.y / MyObject.GetComponent<Renderer>().bounds.size.y;
        float resizeZ = refSize.z / MyObject.GetComponent<Renderer>().bounds.size.z;
        resizeX *= MyObject.transform.localScale.x;
        resizeY *= MyObject.transform.localScale.y;
        resizeZ *= MyObject.transform.localScale.z;
        MyObject.GetComponent<Transform>().localScale = new Vector3(resizeX, resizeY, resizeZ);*/

        LoadingPanel.SetActive(false);

        AR_Prefab_GameObject.SetActive(true);
        AR_Prefab_GameObject.transform.GetComponent<PlaceOnPlane>().enabled = true;
        AR_Prefab_GameObject.transform.GetComponent<PlaceOnPlane>().m_PlacedPrefab = loadedObject;
    }
}
