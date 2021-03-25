using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BallAgentLogicPlatforms : Agent
{

    Rigidbody rBody;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public List<Transform> Worlds;
    public List<Transform> spawningAreas;
    public Transform target;
    public List<Transform> obsticles;
    public override void OnEpisodeBegin()
    {
        // Reset agent
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        //this.transform.localPosition = new Vector3(-9, 0.5f, 0);

        // Move target to a new spot
        //target.localPosition = new Vector3(12 + Random.value * 8, Random.value * 3, Random.value * 10 - 5);
        int ballArea = Random.Range(0, spawningAreas.Count);
        Vector3 transformPos = new Vector3(spawningAreas[ballArea].localPosition.x, 0.5f, spawningAreas[ballArea].localPosition.z);
        transform.localPosition = transformPos;

        int targetArea = Random.Range(0, spawningAreas.Count);
        while (targetArea.Equals(ballArea))
        {
            targetArea = Random.Range(0, spawningAreas.Count);
        }
        target.localPosition = spawningAreas[targetArea].localPosition;

        int obscticleIndex = 0;
        for (int i = 0; i < spawningAreas.Count; i++)
        {
            if(i.Equals(ballArea) || i.Equals(targetArea))
                continue;

            obsticles[obscticleIndex].transform.localPosition = spawningAreas[i].localPosition;
            obscticleIndex++;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions & Agent velocity
        sensor.AddObservation(target.localPosition);
        foreach (var VARIABLE in obsticles)
        {
            sensor.AddObservation(VARIABLE.localPosition);
        }

        foreach (var VARIABLE in Worlds)
        {
            sensor.AddObservation(VARIABLE.localPosition);
            sensor.AddObservation(VARIABLE.localScale);
        }

        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity);
    }


    public float speed = 20;
    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;


        if (vectorAction[0] == 1)
        {
            controlSignal.x = 1;
        }
        else
        {
            controlSignal.x = -1;
        }

        if (vectorAction[1] == 1)
        {
            controlSignal.z = 1;
        }
        else
        {
            controlSignal.z = -1;
        }

        rBody.AddForce(controlSignal * speed);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

        foreach (var VARIABLE in obsticles)
        {
            float distanceToObsticle = Vector3.Distance(this.transform.localPosition, VARIABLE.localPosition);
            if (distanceToObsticle < 2f)
            {
                SetReward(-1.0f);
                EndEpisode();
            }
        }

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell of platform
        if (this.transform.localPosition.y < -0.5f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Vertical");
        actionsOut[1] = Input.GetAxis("Horizontal");
    }

}

