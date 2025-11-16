using System.Collections;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
[AddComponentMenu("AI Behaviours/NPC")]
public class NPC : MonoBehaviour
{
    [System.Serializable]
    public enum DebugLevel
    {
        NONE,
        MESSAGES,
        VERBOSE,
        EVERYTHING
    }
    public DebugLevel debugLevel = DebugLevel.NONE;

    // Class-level declarations

    [SerializeField] private AudioClip deathSound;

    [Header("Behaviours")]

    [SerializeField] private ConstrainedTrigger[] triggers;

    [Tooltip("Order these in descending order of priority. The first behaviour that fits the constraints will be chosen for pathfinding.")]
    [SerializeField] private Behaviour[] movementBehaviours;

    private int activeBehaviour = -1;
    private int checkActiveBehaviour = -1;
    private float timer;
    private readonly System.Random rand = new();

    [Header("Movement and Pathfinding")]

    private Transform target;
    public Transform _Target
    {
        get { return target; }
        set { target = value; }
    }
    private Vector3 eyePos;
    private Vector3 targetAtEyeHeight;

    [SerializeField] private float health = 100;
    public float _Health => health;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float height = 0.5f;

    private bool chasing = false;
    public bool _Chasing
    {
        get { return chasing; }
        set { chasing = value; }
    }

    [SerializeField] private float seeingDistance = 10;
    [SerializeField] private LayerMask obstacleLayer;
    private NPCManager NPCManager;
    private bool seeingTarget = false;
    public bool _SeeingTarget
    {
        get { return seeingTarget; }
        set { seeingTarget = value; }
    }
    private bool attacking = false;
    public bool _Attacking
    {
        get { return attacking; }
        set { attacking = value; }
    }
    private float attackTime = 2f;
    private float cooldown = 0f;

    private bool sawCandyStolen = false;
    public bool _SawCandyStolen
    {
        get { return sawCandyStolen; }
        set { sawCandyStolen = value; }
    }

    private bool followingCommand = false;
    public bool _FollowingCommand
    {
        get { return followingCommand; }
        set { followingCommand = value; }
    }

    private Seeker seeker;
    private Path path;
    [HideInInspector] public EntitySpawner parent;
    [HideInInspector] public int id;

    private int currentWaypoint = 0;
    [SerializeField] private float nextWaypointDistance = 3;

