using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.EventSystems;

[AddComponentMenu("Camera/Simple Smooth Mouse Look ")]
public class controller : MonoBehaviour
{
    public GameObject ballPrefab;

    public int currentPitch = 24;
    public int currentTimbre = 0;

    public bool hideUI = false;
    Text pitchUI;
    Image sizeUI;
    Text debugUI;
    Slider pitchSlider;
    CanvasGroup canvas;
    GameObject menu;
    Light light;

    private bool gyroEnabled;
    private Gyroscope gyro;

    private bool initiation = true;

    private GameObject cameraContainer;
    private Quaternion rot;

    bool lockCamera = false;

    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor;
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    void Start()
    {
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);
        light = GameObject.FindGameObjectWithTag("MainLight").GetComponent<Light>();
        menu = GameObject.FindGameObjectWithTag("Menu");
        menu.SetActive(false);

        gyroEnabled = EnableGyro();

        pitchUI = GameObject.FindGameObjectWithTag("CurrentPitch").GetComponent<Text>();
        sizeUI = GameObject.FindGameObjectWithTag("CurrentSize").GetComponent<Image>();
        //debugUI = GameObject.FindGameObjectWithTag("Debug").GetComponent<Text>();
        pitchSlider = GameObject.FindGameObjectWithTag("PitchSlider").GetComponent<Slider>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasGroup>();
        UpdateUI();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;
    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            rot = new Quaternion(0, 0, 1, 0);
            return true;
        }

        else return false;

    }

    void Update()
    {
        UpdateUI();

        // Ensure the cursor is always locked when set
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

#if UNITY_STANDALONE || UNITY_EDITOR
        if (!lockCamera)
        {
            // Allow the script to clamp based on a desired target value.
            var targetOrientation = Quaternion.Euler(targetDirection);
            var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

            // Get raw mouse input for a cleaner reading on more sensitive mice.
            var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            // Interpolate mouse movement over time to apply smoothing delta.
            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

            // Find the absolute mouse movement value from point zero.
            _mouseAbsolute += _smoothMouse;

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (clampInDegrees.x < 360)
                _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
                _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

            transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

            var yRot = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRot;
        }

        //Pitch control with scroll wheel and arrow keys
        if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentPitch < 48) currentPitch++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentPitch > 0) currentPitch--;
        }

        //New ball with left click
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            SpawnBall();

            if (initiation)
            {
                GameObject.FindGameObjectWithTag("Help").SetActive(false);
                initiation = false;
            }
        }

        //Reset with right click
        if (Input.GetMouseButtonDown(1))
        {
            Reset();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

#endif

#if UNITY_ANDROID
        if (gyroEnabled)
        {
            //yRotation += -gyro.rotationRateUnbiased.y;
            //xRotation += -gyro.rotationRateUnbiased.x;

            cameraContainer.transform.Rotate(0, -gyro.rotationRateUnbiased.y, 0);
            this.transform.Rotate(-gyro.rotationRateUnbiased.x, 0, 0);

            //transform.eulerAngles = new Vector3(xRotation, yRotation, 0);
        }

        foreach (Touch touch in Input.touches)
        {
            int id = touch.fingerId;
            if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(id))
            {
                SpawnBall();

                if (initiation)
                {
                    GameObject.FindGameObjectWithTag("Help").SetActive(false);
                    initiation = false;
                }
            }
        }
#endif

    }

    private void UpdateUI()
    {
        string[] notes = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };
        string note = notes[currentPitch % 12];
        int octave = Mathf.FloorToInt(currentPitch / 12f);
        pitchUI.text = note + octave;

        float maxSize = 0.5f;
        float minSize = 0.1f;
        float currentSize = Mathf.Lerp(minSize, maxSize, Mathf.InverseLerp(48, 0, currentPitch));
        sizeUI.GetComponent<RectTransform>().localScale = new Vector3(currentSize, currentSize, 1f);

        pitchSlider.value = currentPitch;
    }

    public void ToggleUI()
    {
        if (!hideUI)
        {
            canvas.alpha = 0f;
            hideUI = true;
        }
        else
        {
            canvas.alpha = 1f;
            hideUI = false;
        }
    }

    public void UpdateSlider()
    {
        currentPitch = Mathf.RoundToInt(pitchSlider.value);
    }

    public void SpawnBall()
    {
        GameObject newBall = Instantiate(ballPrefab, transform.position + (transform.forward * 1), Quaternion.identity) as GameObject;
        newBall.GetComponent<ball>().SetPitch(currentPitch, currentTimbre);
        newBall.GetComponent<Rigidbody>().AddForce(transform.forward * 500);
    }

    public void Reset()
    {
        GameObject[] toDestroy = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in toDestroy)
        {
            Destroy(ball);
        }
    }

    public void SetTimbre(int timbre)
    {
        currentTimbre = timbre;

        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in balls)
        {
            ball.GetComponent<ball>().timbre = timbre;
        }
    }

    public void SetLight(float brightness)
    {
        light.intensity = brightness;
    }

    public void SetGraphics(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
    }

    public void ToggleGravity(bool state)
    {
        if (state) Physics.gravity = new Vector3(0, -9.81f, 0);
        else Physics.gravity = Vector3.zero;
    }

    public void ToggleMenu()
    {
        if (menu.activeSelf)
        {
            menu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lockCamera = false;
        }
        else
        {
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            lockCamera = true;
        }
    }
}