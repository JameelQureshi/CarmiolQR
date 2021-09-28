using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TBEasyWebCam;
using System.Text.RegularExpressions;

public class QRDecodeTest : MonoBehaviour
{
	public QRCodeDecodeController e_qrController;
	public Text UiText;
	public static string url_to_pass;
	public GameObject resetBtn;
	public GameObject scanLineObj;
	#if (UNITY_ANDROID||UNITY_IOS) && !UNITY_EDITOR
	bool isTorchOn = false;
	#endif
	public Sprite torchOnSprite;
	public Sprite torchOffSprite;
	public Image torchImage;
	/// <summary>
	/// when you set the var is true,if the result of the decode is web url,it will open with browser.
	/// </summary>
	public bool isOpenBrowserIfUrl;
    private void Start()
	{
		PlayerPrefs.SetString("code", "");
		PlayerPrefs.SetString("Model_url", "");
		PlayerPrefs.SetString("Texture_url", "");
		if (this.e_qrController != null)
		{
			this.e_qrController.onQRScanFinished += new QRCodeDecodeController.QRScanFinished(this.qrScanFinished);
		}
	}

	private void Update()
	{
	}
	private void qrScanFinished(string dataText)
	{
		if (isOpenBrowserIfUrl) {
			if (Utility.CheckIsUrlFormat(dataText))
			{
				if (!dataText.Contains("http://") && !dataText.Contains("https://"))
				{
					dataText = "http://" + dataText;
				}
				Application.OpenURL(dataText);
			}
		}
		//this.UiText.text = dataText;
		//url_to_pass = dataText;
		cutString(dataText);
		SceneManager.LoadScene(1);
		/*if (this.resetBtn != null)
		{
			this.resetBtn.SetActive(true);
		}
		if (this.scanLineObj != null)
		{
			this.scanLineObj.SetActive(false);
		}*/

	}

	public void Reset()
	{
		if (this.e_qrController != null)
		{
			this.e_qrController.Reset();
		}
		if (this.UiText != null)
		{
			this.UiText.text = string.Empty;
		}
		if (this.resetBtn != null)
		{
			this.resetBtn.SetActive(false);
		}
		if (this.scanLineObj != null)
		{
			this.scanLineObj.SetActive(true);
		}
	}

	public void Play()
	{
		Reset ();
		if (this.e_qrController != null)
		{
			this.e_qrController.StartWork();
		}
	}

	public void Stop()
	{
		if (this.e_qrController != null)
		{
			this.e_qrController.StopWork();
		}

		if (this.resetBtn != null)
		{
			this.resetBtn.SetActive(false);
		}
		if (this.scanLineObj != null)
		{
			this.scanLineObj.SetActive(false);
		}
	}

	public void GotoNextScene(string scenename)
	{
		if (this.e_qrController != null)
		{
			this.e_qrController.StopWork();
		}
		//Application.LoadLevel(scenename);
		SceneManager.LoadScene(scenename);
	}

	/// <summary>
	/// Toggles the torch by click the ui button
	/// note: support the feature by using the EasyWebCam Component 
	/// </summary>
	public void toggleTorch()
	{
		#if (UNITY_ANDROID||UNITY_IOS) && !UNITY_EDITOR
		if (EasyWebCam.isActive) {
			if (isTorchOn) {
				torchImage.sprite = torchOffSprite;
				EasyWebCam.setTorchMode (TBEasyWebCam.Setting.TorchMode.Off);
			} else {
				torchImage.sprite = torchOnSprite;
				EasyWebCam.setTorchMode (TBEasyWebCam.Setting.TorchMode.On);
			}
			isTorchOn = !isTorchOn;
		}
		#endif
	}
	public void cutString(string url)
	{
		Debug.Log("QR url = " + url);
		string[] x = url.Split('&');
		string my_string = x[1];
		Debug.Log(x.Length);
		GetCode(my_string);
	}
	public void GetCode(string input)
	{
		string pattern = "[a-z,:/?.=_&|!@#$%^*()-]+";
		string[] result = Regex.Split(input, pattern,
									  RegexOptions.IgnoreCase,
									  TimeSpan.FromMilliseconds(500));
		string code = result[1];
		PlayerPrefs.SetString("code", code);
		PlayerPrefs.SetString("Model_url", "https://carmiolindustrial.com/store_front/ar_assets/objects/" + code + "/" + code + ".zip");
		PlayerPrefs.SetString("Texture_url", "https://carmiolindustrial.com/store_front/ar_assets/objects/" + code + "/" + code + ".png");
		Debug.Log("final url " + PlayerPrefs.GetString("url"));
	}




}
