using UnityEngine;

public class XGB_controller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    MyXGB model;
    private Vector2[] mouseAxis;
    private Vector2[] keyboardAxis;
    private int tok;

    void Start()
    {
        // string jspath = System.IO.Path.Combine(Application.dataPath,"XGB_models","model_reduced.json");
        // string jstr = System.IO.File.ReadAllText(jspath);
        // model = JsonUtility.FromJson<MyXGB>(jstr);

        mouseAxis = new Vector2[5];
        for( int i=0;i<5;i++){mouseAxis[i] = Vector2.one*1500;}
        keyboardAxis = new Vector2[5];
        for( int i=0;i<5;i++){keyboardAxis[i] = Vector2.zero;}
        tok = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ReadInput();
        float[] inputs = new float[20];
        for (int t=0;t<5;t++){
            inputs[t] = Mathf.Clamp(mouseAxis[(tok+5-t)%5].x,1000,2000);
            inputs[5+t] = Mathf.Clamp(mouseAxis[(tok+5-t)%5].y,1000,2000);
            inputs[10+t] = Mathf.Clamp01(keyboardAxis[(tok+5-t)%5].y)*1000+1000;
            inputs[15+t] = keyboardAxis[(tok+5-t)%5].x*500+1500;
        }
        // string ips = "";
        // for (int i=0;i<20;i++){ips += inputs[i] + " ";}
        // Debug.Log(ips);
        tok = (tok+1)%5;
    }

    private void ReadInput(){
        mouseAxis[tok].x = mouseAxis[(tok+4)%5].x + Input.GetAxis("Mouse X")*20;
        mouseAxis[tok].y = mouseAxis[(tok+4)%5].y + Input.GetAxis("Mouse Y")*20;
        keyboardAxis[tok].x = Input.GetAxisRaw("Horizontal");
        keyboardAxis[tok].y = Input.GetAxisRaw("Vertical");
    }
}
