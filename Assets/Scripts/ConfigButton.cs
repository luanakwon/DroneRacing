using TMPro;
using UnityEngine;


public class ConfigButton : MonoBehaviour
{
    public FlightCon drone;
    public TMP_Text UI_text;
    public int axis;
    public string[] param_names;
    

    public ControlMode mode = ControlMode.assist;
    
    private string[] keyList;
    private ConfigHandler configHandler;
    private Instructions inst_UI;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        configHandler = FindFirstObjectByType<ConfigHandler>();
        inst_UI = FindFirstObjectByType<Instructions>();
        mode = drone.GetFlightMode(axis);
        UI_text.text = mode.ToString();

        if (axis == 2){
            keyList = new string[] {
                "thrust_mode_raw",
                "thrust_mode_semi_assisted",
                "thrust_mode_assisted"
            };
        } else if (axis == 3){
            keyList = new string[] {
                "yaw_mode_raw",
                "yaw_mode_semi_assisted",
                "yaw_mode_assisted"
            };
        } else {Debug.LogWarning("Wrong axis. (2 for thrust, 3 for yaw)");}
    }

    public void Next(){
        if (mode < ControlMode.assist){
            mode ++;
            Configure();
            if (keyList != null) {inst_UI.Show(keyList[(int)mode]);}
        }
    }
    public void Prev(){
        if (mode > ControlMode.raw){
            mode --;
            Configure();
            if (keyList != null) {inst_UI.Show(keyList[(int)mode]);}
        }
    }

    private void Configure(){
        UI_text.text = mode.ToString();
        // set the flightmode of the drone at this scene
        drone.SetFlightMode(axis,mode);
        // get configs from matching mode
        float[] values = configHandler.GetConfig(mode,param_names);
        // set the configs of the drone at this scene
        // dont set the thrust in this scene
        drone.SetConfig(param_names, values);
    }
}
