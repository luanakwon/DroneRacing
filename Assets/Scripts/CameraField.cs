using TMPro;
using UnityEngine;

public class CameraField : MonoBehaviour
{
    public string param;
    public FlightCon drone;
    private ConfigHandler configHandler;
    private TMP_InputField UI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        configHandler = FindFirstObjectByType<ConfigHandler>();
        UI = gameObject.GetComponent<TMP_InputField>();
        UI.text = configHandler.GetConfig(ControlMode.raw, param).ToString();

        UI.onEndEdit.AddListener(
            delegate {
                configHandler.SetConfig(UI, ControlMode.raw, param);
                drone.ConfigureCamera();
            }
        );
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    // public void ChangeCameraFOV(){
    //     configHandler.SetConfig(UI, ControlMode.raw, param);
    //     drone.ConfigureCamera();
    // }
    // public void ChangeCameraAngle(){
    //     configHandler.SetConfig(UI, ControlMode.raw, param);
    //     drone.ConfigureCamera();
    // }
}
