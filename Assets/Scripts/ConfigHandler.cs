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

        // load config
        raw_mode_config.LoadFromPlayerPref("raw_cfg");
        assist_mode_config.LoadFromPlayerPref("ai_cfg");
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
    public void SaveConfig(){
        raw_mode_config.ToPlayerPref("raw_cfg");
        assist_mode_config.ToPlayerPref("ai_cfg");
        drone.SaveConfig();
    }
}

public enum ControlMode {raw=0, semi_assist=1, assist=2}