using UnityEngine;

public class FirstPersonController : MonoBehaviour
{

    private CharacterController characterController;
    public float walkSpeed = 5;
    public float sprintSpeedAdd;
    public float mouseSensitivity = 2;
    float verticalRotation;
    float upDownRange = 80;

    public float jumpForce = 5;
    private Vector3 currentMovement;
    private float gravity = 9.81f;
    private Vector3 hitPoint;
    public ParticleSystem impactPS;
    [Range(1, 50)] public int particleCount = 20;

    public float pickUpRange = 2;
    public Transform holdPoint;

    public float throwForce = 5;
    private ItemScript heldItem = null;
    private BuildingHealthScript buildingHealth;

    public GameObject platform;

    private Camera cam;


    //Camera
    public Vector3 ogCamPos;
    public Transform cameraPositionTransform;
    public GameObject cameraObject;
    public float shakeDistance = 0.05f;
    public float shakeTimer;
    public float maxShakeTimer = 0.5f;

    private AudioSource audioSource;
    public AudioClip stompClip;
    public AudioClip shootClip;
    public AudioClip beamClip;






    private bool beamActive = false;
    private float beamDamageTimer;
    public float maxBeamDamageTimer = 0.1f;
    public GameObject beamAttack;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
        audioSource = GetComponent<AudioSource>();

        ogCamPos = new Vector3(0, 0, 0);
        


    }

    void Update()
    {

        Movement();
        MouseLook();
        Jumping();

        cameraObject.transform.localPosition = ogCamPos;



        if (heldItem != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                heldItem.Throw(throwForce, cam.transform.forward);
                heldItem = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (beamActive == true)
            {
                beamActive = false;;
            }
            else if (beamActive == false)
            {
                beamActive = true;
            }
        }

        if (beamActive == false)
        {
            beamAttack.SetActive(false);
        }

        if (beamActive == true)
        {
            beamAttack.SetActive(true);
            audioSource.pitch = (1);
            audioSource.PlayOneShot(beamClip);
            CameraShaking();
        }


        if (ObjectInFocus() != null)
        {
            float distanceToObject = Vector3.Distance(cam.transform.position, ObjectInFocus().transform.position);
            if (Input.GetMouseButtonDown(0))
            {
                impactPS.transform.position = hitPoint;
                impactPS.Emit(particleCount);

                audioSource.pitch = (Random.Range(0.7f, 1.7f));
                audioSource.PlayOneShot(shootClip);

                if (ObjectInFocus() != platform && ObjectInFocus().GetComponent<BuildingHealthScript>() != null)
                {
                    buildingHealth = ObjectInFocus().GetComponent<BuildingHealthScript>();
                    buildingHealth.health -= 1;
                    //Destroy(ObjectInFocus());
                }


            }

            if (distanceToObject <= pickUpRange && ObjectInFocus().GetComponent<ItemScript>() != null)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    heldItem = ObjectInFocus().GetComponent<ItemScript>();
                    heldItem.PickUp(cam.transform, holdPoint.position);
                }
            }

            if (beamActive == true)
            {
  
                if (ObjectInFocus() != platform && ObjectInFocus().GetComponent<BuildingHealthScript>() != null)
                {
                    buildingHealth = ObjectInFocus().GetComponent<BuildingHealthScript>();
                    buildingHealth.health -= .05f;
                }
                impactPS.transform.position = hitPoint - new Vector3(0, 0.5f, 0);
                impactPS.Emit(particleCount);
            }
        }

    }


    void Movement()
    {

        float verInput = Input.GetAxis("Vertical");
        float horInput = Input.GetAxis("Horizontal");
        float verSpeed = verInput * (walkSpeed + sprintSpeedAdd);
        float horSpeed = horInput * (walkSpeed + sprintSpeedAdd);

        if (horSpeed > 0 || verSpeed > 0)
        {
            if (shakeTimer < maxShakeTimer)
            {
                shakeTimer += Time.deltaTime;
            }

            else if (shakeTimer >= maxShakeTimer)
            {
                CameraShaking();
                shakeTimer += Time.deltaTime;

                if (shakeTimer > maxShakeTimer + 0.5f)
                {
                    ogCamPos = new Vector3(0, 0, 0);
                    audioSource.pitch = (Random.Range(0.7f, 1.7f));
                    audioSource.PlayOneShot(stompClip);
                    shakeTimer = 0;
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            sprintSpeedAdd = 5;
        }
        else
        {
            sprintSpeedAdd = 0;
        }

        Vector3 horizontalMovement = new Vector3(horSpeed, 0, verSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;
        characterController.Move(currentMovement * Time.deltaTime);


        




    }

    void MouseLook()
    {
        float mouseXRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        cam.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        //local rotation rotates object in relation to the parent


    }

    void Jumping()
    {
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentMovement.y = jumpForce;
                
            }

        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
            ogCamPos = new Vector3(0, 0, 0);

        }
    }


    public GameObject ObjectInFocus()
    {

        GameObject result = null;
        RaycastHit hit;
        

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            result = hit.transform.gameObject;
            hitPoint = hit.point;
        }


        return result;
    }

    void CameraShaking()
    {

        ogCamPos = new Vector3(Random.Range(-shakeDistance, shakeDistance), Random.Range(-shakeDistance, shakeDistance), Random.Range(-shakeDistance, shakeDistance));


    }

}
