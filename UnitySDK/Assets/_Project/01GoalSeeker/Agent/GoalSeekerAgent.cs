using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// (Position) code is kept relative to the TrainingArea, so it can be used
/// with multiple areas in the same scene.
/// </summary>
public class GoalSeekerAgent : Agent
{
    [SerializeField] private GameObject _goal = null;
    [SerializeField] private GameObject _trainingArea = null;
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _jumpSpeed = 50f;
    [SerializeField] private GameObject _wall = null;
    [SerializeField] private GameObject _floor = null;
    private bool _isJumping = false;
    private const float _maxHorizontalPositionValue = 5f;
    private float _lastDistanceToGoal = 0f;

    private Rigidbody _rigidbody = null;
    private Vector3 _startPosition = Vector3.zero;
    private Vector3 _goalPosition = Vector3.zero;
    private Color _standardFloorColor;

    public override void InitializeAgent()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _startPosition = transform.position;
        _goalPosition = _goal.transform.position;
        _standardFloorColor = _floor.GetComponent<MeshRenderer>().material.color;
        _lastDistanceToGoal = Vector3.Distance(transform.position, _goalPosition);
    }

    public override void CollectObservations()
    {
        var observations = new List<float>()
        {
            _isJumping ? 1 : 0,
            // Agent relative position.
            (transform.position.x - _trainingArea.transform.position.x) / _maxHorizontalPositionValue,
            (transform.position.z - _trainingArea.transform.position.z) / _maxHorizontalPositionValue,
            // Agent velocities.
            _rigidbody.velocity.x / _moveSpeed,
            _rigidbody.velocity.y / _jumpSpeed,
            _rigidbody.velocity.z / _moveSpeed,
            // Goal relative position.
            (_goal.transform.position.x - _trainingArea.transform.position.x) / _maxHorizontalPositionValue,
            (_goal.transform.position.z - _trainingArea.transform.position.z) / _maxHorizontalPositionValue,
            // Wall relative height.
            _wall.transform.position.y - _trainingArea.transform.position.y,
        };
        AddVectorObs(observations);
    }

    /// <summary>
    /// Continuous actions for each step.
    /// </summary>
    /// <param name="vectorAction"></param>
    /// <param name="textAction"></param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // TODO: Check use of SetReward() vs. AddReward()!
        // Time penalty.
        SetReward(-0.01f);

        // Distance reduction reward.
        if (Vector3.Distance(transform.position, _goalPosition) < _lastDistanceToGoal)
        {
            SetReward(0.005f);
        }

        Vector3 controlForce = Vector3.zero;

        controlForce.x = vectorAction[0];
        // Jumping.
        controlForce.y = vectorAction[1];
        // TODO: Force positive z movement because the goal is always in that direction?
        controlForce.z = vectorAction[2];

        if (_isJumping)
        {
            // Prevents double jump and reduced air manouvering.
            controlForce.y = 0f;
            controlForce.x *= 0.5f;
            controlForce.z *= 0.5f;
        }
        else if (controlForce.y > 0f)
        {
            _isJumping = true;
        }

        _rigidbody.AddForce(new Vector3(controlForce.x * _moveSpeed, controlForce.y * _jumpSpeed, controlForce.z * _moveSpeed));
    }

    /// <summary>
    /// Called when Done().
    /// </summary>
    public override void AgentReset()
    {
        transform.position = _startPosition + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        _rigidbody.velocity = Vector3.zero;
        _goal.transform.position = _goalPosition + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(1f, 2f));
        _isJumping = false;
        StartCoroutine(ColorizeFloor(_standardFloorColor, 0.2f));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Finish")
        {
            StartCoroutine(ColorizeFloor(Color.green, 0f));
            SetReward(1f);
            Done();
        }
        else if (other.gameObject.tag == "Dead")
        {
            StartCoroutine(ColorizeFloor(Color.red, 0f));
            SetReward(-1f);
            Done();
        }
        else if (other.gameObject.tag == "Walkable")
        {
            _isJumping = false;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Walkable")
        {
            _isJumping = false;
        }
    }

    private IEnumerator ColorizeFloor(Color color, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        _floor.GetComponent<MeshRenderer>().material.color = color;
    }
}
