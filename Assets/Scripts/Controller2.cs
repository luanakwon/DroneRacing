using UnityEngine;

public class Controller2 : MonoBehaviour
{
    // public CursorLockMode cursorlock;
    private Vector2 mouseAxis;
    private Vector2 keyboardAxis;
    private FlightCon fc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fc = gameObject.GetComponent<FlightCon>();
        // lock cursor
        // Cursor.lockState = cursorlock;

        // init those required
        mouseAxis = Vector2.zero;
        keyboardAxis = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // reset state with Space key
        if (Input.GetKey(KeyCode.Space)){
            fc.ResetState();
            mouseAxis *= 0;
            keyboardAxis *= 0;
        }

        // read user inputs
        ReadInput();
        // send signal to flightcon
        fc.SetSignal(
            Mathf.Clamp(mouseAxis.x,-1,1)*500+1500,
            Mathf.Clamp(mouseAxis.y,-1,1)*500+1500,
            keyboardAxis.y*1000+1000,
            keyboardAxis.x*500+1500
        );
    }

    // Read and process the inputs
    private void ReadInput()
    {
        // mouse xy. raw input is too big to use
        mouseAxis.x += Input.GetAxis("Mouse X")/100;
        mouseAxis.y += Input.GetAxis("Mouse Y")/100;
        keyboardAxis.x = Input.GetAxisRaw("Horizontal");
        // keyboard w for throttle.
        // keyboard can only have 0 or 100% throttle
        keyboardAxis.y = Mathf.Clamp01(Input.GetAxisRaw("Vertical"));

        // Debug.Log(mouseAxis + " " + keyboardAxis);
    }
    
    // not necessary, but just in case,
    // a method that controls each axis with keypad. 
    private void ManualKeypadControl(float torque){
        // manual add Torque
        //pitch
        if (Input.GetKey(KeyCode.Keypad8)){
            mouseAxis.y = 0.2f;
        } else if (Input.GetKey(KeyCode.Keypad2)) {
            mouseAxis.y = -0.2f;
        }
        //roll
        if (Input.GetKey(KeyCode.Keypad4)){
            mouseAxis.x = -0.2f;
        } else if (Input.GetKey(KeyCode.Keypad6)) {
            mouseAxis.x = 0.2f;
        }
        //yaw
        if (Input.GetKey(KeyCode.Keypad1)){
            keyboardAxis.x = -0.2f;
        } else if (Input.GetKey(KeyCode.Keypad3)) {
            keyboardAxis.x = 0.2f;
        }
    }
}
