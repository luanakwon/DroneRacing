using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Rigidbody rb;
    private Vector3 initpos;
    private Quaternion initrot;

    public float angularDamping;
    public float maxThrust;
    public float pitchSpeed;
    public float rollSpeed;
    public float yawSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        transform.GetPositionAndRotation(out initpos, out initrot);
        Debug.Log("initial angular damping"+rb.angularDamping);
        rb.angularDamping = angularDamping;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // reset button
        if (Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("reset");
            transform.SetPositionAndRotation(initpos,initrot);
            gameObject.GetComponent<Rigidbody>().linearVelocity *= 0;
            gameObject.GetComponent<Rigidbody>().angularVelocity *= 0;
        }

        int ws_raw = 0;
        int ad_raw = 0;
        int ud_raw = 0;
        int rl_raw = 0;
        // w input
        if (Input.GetKey(KeyCode.W)){
            ws_raw = 1;
        }
        // ad input
        if (Input.GetKey(KeyCode.D)){
            ad_raw = 1;
        } else if (Input.GetKey(KeyCode.A)){
            ad_raw = -1;
        }
        // arrow up down input
        if (Input.GetKey(KeyCode.UpArrow)) {
            ud_raw = 1;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            ud_raw = -1;   
        }
        // arrow right left
        if (Input.GetKey(KeyCode.RightArrow)) {
            rl_raw = 1;
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            rl_raw = -1;
        }
        
        // apply thrust
        rb.AddRelativeForce(Vector3.up*maxThrust*ws_raw);
        // apply Yaw
        rb.AddRelativeTorque(Vector3.down*yawSpeed*-ad_raw, ForceMode.Impulse);
        // apply pitch
        rb.AddRelativeTorque(Vector3.left*pitchSpeed*-ud_raw, ForceMode.Impulse);
        // apply roll
        rb.AddRelativeTorque(Vector3.forward*rollSpeed*-rl_raw, ForceMode.Impulse);
    }   
    void Notused()
    {
        // input in range -1 ~ 1
        //float v_axis = Input.GetAxisRaw("Vertical");
        //float h_axis = Input.GetAxisRaw("Horizontal");

        // thrust
        Vector3 base_thrust = gameObject.transform.up * rb.mass * 9.8f;
        float fwd_thrust_coef = 2f;
        float rev_thrust_coef = -1f;
        float input_v = Input.GetAxisRaw("Vertical");
        if (input_v > 0) {
            rb.AddForce(base_thrust * fwd_thrust_coef);
        } else if (input_v < 0) {
            rb.AddForce(base_thrust * rev_thrust_coef);
        } else {
            rb.AddForce(base_thrust);
        }
        
        // roll
        float roll_tq_coef = 4f;
        Vector3 roll_torque = roll_tq_coef * gameObject.transform.forward;
        float input_h = Input.GetAxisRaw("Horizontal");
        float max_angle = 60;
        float curr_angle = gameObject.transform.eulerAngles.z;
        //calculate error angle per input
        float err_angle;
        if (input_h < 0){
            err_angle = Mathf.Repeat(max_angle-curr_angle+180,360)-180;
        } else if (input_h > 0) {
            err_angle = Mathf.Repeat(-max_angle-curr_angle+180,360)-180;
        } else {
            err_angle = Mathf.Repeat(0-curr_angle+180,360)-180;
        }
        // snap if under thres, else add torque
        float thres = 1;
        if (err_angle > -thres && err_angle < thres){
            rb.angularVelocity = new Vector3(rb.angularVelocity.x,rb.angularVelocity.y,0);
            rb.transform.Rotate(0,0,-err_angle);
        } else {
            rb.AddTorque(roll_torque*err_angle/180);
        }
    }
}
