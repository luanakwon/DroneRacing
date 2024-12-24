using UnityEngine;

public class MultiCamHandler : MonoBehaviour
{
    public GameObject[] Cameras;
    public KeyCode[] keys;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0;i<keys.Length;i++)
        {
            if (Input.GetKey(keys[i])){
                // disable the rest
                for (int j=0;j<Cameras.Length;j++){
                    Cameras[j].SetActive(false);
                }
                // enable the camera
                Cameras[i].SetActive(true);
                break;
            }
        }
    }
}
