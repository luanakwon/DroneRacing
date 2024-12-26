using System;
using System.IO;
using UnityEditor.Analytics;
using UnityEngine;

public class FlightCon : MonoBehaviour
{
    public bool thrust_switch;
    public float pitchRate;
    public float yawRate;
    public float rollRate;
    
    public GameObject FPV_cam;
    public ControlMode throttle_xgb_mode;
    public ControlMode yaw_xgb_mode;
    // read only
    [HideInInspector] public Vector2 left_joystick {get; private set;}
    [HideInInspector] public Vector2 right_joystick {get; private set;}

    private Config flightConfig = new();
    private float[] signals = {1500,1500,1000,1500,1500,1500};
    // signals (roll pitch thrust yaw)
    private float[] signals0 = {1500,1500,1000,1500};
    // previous control signal (for async reading in UI only.)
    private Rigidbody rb;
    private Vector3 initPos;
    private Quaternion initRot;
    private float torque_coef = 0.002f;
    // torque coef is essentially the P gain of PID when applying torque.
    // in theory this is proportional to (moment of inertia)/(response time)
    // but in practice a heuristic value 0.002 is chosen for size 0.3, mass 0.4, about instant response 
    // this includes conversion from deg/s to rad/s

    private XGB_interface xgb_interface;
    private ControlMode[] mode;

    void Awake(){
        // read in flight config
        try{
            string jspath = System.IO.Path.Combine(
                Application.persistentDataPath,"FlightConfigs","config_current.json");
            string jstr = System.IO.File.ReadAllText(jspath);
            flightConfig.LoadFromJson(jstr);
        } catch (IOException) {
            Debug.LogWarning("No previous user configuration found.");
        }

        // set current flight mode
        mode = new ControlMode[4];
        for (int i=0;i<4;i++){
            if (flightConfig.mode[i] == '0'){
                mode[i] = ControlMode.raw;
            } else if (flightConfig.mode[i] == '1'){
                mode[i] = ControlMode.semi_assist;
            } else if (flightConfig.mode[i] == '2'){
                mode[i] = ControlMode.assist;
            }
        }

        xgb_interface = gameObject.GetComponent<XGB_interface>();
        xgb_interface.InitInterface(
            throttle_xgb_mode!=ControlMode.raw, yaw_xgb_mode != ControlMode.raw);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get rigidbody
        rb = gameObject.GetComponent<Rigidbody>();
        // Configure camera
        ConfigureCamera();
        // save initial flight state (attitude)
        transform.GetPositionAndRotation(out initPos, out initRot);
        // init joystick
        left_joystick = Vector2.zero;
        right_joystick = Vector2.zero;
        // init xgb if used
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // external (control) signal set from update in controller2
        // set internal signal (attitude)
        SetAttitude();
        xgb_interface?.ConvertSignal(signals, mode);

        ApplyPhysics();

        string ips = "";
        for (int i=0;i<4;i++){ips += signals[i] + " ";}
        // Debug.Log(ips);
    }
    public void GetSignal(out float roll, out float pitch, out float throttle, out float yaw){
        roll = signals0[0];
        pitch = signals0[1];
        throttle = signals0[2];
        yaw = signals0[3];
    }
    public void SetSignal(float roll, float pitch, float throttle, float yaw)
    {
        signals[0] = roll;
        signals[1] = pitch;
        signals[2] = throttle;
        signals[3] = yaw;
    }
    private void SetAttitude(){
        // divide gravity to each up, right, forward component
        float g_r = Vector3.Dot(Vector3.down,transform.right);
        float g_u = Vector3.Dot(Vector3.down,transform.up);
        float g_f = Vector3.Dot(Vector3.down,transform.forward);
        // attitude roll is calculated in up-right plane
        float cos, att_r, att_p;
        cos = (g_u == 0 && g_r == 0) ? 0 : -g_u/Mathf.Sqrt(g_u*g_u+g_r*g_r);
        att_r = Mathf.Acos(cos)*Mathf.Sign(g_r);
        // attitude pitch is calculated in up-forward plane
        cos = (g_u == 0 && g_f == 0) ? 0 : -g_u/Mathf.Sqrt(g_u*g_u+g_f*g_f);
        att_p = Mathf.Acos(cos)*Mathf.Sign(g_f);

        // Debug.Log(att_r/Mathf.PI*180 + ", " + att_p/Mathf.PI*180);

        signals[4] = att_r / Mathf.PI * 500 + 1500;
        signals[5] = att_p / MathF.PI * 500 + 1500;
    }
    public void SetConfig(string param, float value){
        // get param
        System.Reflection.PropertyInfo property = typeof(Config).GetProperty(param);
        // update config
        property?.SetValue(flightConfig, value);
    }
    public void SetConfig(string[] param, float[] value){
        for (int i=0;i<param.GetLength(0);i++){
            // get param
            System.Reflection.PropertyInfo property = typeof(Config).GetProperty(param[i]);
            // update config
            property?.SetValue(flightConfig, value[i]);
        }
    }
    public void SetConfig(System.Reflection.PropertyInfo property, float value){
        // Debug.Log(property + " " + value);
        property?.SetValue(flightConfig,value);
    }
    public void SaveConfig(){// TODO : if exist-overwrite, if not-create, if no dir-create
        string mode_str = "";
        for (int i=0;i<4;i++){
            mode_str += ((int)mode[i]).ToString();
        }
        flightConfig.mode = mode_str;
        System.IO.File.WriteAllText(
            System.IO.Path.Combine(Application.persistentDataPath,"FlightConfigs","config_current.json"),
            flightConfig.ToJson()
        );
    }
    public ControlMode GetFlightMode(int axis){
        return mode[axis];
    }
    public bool SetFlightMode(int axis, ControlMode newMode){
        if (axis == 0){ // roll
            Debug.Log("FlightController.SetFlightMode: axis[roll] only supports mode[raw]");
            return false;
        }
        if (axis == 1) { // pitch
            Debug.Log("FlightController.SetFlightMode: axis[pitch] only supports mode[raw]");
            return false;
        }
        if (axis == 2 || axis == 3) { // thrust or yaw
            if (xgb_interface.GetSupportedAxes()[axis]){ // when ai assist is supported
                mode[axis] = newMode;
                return true;
            } else { // when AI assist is not available
                bool flag = true;
                if (newMode != ControlMode.raw){ 
                    Debug.Log("FlightController.SetFlightMode: xgb interface of axis "+axis+" not initialized");
                    flag = false;
                }
                mode[axis] = ControlMode.raw;
                return flag;
            }
        }
        return false;
    }
    public void ResetState()
    {
        transform.SetPositionAndRotation(initPos, initRot);
        rb.linearVelocity *= 0;
        rb.angularVelocity *= 0;
        for (int i=0;i<4;i++)
        {
            signals[i] = 1500;
            signals0[i] = 1500;
        }
        signals[2] = 1000;
        signals0[2] = 1000;
    }
    public void ConfigureCamera(){
        // reset FOV
        FPV_cam.GetComponent<Camera>().fieldOfView = flightConfig.cam_fov;
        // reset angle
        FPV_cam.transform.rotation = transform.rotation * Quaternion.Euler(-flightConfig.cam_ang,0,0);
    }
    private void ApplyPhysics()
    {
        // local angularvelocity (pitch,yaw,-roll) rad/s
        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);
        // convert to degree/s
        localAngularVelocity *= 180/Mathf.PI;
        localAngularVelocity.z *= -1;
        

