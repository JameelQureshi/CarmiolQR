using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FileDownload : MonoBehaviour
{
    public Text progress;

    public void Download()
    {
        StartCoroutine(GetText());
        Debug.Log("Download Started!");
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = new UnityWebRequest("https://carmiolindustrial.com/store_front/ar_assets/objects/103/103.zip");
        www.downloadHandler = new DownloadHandlerBuffer();
        StartCoroutine(WaitForResponse(www));
        yield return www.SendWebRequest();
        progress.text = "" + www.downloadProgress;
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            string savePath = string.Format("{0}/{1}.zip", Application.persistentDataPath,"103");
            Debug.Log(Application.persistentDataPath);
            System.IO.File.WriteAllBytes(savePath, results);
        }
    }

    IEnumerator WaitForResponse(UnityWebRequest request)
    {
        while (!request.isDone)
        {
            progress.text = "" + request.downloadProgress;

            yield return null;
        }
    }
}
