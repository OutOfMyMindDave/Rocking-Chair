using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockingTracker : MonoBehaviour
{
    public enum RockingState
    {
        STILL,
        BACKWARD,
        FORWARD,
        CHAOS,
        FALLEN,
    }

    public Transform playerTransform;
    public Transform headTransform;

    public Animator eyelidAnimator;
    public Animator rockingChairAnimator;

    [Tooltip("This is the minimum distance that you should be rocking to get full credit.")]
    public float rockingDistanceMin = .25f;
    [Tooltip("This is the maximum distance that you should be rocking to get full credit.")]
    public float rockingDistanceMax = .5f;
    [Tooltip("This is the maximum distance that you should be rocking before you fall over.")]
    public float rockingDistanceFall = 1f;
    [Tooltip("This is the minimum amount of time that you should move each direction to get full credit.")]
    public float rockingTimeMin = .5f;
    [Tooltip("This is the maximum amount of time that you should move each direction to get full credit.")]
    public float rockingTimeMax = 1.5f;

    public float sleepTime = 5;

    public float tiredness;

    public RockingState rockingState = RockingState.STILL;

    public List<AudioClip> audioClipList;

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 prevPos;

    private Vector3 centerPos;

    private float startTime;
    private float currentTime;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            ResetPosition();
        }

        Vector3 headPos = headTransform.position;

        currentTime += Time.deltaTime;

        float tirednessChange = Time.deltaTime / sleepTime;

        if(rockingState == RockingState.STILL)
        {
            centerPos = headPos;
            startPos = headPos;
            endPos = headPos;

            if (headPos.z > startPos.z)
            {
                rockingState = RockingState.FORWARD;
            }
            else
            {
                rockingState = RockingState.BACKWARD;
            }
        }
        else
        if(rockingState == RockingState.BACKWARD)
        {
            if(headPos.z > Mathf.Lerp(endPos.z, startPos.z, .1f))
            {
                // Switch direction
                startPos = headPos;
                endPos = headPos;
                currentTime = 0f;
                rockingState = RockingState.FORWARD;

                PlayCreak();
            }
            else
            if(headPos.z < endPos.z)
            {
                endPos = headPos;

                if ((rockingTimeMin < currentTime) && (currentTime < rockingTimeMax))
                {
                    tirednessChange = -(Time.deltaTime / sleepTime);
                }
                else
                {
                    tirednessChange = 0;
                }
            }
        }
        else
        if (rockingState == RockingState.FORWARD)
        {
            if (headPos.z < Mathf.Lerp(endPos.z, startPos.z, .1f))
            {
                // Switch direction
                startPos = headPos;
                endPos = headPos;
                currentTime = 0f;
                rockingState = RockingState.BACKWARD;

                PlayCreak();
            }
            else
            if(headPos.z > endPos.z)
            {
                endPos = headPos;

                if ((rockingTimeMin < currentTime) && (currentTime < rockingTimeMax))
                {
                    tirednessChange = -(Time.deltaTime / sleepTime);
                }
                else
                {
                    tirednessChange = 0;
                }
            }
        }

        prevPos = headTransform.position;

        tiredness = Mathf.Clamp(tiredness + tirednessChange, 0f, 1f);

        eyelidAnimator.SetFloat("Open", Mathf.Clamp(1f - tiredness, 0f, 1f));

        float frontZ = centerPos.z + rockingDistanceMax;
        float backZ = centerPos.z - rockingDistanceMax;
        float chairPosition = Mathf.Clamp01((headPos.z - frontZ) / (backZ - frontZ));

        rockingChairAnimator.SetFloat("RockerPosition", chairPosition);
    }

    private void ResetPosition()
    {

        centerPos = headTransform.position;
    }

    private void PlayCreak()
    {
        if (audioClipList.Count > 0)
        {
            GetComponent<AudioSource>().PlayOneShot(audioClipList[Random.Range(0, audioClipList.Count)]);
        }
    }
}