        float error;
        //roll
        error = BetaFlightActualRate(signals[0],0) - localAngularVelocity.z;
        rb.AddRelativeTorque(Vector3.forward*torque_coef*-error);
        signals0[0] = signals[0];
        //pitch
        error = BetaFlightActualRate(signals[1],1) - localAngularVelocity.x;
        // Debug.Log(signals[1] +" "+ BetaFlightActualRate(signals[1],1)+" "+localAngularVelocity);
        rb.AddRelativeTorque(Vector3.left*torque_coef*-error);
        signals0[1] = signals[1];
        // d_signals[1] = 0;
        //thrust
        if (thrust_switch){
            float coef = rb.mass*flightConfig.thrust_max*9.81f/1000;
            rb.AddRelativeForce(Vector3.up*coef*(signals[2]-1000));
        }
        signals0[2] = signals[2];
        //yaw
        error = BetaFlightActualRate(signals[3],3) - localAngularVelocity.y;
        rb.AddRelativeTorque(Vector3.down*torque_coef*-error);
        // Debug.Log(signals[3] + " " + (signals[3]-signals0[3]));
        signals0[3] = signals[3];
    }
    private float BetaFlightActualRate(float signal, int axis_id){
        // get axis
        float csen, maxr, expo;
        if (axis_id == 0){
            csen = flightConfig.roll_cs;
            maxr = flightConfig.roll_max;
            expo = flightConfig.roll_exp;
        } else if (axis_id == 1){            
            csen = flightConfig.pitch_cs;
            maxr = flightConfig.pitch_max;
            expo = flightConfig.pitch_exp;
        } else if (axis_id == 3){
            csen = flightConfig.yaw_cs;
            maxr = flightConfig.yaw_max;
            expo = flightConfig.yaw_exp;
        } else {return 0;}
        // calculate angle rate in deg/s
        signal = (signal-1500)/500; 
        expo /= 1000;
        expo = Mathf.Abs(signal)*(Mathf.Pow(signal,5)*expo+signal*(1-expo));
        float stickMovement = Mathf.Max(0,maxr-csen);
        return signal*csen+stickMovement*expo;
    }
}
