using UnityEngine;

public class TEMP : MonoBehaviour
{
    public float thrust;
    public float torque;
    public float sensitivity;
    public float threshold;
    public float pitchRate;
    public float yawRate;
    public float rollRate;
    public Vector3 angularV2;


    [SerializeField] private Vector2 mouseDelta;
    [SerializeField] private int mouseClick = 0;
    [SerializeField] private float wheel = 0;
    [SerializeField] private Vector3 angularV;
    
    private Rigidbody rb;
    private Vector3 initpos;
    private Quaternion initrot;
    private Vector3 baseThrust = Vector3.up*0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = gameObject.GetComponent<Rigidbody>();
        mouseDelta = Vector2.zero;

        transform.GetPositionAndRotation(out initpos, out initrot);
        baseThrust *= 9.81f*rb.mass;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space)){
            transform.SetPositionAndRotation(initpos, initrot);
            rb.angularVelocity *= 0;
            mouseDelta *= 0;
        }

        // accumulate mouse
        mouseDelta.x = Input.GetAxis("Mouse X") / sensitivity;
        mouseDelta.y = Input.GetAxis("Mouse Y") / sensitivity;
        // read mouse click
        int click_delta;
        if (Input.GetMouseButton(0)){
            click_delta = mouseClick < 0 ? 0 : -1;
            mouseClick = -1;
            
        } else if (Input.GetMouseButton(1)){
            click_delta = mouseClick > 0 ? 0 : 1;
            mouseClick = 1;
        } else {
            click_delta = 0-mouseClick;
            mouseClick = 0;
        }
        // read wheel
        wheel = Mathf.Clamp01(wheel + Input.GetAxis("Mouse ScrollWheel")/5);
    

        // apply torque
        //pitch
        rb.AddRelativeTorque(Vector3.left*pitchRate*-mouseDelta.y);
        //roll
        rb.AddRelativeTorque(Vector3.forward*rollRate*-mouseDelta.x);
        //yaw
        rb.AddRelativeTorque(Vector3.down*yawRate*-click_delta);
        //thrust
        rb.AddRelativeForce(Vector3.up*thrust*wheel + baseThrust);

        // keypad control feature
        ManualKeypadControl();
        //update angular velocity
        angularV = rb.angularVelocity;
        angularV2 = transform.InverseTransformDirection(angularV);
        
    }

    void ManualKeypadControl(){
        // manual add Torque
        //pitch
        if (Input.GetKey(KeyCode.Keypad8)){
            rb.AddRelativeTorque(Vector3.left*torque*-pitchRate, ForceMode.VelocityChange);
        } else if (Input.GetKey(KeyCode.Keypad2)) {
            rb.AddRelativeTorque(Vector3.left*torque*pitchRate);
        }
        //roll
        if (Input.GetKey(KeyCode.Keypad4)){
            rb.AddRelativeTorque(Vector3.forward*torque*-rollRate);
        } else if (Input.GetKey(KeyCode.Keypad6)) {
            rb.AddRelativeTorque(Vector3.forward*torque*rollRate);
        }
        //yaw
        if (Input.GetKey(KeyCode.Keypad1)){
            rb.AddRelativeTorque(Vector3.down*torque*-yawRate);
        } else if (Input.GetKey(KeyCode.Keypad3)) {
            rb.AddRelativeTorque(Vector3.down*torque*yawRate);
        }
    }

    
}
