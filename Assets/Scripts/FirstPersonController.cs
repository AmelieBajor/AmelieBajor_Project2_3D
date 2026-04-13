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

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }

    void Update()
    {

        Movement();
        MouseLook();
        Jumping();

        if (heldItem != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                heldItem.Throw(throwForce, cam.transform.forward);
                heldItem = null;
            }
        }

        if (ObjectInFocus() != null)
        {
            float distanceToObject = Vector3.Distance(cam.transform.position, ObjectInFocus().transform.position);
            if (Input.GetMouseButtonDown(0))
            {
                impactPS.transform.position = hitPoint;
                impactPS.Emit(particleCount);

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
        }

    }

    void Movement()
    {

        float verInput = Input.GetAxis("Vertical");
        float horInput = Input.GetAxis("Horizontal");
        float verSpeed = verInput * (walkSpeed + sprintSpeedAdd);
        float horSpeed = horInput * (walkSpeed + sprintSpeedAdd);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            sprintSpeedAdd = 20;
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
}
