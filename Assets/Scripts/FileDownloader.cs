using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FileDownloader : MonoBehaviour
{
    public GameObject ErrorImage;
    public Text progress;
    public Text url;


    private void Awake()
    {
        ErrorImage.SetActive(false);
        url.text = PlayerPrefs.GetString("Model_url");
        DeletePreviousData();
        StartCoroutine(GetText());
    }

    private void DeletePreviousData()
    {
        string path = Application.persistentDataPath;

        DirectoryInfo directory = new DirectoryInfo(path);

        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo dir in directory.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    public void Download()
    {
        StartCoroutine(GetText());
        Debug.Log("Download Started!");
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = new UnityWebRequest(PlayerPrefs.GetString("Model_url"));
        //UnityWebRequest www = new UnityWebRequest("https://carmiolindustrial.com/store_front/ar_assets/objects/103/103.zip");
        www.downloadHandler = new DownloadHandlerBuffer();
        StartCoroutine(WaitForResponse(www));
        yield return www.SendWebRequest();
        //Debug.Log($"Loading Model. Progress: {progress:P}");
        //progress.text = "" + www.downloadProgress;
        progress.text = $"Cargando Modelo...{www.downloadProgress:P}";
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            ErrorImage.SetActive(true);
            ErrorImage.transform.GetChild(0).GetComponent<Text>().text = "Network error!";
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            string savePath = string.Format("{0}/{1}.zip", Application.persistentDataPath, PlayerPrefs.GetString("code"));
            //string savePath = string.Format("{0}/{1}.zip", Application.persistentDataPath, "103");
            Debug.Log(Application.persistentDataPath);
            System.IO.File.WriteAllBytes(savePath, results);
            UnzipFile();
           
        }
    }

    IEnumerator WaitForResponse(UnityWebRequest request)
    {
        while (!request.isDone)
        {
            //progress.text = "" + request.downloadProgress;
            progress.text = $"Cargando Modelo...{request.downloadProgress:P}";

            yield return null;
        }
    }
    public void UnzipFile()
    {
        progress.text = "Unziping Model!";
        //string FilePath = string.Format("{0}/{1}.zip", Application.persistentDataPath, "103");
        string FilePath = string.Format("{0}/{1}.zip", Application.persistentDataPath, PlayerPrefs.GetString("code"));
        System.IO.Compression.ZipFile.ExtractToDirectory(FilePath, Application.persistentDataPath);
        progress.text = "Model Unzipped!";
        ModelImporter.instance.LoadModel();
    }
}
