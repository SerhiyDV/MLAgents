using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;
using System.Collections;

public class BallAgentFindingState5 : Agent
{
    Rigidbody rBody;
    public List<Transform> spawningAreas;
    public Transform target;
    public List<Transform> obsticles;

    private int _seconds;

    private float timeRemaining = 30f;
    private float time;
    private bool timerIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {       
        time = timeRemaining;
        timerIsRunning = true;

        // Reset agent
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0.5f, 0);

        int targetArea = Random.Range(0, spawningAreas.Count);
        target.localPosition = spawningAreas[targetArea].localPosition;

        int obscticleIndex = 0;
        for (int i = 0; i < spawningAreas.Count; i++)
        {
            if (i.Equals(targetArea))
                continue;

            obsticles[obscticleIndex].transform.localPosition = spawningAreas[i].localPosition;
            obscticleIndex++;
        }
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                time = 0;
                timerIsRunning = false;
                SetReward(-1.0f);
                EndEpisode();
            }
        }
    }

    //private IEnumerator time()
    //{
    //    yield return new WaitForSeconds(1);
    //    _seconds += 1;
    //    if (_seconds >= 120)
    //    {
    //        Debug.Log("reset by timer");           
    //        EndEpisode();
    //    }
    //    else
    //    {
    //        StartCoroutine(time());
    //    }
    //}



    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions & Agent velocity
        sensor.AddObservation(target.localPosition);
        foreach (var VARIABLE in obsticles)
        {
            sensor.AddObservation(VARIABLE.localPosition);
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
    }

    public override void Heuristic(float[] actionsOut)
    {
        //actionsOut[0] = Input.GetAxis("Vertical");
        //actionsOut[1] = Input.GetAxis("Horizontal");

        //Vector3 controlSignal = Vector3.zero;

        

        //if (actionsOut[0] == 1)
        //{
        //    controlSignal.x = 1;
        //}
        //else
        //{
        //    controlSignal.x = -1;
        //}

        //if (actionsOut[1] == 1)
        //{
        //    controlSignal.z = 1;
        //}
        //else
        //{
        //    controlSignal.z = -1;
        //}

        //Debug.Log(controlSignal.x + "   " + controlSignal.z);

        //rBody.AddForce(controlSignal * speed);

        //float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

        //// Reached target
        //if (distanceToTarget < 1.42f)
        //{
        //    SetReward(10.0f);
        //    EndEpisode();
        //}

        //foreach (var VARIABLE in obsticles)
        //{
        //    float distanceToObsticle = Vector3.Distance(this.transform.localPosition, VARIABLE.localPosition);
        //    if (distanceToObsticle < 2f)
        //    {
        //        SetReward(-1.0f);
        //        EndEpisode();
        //    }
        //}

        //// Fell of platform
        //if (this.transform.localPosition.y < -0.5f)
        //{
        //    EndEpisode();
        //}
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
