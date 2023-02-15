using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBase : MonoBehaviour
{
    public static event System.Action OnGuardHasSpottedPlayer;
    
    [SerializeField] private Transform pathHolder;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float timeToSpotPlayer = 0.5f;

    [SerializeField] private Light spotlight;
    [SerializeField] private float viewDistance;
    [SerializeField] private LayerMask obstacleMask;
    
    private float viewAngle;
    private float playerVisableTimer;
    
    private Transform player; 

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.transform.position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder) {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward*viewDistance);
    }


    // Start is called before the first frame update
    void Awake()
    {
        viewAngle = spotlight.spotAngle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }

    bool isPlayerVisable()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance) {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f) {
                if (!Physics.Linecast(transform.position, player.position, obstacleMask)) {
                    return true;
                }
            }
        }
        return false;
    }
    
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngel = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngel)) > 0.05)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngel, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true) {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint) {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }

            yield return null;
        }
    }
    
    

    // Update is called once per frame
    void Update()
    {
        if (isPlayerVisable()) {
            playerVisableTimer += Time.deltaTime;
        }
        else {
            playerVisableTimer -= Time.deltaTime;
        }

        playerVisableTimer = Mathf.Clamp(playerVisableTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(Color.yellow, Color.red, playerVisableTimer / timeToSpotPlayer);

        if (playerVisableTimer >= timeToSpotPlayer) {
            if (OnGuardHasSpottedPlayer != null) {
                OnGuardHasSpottedPlayer();
            }
        }
    }
}
