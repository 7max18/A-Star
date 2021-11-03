using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Target : MonoBehaviour
{
    public Transform targetMarker;
    private UnityEvent mouseClicked;

    void Start ()
    {
        if (mouseClicked == null)
        {
            mouseClicked = new UnityEvent();
        }
    }

    void Update ()
    {
        int button = 0;

        //Get the point of the hit position when the mouse is being clicked
        if(Input.GetMouseButtonDown(button)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo)) 
            {
                Vector3 targetPosition = hitInfo.point;
                targetMarker.position = targetPosition;
            }

            EventManager.TriggerEvent("Mouse Clicked");
        }
    }

}