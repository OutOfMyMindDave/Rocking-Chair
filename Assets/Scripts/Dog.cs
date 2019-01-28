using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    public enum State
    {
        GOING_TO_POOP,
        RUNNING_AWAY,
    }

    public State state;

    public float poopSpeed;
    public float runAwaySpeed;

    public List<Transform> waypointsToPoop;
    public List<Transform> waypointsToRunAway;

    public GameObject poop;
    public AudioSource poopSound;

    public int waypointIndex;

    private void Start()
    {
        state = State.GOING_TO_POOP;
        transform.position = waypointsToPoop[0].position;
        transform.rotation = waypointsToPoop[0].rotation;
        waypointIndex = 1;
    }

    private void Update()
    {
        float speed = poopSpeed;

        if(state == State.RUNNING_AWAY)
        {
            speed = runAwaySpeed;
        }

        Transform target;

        if(state == State.GOING_TO_POOP)
        {
            target = waypointsToPoop[waypointIndex];
        }
        else
        {
            target = waypointsToRunAway[waypointIndex];
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position);

        if(Vector3.Distance(transform.position, target.position) < .1f)
        {
            waypointIndex++;

            if(state == State.GOING_TO_POOP)
            {
                if(waypointIndex == waypointsToPoop.Count)
                {
                    PlayPoopSound();
                    PlacePoop();
                    state = State.RUNNING_AWAY;
                    waypointIndex = 0;
                }
            }
            else
            if(state == State.RUNNING_AWAY)
            {
                if (waypointIndex == waypointsToRunAway.Count)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void PlayPoopSound()
    {
        poopSound.Play();
    }

    private void PlacePoop()
    {
        Instantiate(poop, transform.position, transform.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        state = State.RUNNING_AWAY;
        waypointIndex = 0;
    }
}
