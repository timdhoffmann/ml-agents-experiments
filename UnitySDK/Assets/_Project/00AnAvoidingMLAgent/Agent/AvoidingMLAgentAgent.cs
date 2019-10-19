using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AvoidingMLAgentAgent : Agent
{
    private readonly float _force = 5.0f;
    private Vector2 _startPosition = Vector2.zero;
    private Rigidbody2D _rigidBody2D = null;
    private bool _hasCrashed = false;

    /// <summary>
    /// Only initializes when called from the ml-agents system,
	/// not every time the game runs.
    /// </summary>
    public override void InitializeAgent()
    {
        _startPosition = transform.position;

        _rigidBody2D = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(_rigidBody2D);
    }

    public override void CollectObservations()
    {
        // Define observations.

        var raycastDistance = 20.0f;

        var distanceUp = 100f;
        // Raycasts up.
        // TODO: Would transform.up instead of Vector2.up make any difference?
        RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, 1, Vector2.up, raycastDistance);

        // TODO: Can an explicit check for != null lead different results?
        if (hit2D.collider != null)
        {
            distanceUp = hit2D.distance;
        }

        var distanceDown = 100f;
        // Raycasts down.
        hit2D = Physics2D.CircleCast(transform.position, 1, Vector2.down, raycastDistance);
        if (hit2D.collider != null)
        {
            distanceDown = hit2D.distance;
        }

        var distanceLeft = 100f;
        // Raycasts left.
        hit2D = Physics2D.CircleCast(transform.position, 1, Vector2.left, raycastDistance);
        if (hit2D.collider != null)
        {
            distanceLeft = hit2D.distance;
        }

        var distanceRight = 100f;
        // Raycasts right.
        hit2D = Physics2D.CircleCast(transform.position, 1, Vector2.right, raycastDistance);
        if (hit2D.collider != null)
        {
            distanceRight = hit2D.distance;
        }

        // Debug.DrawRay(transform.position, Vector2.up * raycastDistance, Color.red);
        // Debug.DrawRay(transform.position, Vector2.down * raycastDistance, Color.red);
        // Debug.DrawRay(transform.position, Vector2.left * raycastDistance, Color.red);
        // Debug.DrawRay(transform.position, Vector2.right * raycastDistance, Color.red);


        // Submit observations.

        /// [IMPORTANT!] 
        /// Submitting observations in a single list seems to lead
        /// to undesired results!!!
        //var state = new List<float>()
        //{
        //    distanceUp,
        //    distanceDown,
        //    distanceLeft,
        //    distanceRight,
        //    _rigidBody2D.velocity.x,
        //    _rigidBody2D.velocity.y,
        //};
        //AddVectorObs(state);

        /// Therefore, submitting each observation separately.
        AddVectorObs(distanceUp);
        AddVectorObs(distanceDown);
        AddVectorObs(distanceLeft);
        AddVectorObs(distanceRight);
        AddVectorObs(_rigidBody2D.velocity.x);
        AddVectorObs(_rigidBody2D.velocity.y);
    }

    /// <summary>
    /// Reports collisions of collider & Rigidbody on the same GameObject.
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        _hasCrashed = true;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = (int)vectorAction[0];

        // Assert.IsNotNull(brain);

        /// Discrete is in this case only included for practice.
        /// Only use discrete for discrete choices (eg. which weapon?, which stage?).
        /// Here, the amount matters, which makes continuous a better fit.
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            switch (action)
            {
                case 0:
                    _rigidBody2D.AddForce(Vector2.up * _force);
                    break;
                case 1:
                    _rigidBody2D.AddForce(Vector2.down * _force);
                    break;
                case 2:
                    _rigidBody2D.AddForce(Vector2.left * _force);
                    break;
                case 3:
                    _rigidBody2D.AddForce(Vector2.right * _force);
                    break;
                default:
                    _rigidBody2D.AddForce(Vector2.zero);
                    break;
            }
            if (!IsDone())
            {
                SetReward(0.1f);
            }
        }
        /// <summary>
        /// Continuous action type.
		/// Allows for applying different amounts.
        /// </summary>
        /// <value></value>
        else
        {
            _rigidBody2D.AddForce(Vector2.up * _force * vectorAction[0]);
            _rigidBody2D.AddForce(Vector2.down * _force * vectorAction[1]);
            _rigidBody2D.AddForce(Vector2.left * _force * vectorAction[2]);
            _rigidBody2D.AddForce(Vector2.right * _force * vectorAction[3]);

            if (!IsDone())
            {
                SetReward(0.1f);
            }
        }
        if (_hasCrashed)
        {
            Done();
            SetReward(-1.0f);
        }
    }

    /// <summary>
    /// Called when IsDone() reports true or
    /// when max steps is reached.
    /// </summary>
    public override void AgentReset()
    {
        transform.position = _startPosition;
        _rigidBody2D.velocity = Vector2.zero;
        _hasCrashed = false;
    }
}
