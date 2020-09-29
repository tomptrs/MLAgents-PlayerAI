using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerAgent : Agent
{
    [Tooltip("Object the agent searches for")]
    public Transform Target;
    
    [Tooltip("how fast the agent moves forward")]
    public float forceMultiplier = 20;

    [Tooltip("How fast the agent turns")]
    public float turnSpeed = 180f;

    Rigidbody rBody;

    RayPerceptionSensorComponent3D rayPerception;



    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        rayPerception = GetComponent<RayPerceptionSensorComponent3D>();
    }

    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            this.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }

        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.Range(-4,4),
                                           0.5f,
                                           Random.Range(-4, 4));

      
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
        sensor.AddObservation(rayPerception);

    }   



    
    public override void OnActionReceived(float[] vectorAction)
    {
        // Convert the first action to forward movement
        float forwardAmount = vectorAction[0];

        // Convert the second action to turning left or right
        float turnAmount = 0f;
        if (vectorAction[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (vectorAction[1] == 2f)
        {
            turnAmount = 1f;
        }

        // Apply movement
        rBody.MovePosition(transform.position + transform.forward * forwardAmount * forceMultiplier * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        if (distanceToTarget < 1.4f)
        {
            SetReward(1.0f);
            EndEpisode();
        }


        // Apply a tiny negative reward every step to encourage action
        if (this.MaxStep > 0)
            AddReward(-1f / MaxStep);


        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }

        //// Actions, size = 2
        //Vector3 controlSignal = Vector3.zero;
        ////TODO: play with the actions!!
        //controlSignal.x = vectorAction[0];
        //controlSignal.z = vectorAction[1]; 
        //transform.rotation = Quaternion.Euler(0f, vectorAction[2] * 180, 0f);
        //// rBody.AddForce(controlSignal * forceMultiplier, ForceMode.Acceleration);
        //rBody.AddForce(transform.forward * forceMultiplier);

        //// Rewards
        //float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        //// Reached target
        //if (distanceToTarget < 1.4f)
        //{
        //    SetReward(1.0f);
        //    EndEpisode();
        //}

        //// Fell off platform
        //if (this.transform.localPosition.y < 0)
        //{
        //    EndEpisode();
        //}
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}
