using UnityEngine;

public class UI_ThnY : MonoBehaviour
{
    public GameObject dot;
    public FlightCon droneScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float r, p, t, y;
        droneScript.GetSignal(out r,out p,out t,out y);
        // scale to interface
        t = (t - 1000) / 10;
        y = Mathf.Sin((y-1500)/1000*Mathf.PI)*50;
        // show interface
        dot.transform.localPosition = new Vector3(y, t, 0);
    }
}
