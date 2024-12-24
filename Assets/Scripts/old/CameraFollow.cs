using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum CameraMode {fpv, tpv, lookAt};
    public Transform target;
    public CameraMode mode;
    private Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == CameraMode.lookAt)
        {
            // transform.position *= 0;
            transform.LookAt(target);
            float distance = (target.position - transform.position).magnitude;
            cam.fieldOfView = 800/(distance+30);
            cam.nearClipPlane = Mathf.Max(0.3f,distance*0.9f);
        }
    }
}