    [SerializeField] private bool drawGizmos = false;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        target = GameManager.active.player;
    }

    private void OnEnable()
    {
        seeker.pathCallback += OnPathComplete;
    }

    private void OnDisable()
    {
        seeker.pathCallback -= OnPathComplete;
    }

    /*
    Initialize all of the constraints with the reference to this gameObject so the constraint can
    access the object properties
    */
    private void Start()
    {
        foreach (Behaviour b in movementBehaviours)
        {
            b.behaviour.Initialize(gameObject);

            foreach (Constraint c in b.constraints)
                c.Initialize(gameObject);
        }

        foreach (ConstrainedTrigger t in triggers)
        {
            t.trigger.Initialize(this);

            foreach (Constraint c in t.constraints)
                c.Initialize(gameObject);
        }

        NPCManager = FindObjectOfType<NPCManager>();
    }

    /*
    Go down the list of behaviours and make sure the correct one is selected before pathfinding 
    with the correct targeting behaviour
    */
    private void Update()
    {
        if (health <= 0)
        {
            NPCManager.RemoveEnemy(this);
            Destroy(gameObject);

        }
        if (cooldown > 0f)
            cooldown -= Time.deltaTime;
        /*
        Check if NPC can see target
        Returns true if the raycast hits no obstacles (returns false), false if the raycast 
        hits obstacles (returns true)
         > ie seeingTarget = !Raycast
        */
        float dist;
        eyePos = transform.position + Vector3.up * height;
        targetAtEyeHeight = target.position + Vector3.up * height;

        seeingTarget = !Physics.Raycast(
            eyePos,
            targetAtEyeHeight - eyePos,
            seeingDistance > (dist = Vector3.Distance(eyePos, targetAtEyeHeight)) ? dist : seeingDistance,
            obstacleLayer
        );
        Debug.DrawRay(
        eyePos,
        2 * (targetAtEyeHeight - eyePos).normalized,
        seeingTarget ? Color.green : Color.red
        );


        // Invoke all applicable triggers
        foreach (ConstrainedTrigger t in triggers)
        {
            bool eval;
            if (eval = Constraint.Evaluate(t.constraints, debugLevel >= DebugLevel.EVERYTHING))
                t.trigger.Invoke();
            if (debugLevel >= DebugLevel.EVERYTHING) Log(string.Format("{0} evaluated as {1}", t.trigger.name, eval));
        }

        // Check whether behaviour has changed
        activeBehaviour = GetActiveBehaviour();

        // Return early if no behaviour is selected
        if (activeBehaviour == -1)
        {
            if (debugLevel >= DebugLevel.MESSAGES) Log("No AI behaviour - selecting a behaviour");
            return;
        }

        // Generate new path if AI Behaviour changed
        if (activeBehaviour != checkActiveBehaviour)
        {
            if (debugLevel >= DebugLevel.VERBOSE) Log("New AI behaviour - generating new path");
            checkActiveBehaviour = activeBehaviour;
            StartCoroutine(StartPath());
        }

        AIBehaviour active = movementBehaviours[activeBehaviour].behaviour;

        timer += Time.deltaTime;
        // Request new path after an interval if defined by behaviour, do not erase previous path during generation
        if (active.continuous && timer > active.minimumInterval)
        {
            timer = 0;
            StartCoroutine(StartPath(false));
        }

        // Follow path, using whether path exists as a status variable
        if (path != null)
        {
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            Vector3 movement = (speed + active.speedModifier) * active.multiplicativeSpeedMultiplier * Time.deltaTime * dir;
            if (debugLevel >= DebugLevel.VERBOSE)
                Log(string.Format(
                    "Waypoint {0} = {1}\nMovement vector = {2}",
                    currentWaypoint,
                    path.vectorPath[currentWaypoint],
                    movement
                ));
            transform.Translate(movement);

            // Complete all waypoints which are close enough
            while (true)
            {
                float distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < nextWaypointDistance)
                    if (currentWaypoint + 1 < path.vectorPath.Count)
                        currentWaypoint++;
                    else
                    {
                        if (debugLevel >= DebugLevel.VERBOSE) Log("Finished path - generating new path");
                        StartCoroutine(StartPath());
                        break;
                    }
                else break;
            }
        }
        if (attacking && cooldown <= 0f)
        {
            Attack();
        }
    }

    private int GetActiveBehaviour()
    {
        for (int i = 0; i < movementBehaviours.Length; i++)
            if (Constraint.Evaluate(movementBehaviours[i].constraints))
                return i;  // Stop loop when the first viable AI behaviour is found

        return -1;
    }

    private IEnumerator StartPath(bool reset = true)
    {
        if (reset)
            path = null;    // Reset path like a status variable

        if (activeBehaviour != -1)
        {
            AIBehaviour active = movementBehaviours[activeBehaviour].behaviour;
            // Get target and path to target
            Vector3 targetPosition = active.SelectTarget();
            yield return new WaitForSeconds(active.minimumInterval + (float)(rand.NextDouble() * active.randomInterval));
            seeker.StartPath(transform.position, targetPosition);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (p.error == true) return;

        path = p;
        currentWaypoint = 0;    // Reset to avoid IndexOutOfBounds errors
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !drawGizmos) return;

        if (0 <= activeBehaviour && activeBehaviour < movementBehaviours.Length)
            movementBehaviours[activeBehaviour].behaviour.Gizmos();
    }

    private void Attack()
    {
        target.GetComponent<PlayerController>().DamagePlayer();
        cooldown = attackTime;
    }

    private IEnumerator OnSleep()
    {
        // Disable movement
        speed = 0;

        // Play death sound
        GameManager.active.universalSoundEffect.PlayOneShot(deathSound);

        // Despawn NPC
        Despawn();

        yield break;
    }

    public void Sleep()
    {
        if (debugLevel > DebugLevel.NONE) Log("Sleeping");
        StartCoroutine(OnSleep());
    }

    private void Despawn()
    {
        if (parent != null)
            parent.Despawn(id);
        else
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(GameManager.active.projectileTag))
            Sleep();
    }

    public void ReceiveCommand()
    {
        followingCommand = true;
        sawCandyStolen = true;
    }

    public void SendRandomCommand()
    {
        if (debugLevel > DebugLevel.NONE) Log("Sending Command");
        GameObject[] temp;
        (temp = GameManager.active.floorClerkSpawner.GetAllSpawnedEntities())[Random.Range(0, temp.Length)].GetComponent<NPC>().ReceiveCommand();
    }

    public void Log(string message)
    {
        Debug.Log(gameObject.name + " - " + message);
    }
}

[System.Serializable]
public struct Behaviour
{
    public AIBehaviour behaviour;
    public Constraint[] constraints;
}

[System.Serializable]
public struct ConstrainedTrigger
{
    public Trigger trigger;
    public Constraint[] constraints;
}