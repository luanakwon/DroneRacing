using UnityEngine;

public class Config
{
    private float _thrust_max=3;
    private float _roll_cs=100, _roll_max=1000, _roll_exp=200;
    private float _pitch_cs=100, _pitch_max=1000, _pitch_exp=200;
    private float _yaw_cs=100, _yaw_max=500, _yaw_exp=100;
    private float _cam_fov=90, _cam_ang=30;

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
    public void ToPlayerPref(string uid){
        PlayerPrefs.SetFloat(uid+"_thrust_max",_thrust_max);
        PlayerPrefs.SetFloat(uid+"_roll_cs",_roll_cs);
        PlayerPrefs.SetFloat(uid+"_roll_max",_roll_max);
        PlayerPrefs.SetFloat(uid+"_roll_exp",_roll_exp);
        PlayerPrefs.SetFloat(uid+"_pitch_cs",_pitch_cs);
        PlayerPrefs.SetFloat(uid+"_pitch_max",_pitch_max);
        PlayerPrefs.SetFloat(uid+"_pitch_exp",_pitch_exp);
        PlayerPrefs.SetFloat(uid+"_yaw_cs",_yaw_cs);
        PlayerPrefs.SetFloat(uid+"_yaw_max",_yaw_max);
        PlayerPrefs.SetFloat(uid+"_yaw_exp",_yaw_exp);
        PlayerPrefs.SetFloat(uid+"_cam_fov",_cam_fov);
        PlayerPrefs.SetFloat(uid+"_cam_ang",_cam_ang);
        PlayerPrefs.SetString(uid+"mode",mode);
    }
    public void LoadFromPlayerPref(string uid){
        thrust_max = PlayerPrefs.GetFloat(uid+"_thrust_max",3);
        roll_cs = PlayerPrefs.GetFloat(uid+"_roll_cs",100);
        roll_max = PlayerPrefs.GetFloat(uid+"_roll_max",1000);
        roll_exp = PlayerPrefs.GetFloat(uid+"_roll_exp",200);
        pitch_cs = PlayerPrefs.GetFloat(uid+"_pitch_cs",100);
        pitch_max = PlayerPrefs.GetFloat(uid+"_pitch_max",1000);
        pitch_exp = PlayerPrefs.GetFloat(uid+"_pitch_exp",200);
        yaw_cs = PlayerPrefs.GetFloat(uid+"_yaw_cs",100);
        yaw_max = PlayerPrefs.GetFloat(uid+"_yaw_max",500);
        yaw_exp = PlayerPrefs.GetFloat(uid+"_yaw_exp",100);
        cam_fov = PlayerPrefs.GetFloat(uid+"_cam_fov",90);
        cam_ang = PlayerPrefs.GetFloat(uid+"_cam_ang",30);
        mode = PlayerPrefs.GetString(uid+"mode","0000");
    }
}