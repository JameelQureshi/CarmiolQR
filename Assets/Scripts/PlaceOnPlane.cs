using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceOnPlane : MonoBehaviour
    {
        public GameObject reffernceObject;
        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        public GameObject m_PlacedPrefab;
        public UnityEvent onContentPlaced;
        public float scal;
        public float var = 0.001f;

        public Text debugLog;
        public GameObject ErrorImage;
        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
        }

        
        public void loadScene(int number)
        {
            
            if (number == 0)
            {
                Destroy(spawnedObject);
                SceneManager.LoadScene(number);
                Destroy(this.gameObject);
            }
            SceneManager.LoadScene(number);
        }
        bool TryGetTouchPosition(out Vector2 touchPosition)
        {
    #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                var mousePosition = Input.mousePosition;
                touchPosition = new Vector2(mousePosition.x, mousePosition.y);
                return true;
            }
    #else
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }
    #endif

            touchPosition = default;
            return false;
        }
        void Update()
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;

            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = s_Hits[0].pose;
                m_PlacedPrefab.SetActive(false);
                if (spawnedObject == null)
                {
                    try
                    {
                        //float size = 0.3f;
                        m_PlacedPrefab.SetActive(true);
                        //m_PlacedPrefab.AddComponent<Renderer>();
                        //Vector3 refSize = reffernceObject.GetComponent<Renderer>().bounds.size;
                        //float resizeX = refSize.x / m_PlacedPrefab.GetComponent<Renderer>().bounds.size.x;
                        //float resizeY = refSize.y / m_PlacedPrefab.GetComponent<Renderer>().bounds.size.y;
                        //float resizeZ = refSize.z / m_PlacedPrefab.GetComponent<Renderer>().bounds.size.z;
                        //resizeX *= m_PlacedPrefab.transform.localScale.x;
                        //resizeY *= m_PlacedPrefab.transform.localScale.y;
                        //resizeZ *= m_PlacedPrefab.transform.localScale.z;
                        //m_PlacedPrefab.GetComponent<Transform>().localScale = new Vector3(resizeX, resizeY, resizeZ);

                        m_PlacedPrefab.AddComponent<leantouch.Touch.LeanPinchScale>();

                        //scal = ((Camera.main.transform.position - transform.position).magnitude) * var;
                        //cube.transform.localScale = new Vector3(scal, scal, scal);

                        spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                        
                        spawnedObject.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                        StartCoroutine(GetTextureCoroutine(spawnedObject));
                        onContentPlaced.Invoke();
                        Handheld.Vibrate();
                        m_PlacedPrefab.SetActive(false);
                        gameObject.GetComponent<ARPlaneManager>().enabled = false;
                    }
                    catch (Exception e)
                    {
                        debugLog.text = "" + e;
                    }
                   
                }
            }
        }
        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;



        IEnumerator GetTextureCoroutine(GameObject model)
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
                int count = model.transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    model.transform.GetChild(i).GetComponent<Renderer>().material.SetTexture("_MainTex", ((DownloadHandlerTexture)www.downloadHandler).texture);
                }
               
            }
        }
    }
}