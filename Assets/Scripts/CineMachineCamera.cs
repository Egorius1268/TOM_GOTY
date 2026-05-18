using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Cinemachine;


public class CineMachineCamera : MonoBehaviour
{
    public CinemachineCamera vCam;
    private Vector2 touchStartScreen; 
    public float maxZoom;
    public float minZoom;
    public float sensivity;

    private Camera mainCam;
    
    void Start()
    {
        mainCam = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        if (vCam == null || mainCam == null) return;
        
        if (Input.GetMouseButtonDown(1))
        {
            touchStartScreen = Input.mousePosition;
        }
       
        else if (Input.GetMouseButton(1))  
        {
           Vector2 currentMouseScreen = Input.mousePosition;
           Vector2 screenDelta = touchStartScreen - currentMouseScreen;
           
           float screenToWorldFactor = (vCam.Lens.OrthographicSize * 2f) / Screen.height;
           Vector3 worldDirection = new Vector3(
               screenDelta.x * screenToWorldFactor, 
               screenDelta.y * screenToWorldFactor, 
               0
           );
            transform.position += worldDirection;
            
            touchStartScreen = currentMouseScreen;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Vector3 camPos = mainCam.transform.position;
            transform.position = new Vector3(camPos.x, camPos.y, transform.position.z);
        }
        if (Vector3.Distance(transform.position, mainCam.transform.position) > vCam.Lens.OrthographicSize)
        {
            transform.position = new Vector3(mainCam.transform.position.x, mainCam.transform.position.y, transform.position.z);
        }   
        ZoomCamera(Input.GetAxis("Mouse ScrollWheel"));
    }

    void ZoomCamera(float increment)
    {
        if (increment == 0) return;
        
        float targetZoom = vCam.Lens.OrthographicSize - increment * sensivity;
        
        vCam.Lens.OrthographicSize = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }
    
}