using UnityEngine;

public class XGB_interface
{
    private float[,] signals;
    private int t_max;
    private MyXGB model_th = null;
    private MyXGB model_y = null;
    public XGB_interface(int t_window, string throttle_model_json=null, string yaw_model_json=null){
        t_max = t_window;
        signals = new float[1,t_window*4+2];
        for (int t=0;t<t_window;t++){
            signals[0,t] = 1500; // roll
            signals[0,t+5] = 1500; // pitch
            signals[0,t+10] = 1000; // throttle
            signals[0,t+15] = 1500; // yaw
        }
        signals[0,t_window*4] = 1500; // attitude roll
        signals[0,t_window*4+1] = 1500; // attitude pitch
        // init model
        if (throttle_model_json != null){
            string jspath = System.IO.Path.Combine(Application.streamingAssetsPath,"XGB_models",throttle_model_json);
            string jstr = System.IO.File.ReadAllText(jspath);
            model_th = JsonUtility.FromJson<MyXGB>(jstr);
        }
        if (yaw_model_json != null){
            string jspath = System.IO.Path.Combine(Application.streamingAssetsPath,"XGB_models",yaw_model_json);
            string jstr = System.IO.File.ReadAllText(jspath);
            model_y = JsonUtility.FromJson<MyXGB>(jstr);
        }        
    }

    public bool[] GetSupportedAxes(){
        return new bool[] {false, false, model_y != null, model_th != null};
    }

    public void ConvertSignal(float[] sig, ControlMode[] mode)
    // sig : attitude_r/attitude_p/r/p/t/y
    {
        // push old signals
        for (int c=0;c<4;c++){
            for (int t=3;t>=0;t--){
                signals[0,t_max*c+t+1] = signals[0,t_max*c+t];
            }
        }
        // store current signals
        for (int c=0;c<4;c++){  // main (rpty)
            signals[0,c*t_max] = sig[c];
        }
        for (int c=0;c<2;c++){ // auxilary (attitude r/p)
            signals[0,4*t_max+c] = sig[4+c];
        }
        // predict each axis
        // predict thrust
        if (mode[2] == ControlMode.assist){
            // thrust model .forward
            float[] pred_th = new float[1];
            model_th.Predict(signals, pred_th);
            sig[2] = pred_th[0];
        } else if (mode[2] == ControlMode.semi_assist){
            // thrust forward only if pressed
            if (sig[2] != 1000){
                float[] pred_th = new float[1];
                model_th.Predict(signals, pred_th);
                sig[2] = pred_th[0];
            }
        }
        // predict yaw
        if (mode[3] == ControlMode.assist){
            // yaw model .foward
            float[] pred_y = new float[1];
            model_y.Predict(signals,pred_y);
            sig[3] = pred_y[0];
        } else if (mode[3] == ControlMode.semi_assist){
            // yaw model .foward only if aligns with the input
            if (sig[3] > 1500){
                float[] pred_y = new float[1];
                model_y.Predict(signals,pred_y);
                sig[3] = pred_y[0] > 1500 ? pred_y[0] : 1501;
            } else if (sig[3] < 1500){
                float[] pred_y = new float[1];
                model_y.Predict(signals,pred_y);
                sig[3] = pred_y[0] < 1500 ? pred_y[0] : 1499;
            }
        }
    }
}
