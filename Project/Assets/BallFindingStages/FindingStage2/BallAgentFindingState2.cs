using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

public class BallAgentFindingState2 : Agent
{
    public List<Transform> SpawningAreas;
    public List<Transform> Obsticles;
    public Transform Target;

    Rigidbody _agentRigidbody;
    private float _speed = 20;
    private float _timeRemaining = 30f;
    private float _time;
    private bool _timerIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("stage2");
        _agentRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Timer();
    }

    private void Timer()
    {
        if (_timerIsRunning)
        {
            if (_time > 0)
            {
                _time -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                _time = 0;
                _timerIsRunning = false;
                //SetReward(-1.0f);
                EndEpisode();
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        _time = _timeRemaining;
        _timerIsRunning = true;

        // Reset agent
        _agentRigidbody.angularVelocity = Vector3.zero;
        _agentRigidbody.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0.5f, 0);

        int targetArea = Random.Range(0, SpawningAreas.Count);
        Target.localPosition = SpawningAreas[targetArea].localPosition;

        int obscticleIndex = 0;
        for (int i = 0; i < SpawningAreas.Count; i++)
        {
            if (i.Equals(targetArea))
                continue;

            Obsticles[obscticleIndex].transform.localPosition = SpawningAreas[i].localPosition;
            obscticleIndex++;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions & Agent velocity
        sensor.AddObservation(Target.localPosition);
        foreach (var VARIABLE in Obsticles)
        {
            sensor.AddObservation(VARIABLE.localPosition);
        }


        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(_agentRigidbody.velocity);
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

        _agentRigidbody.AddForce(controlSignal * _speed);
    }

    /// <summary>
    /// When the agent collides with something, take action
    /// </summary>
    /// <param name="collision">The collision info</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("target"))
        {
            Debug.Log("target");
            SetReward(1.0f);
            EndEpisode();
        }
        else if (collision.transform.CompareTag("wall"))
        {
            Debug.Log("wall");
            EndEpisode();
        }
        else if (collision.transform.CompareTag("obsticle"))
        {
            Debug.Log("obsticle");
            SetReward(-1.0f);
            EndEpisode();
        }
    }
}
