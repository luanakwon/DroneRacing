using System;
using System.IO;
using TMPro;
using UnityEngine;

public class ConfigHandler : MonoBehaviour
{
    public FlightCon drone;
    private Config raw_mode_config = new();
    private Config assist_mode_config = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Debug.Log(Application.streamingAssetsPath);
        Debug.Log(Application.persistentDataPath);

        // load config
        try{
            string jspath = Path.Combine(Application.persistentDataPath,"FlightConfigs","config_raw.json");
            string jstr = File.ReadAllText(jspath);
            raw_mode_config.LoadFromJson(jstr);
        } catch (IOException) {
            Debug.LogWarning("No previous user configuration found.");
        }

        try{
            string jspath = Path.Combine(Application.persistentDataPath,"FlightConfigs","config_assisted.json");
            string jstr = File.ReadAllText(jspath);
            assist_mode_config.LoadFromJson(jstr);
        } catch (IOException) {
            Debug.LogWarning("No previous user configuration found.");
        }
    }
    void Start(){
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha0)){
            Debug.Log(raw_mode_config);
        }
    }
    public float[] GetConfig(ControlMode mode, string[] param_names){
        Config config = (mode == ControlMode.raw) ? raw_mode_config : assist_mode_config;
        float[] output = new float[param_names.GetLength(0)];
        for(int i=0;i<param_names.GetLength(0);i++){
            output[i] = (float)(typeof(Config).GetProperty(param_names[i]).GetValue(config));
        }
        return output;
    }
    public float GetConfig(ControlMode mode, string param){
        // get param
        Config config = (mode == ControlMode.raw) ? raw_mode_config : assist_mode_config;
        var property = typeof(Config).GetProperty(param);
        return (property == null) ? 0:(float)property.GetValue(config);
    }
    public void SetConfig(TMP_InputField UI, ControlMode mode, string param){
        // get param
        Config config = (mode == ControlMode.raw) ? raw_mode_config : assist_mode_config;
        var property = typeof(Config).GetProperty(param);
        // get value
        string txt = UI.text;
        float value;
        try {
            value = float.Parse(txt);
        } catch (FormatException) {
            value = 0;
        }
        // update config
        property?.SetValue(config, value);
        // update UI (value can change when setting due to constraints)
        value = (property == null) ? value:(float)property.GetValue(config);
        UI.text = value.ToString();
        // update drone if modes match
        int axis;
        if (param.StartsWith("roll")){
            axis = 0;
        } else if (param.StartsWith("pitch")) {
            axis = 1;
        } else if (param.StartsWith("thrust")) {
            axis = 2;
        } else if (param.StartsWith("yaw")) {
            axis = 3;
        } else if (param.StartsWith("cam")) {
            drone.SetConfig(property, value);
            return;
        } else {return;}
        if ((drone.GetFlightMode(axis)>0) == (mode>0)){
            drone.SetConfig(property, value);
        }
    }
    // TODO : if exist-overwrite, if not-create, if no dir-create
    public void SaveConfig(){
        File.WriteAllText(
            Path.Combine(Application.persistentDataPath,"FlightConfigs","config_raw.json"),
            raw_mode_config.ToJson()
        );
        File.WriteAllText(
            Path.Combine(Application.persistentDataPath,"FlightConfigs","config_assisted.json"),
            assist_mode_config.ToJson()
        );
        drone.SaveConfig();
    }
}

public enum ControlMode {raw=0, semi_assist=1, assist=2}

[Serializable] public class Config {
    private float _thrust_max=3;
    private float _roll_cs, _roll_max=1000, _roll_exp;
    private float _pitch_cs, _pitch_max=1000, _pitch_exp;
    private float _yaw_cs, _yaw_max=1000, _yaw_exp;
    private float _cam_fov=90, _cam_ang=15;

    public float thrust_max{
        get {return _thrust_max;}
        set {_thrust_max = Mathf.Max(0,value);}
    }
    public float roll_max
    {
        get { return _roll_max; }
        set { _roll_max = Mathf.Clamp(value, 0, 1000);
              _roll_cs = Mathf.Min(_roll_cs, _roll_max);}
    }
    public float pitch_max
    {
        get { return _pitch_max; }
        set { _pitch_max = Mathf.Clamp(value, 0, 1000); 
              _pitch_cs = Mathf.Min(_pitch_cs, _pitch_max);}
    }
    public float yaw_max
    {
        get { return _yaw_max; }
        set { _yaw_max = Mathf.Clamp(value, 0, 1000); 
              _yaw_cs = Mathf.Min(_yaw_cs, _yaw_max);}
    }
    public float roll_exp
    {
        get { return _roll_exp; }
        set { _roll_exp = Mathf.Clamp(value, 0, 1000); }
    }
    public float pitch_exp
    {
        get { return _pitch_exp; }
        set { _pitch_exp = Mathf.Clamp(value, 0, 1000); }
    }
    public float yaw_exp
    {
        get { return _yaw_exp; }
        set { _yaw_exp = Mathf.Clamp(value, 0, 1000); }
    }
    public float roll_cs
    {
        get { return _roll_cs; }
        set { _roll_cs = Mathf.Clamp(value, 0, _roll_max); }
    }
    public float pitch_cs
    {
        get { return _pitch_cs; }
        set { _pitch_cs = Mathf.Clamp(value, 0, _pitch_max); }
    }
    public float yaw_cs
    {
        get { return _yaw_cs; }
        set { _yaw_cs = Mathf.Clamp(value, 0, _yaw_max); }
    }
    public float cam_fov
    {
        get {return _cam_fov;}
        set {_cam_fov = Mathf.Clamp(value, 1, 170);}
    }
    public float cam_ang
    {
        get {return _cam_ang;}
        set {_cam_ang = Mathf.Clamp(value, 0,90);}
    }
    public string mode = "0000";

    public string ToJson(){
        string txt = "{\"thrust_max\":"+_thrust_max;
        txt += ",\"roll_cs\":"+_roll_cs+",\"roll_max\":"+_roll_max+",\"roll_exp\":"+_roll_exp;
        txt += ",\"pitch_cs\":"+_pitch_cs+",\"pitch_max\":"+_pitch_max+",\"pitch_exp\":"+_pitch_exp;
        txt += ",\"yaw_cs\":"+_yaw_cs+",\"yaw_max\":"+_yaw_max+",\"yaw_exp\":"+_yaw_exp;
        txt += ",\"cam_fov\":"+_cam_fov+",\"cam_ang\":"+_cam_ang;
        txt += ",\"mode\":\""+ mode+"\"}";
        return txt;
    }
    public void LoadFromJson(string txt){
        // primitive implementation
        int i0,i1,i_f=0;
        float[] values = new float[12];
        
        i0 = txt.IndexOf(':',1);
        i1 = txt.IndexOf(',',1);
        while (i1 > 0 && i1 < txt.Length){
            values[i_f] = float.Parse(txt.Substring(i0+1,i1-i0-1));
            i_f++;
            i0 = txt.IndexOf(':',i1+1);
            i1 = txt.IndexOf(',',i0+1);
        }
        thrust_max = values[0];
        roll_cs = values[1];
        roll_max = values[2];
        roll_exp = values[3];
        pitch_cs = values[4];
        pitch_max = values[5];
        pitch_exp = values[6];
        yaw_cs = values[7];
        yaw_max = values[8];
        yaw_exp = values[9];
        cam_fov = values[10];
        cam_ang = values[11];
        mode = txt.Substring(i0+1+1,4);
    }
}
