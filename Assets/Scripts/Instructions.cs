using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    public TMP_Text UI;

    private Dictionary<string,string> instructionSet = new Dictionary<string, string>();

    void Awake(){
        string jspath = System.IO.Path.Combine(
            Application.streamingAssetsPath,"Instruction","instruction.json");
        string jstr = System.IO.File.ReadAllText(jspath);
        // Debug.Log(jstr);
        LoadInstructions(jstr);
    }

    private void LoadInstructions(string jstr){        
        string[] literals = jstr.Trim(new char[]{' ','\r','\n','\t'}).Split('\"');
        for (int i=0;i<literals.GetLength(0);i++){
            if (literals[i] == ":"){
                instructionSet[literals[i-1]] = literals[i+1];
                i++;
            }
        }

    }

    public void Show(string key){
        UI.text = instructionSet[key];
    }
}
