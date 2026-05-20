using UnityEngine;

public class NPCScript : MonoBehaviour
{
    //private CharacterController controller;
    public Transform player;
    //private Vector3 targetPoint;
    private Vector3 directionToPlayer;
    public float turnTimer;
    private float turnTimerMax = 3;

    //public float rotationSpeed;

    public float viewAngle = 120;
    public float viewRange = 5;
    public float detectionRadius = 2;
    //float walkSpeed = 1;

    public LayerMask playerLayer;

    public Transform turn;
    private Vector3 turnPoint;
    private Vector3 directionToTurn;
    private Vector3 lastKnownPosition;

    private UnityEngine.AI.NavMeshAgent agent;

    private bool patrolling = true;
    private bool playerFound = false;
    public float alertDuration = 5;
    private float timeSinceAlerted = 0;

    public Transform[] waypoints;
    private Transform targetWaypoint;
    private int waypointIndex = 0;

    private float waitingTimer;
    public float maxWaitingTime;
    private bool isWaiting;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //controller = GetComponent<CharacterController>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        SetNextTargetWaypoint(true);
    }

    private void SetNextTargetWaypoint(bool firstTime = false)
    {
        if (!firstTime)
        {
            waypointIndex++;
        }
        if (waypointIndex >= waypoints.Length)
        {
            waypointIndex = 0;
        }

        targetWaypoint = waypoints[waypointIndex];
        agent.SetDestination(targetWaypoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        directionToPlayer = (player.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(directionToPlayer);



        if (PlayerDetected())
        {
            playerFound = true;
            patrolling = false;
            timeSinceAlerted = 0;
            waitingTimer = 0;
            isWaiting = false;
            lastKnownPosition = player.position;
            agent.SetDestination(lastKnownPosition);


            //turnTimer = 0;
            //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, rot, agent.angularSpeed * Time.deltaTime);
        }

        if (playerFound)
        {
            if (timeSinceAlerted < alertDuration)
            {
                timeSinceAlerted += Time.deltaTime;
            }
            else
            {
                playerFound = false;
                timeSinceAlerted = 0;
                patrolling = true;
                SetNextTargetWaypoint(true);

            }

        }
        /*
                if (patrolling)
                {
                    Patrolling();
                }
         */

    }

    private void Patrolling()
    {


        float dist = Vector3.Distance(transform.position, targetWaypoint.position);
        float buffer = 0.5f;

        if (dist < buffer && !isWaiting)
        {
            isWaiting = true;

        }

        if (isWaiting)
        {
            if (waitingTimer < maxWaitingTime)
            {
                waitingTimer += Time.deltaTime;

            }
            else
            {
                SetNextTargetWaypoint();
                waitingTimer = 0;
                isWaiting = false;

            }
        }

    }

    private bool PlayerDetected()
    {
        bool result = false;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle < viewAngle / 2)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, viewRange, playerLayer))
            {
                result = true;
            }
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= detectionRadius)
        {
            result = true;
        }


        return result;
    }
}
