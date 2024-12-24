using TMPro;
using UnityEngine;

public class ConfigField : MonoBehaviour
{
    public ControlMode controlMode;
    public string param_name;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ConfigHandler handler = FindFirstObjectByType<ConfigHandler>();
        TMP_InputField myInputComp = gameObject.GetComponent<TMP_InputField>();

        myInputComp.text = handler.GetConfig(controlMode, param_name).ToString();

        myInputComp.onEndEdit.AddListener(
            delegate {handler.SetConfig(myInputComp, controlMode, param_name);}
        );
    }
}
