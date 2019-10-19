using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GoalSeekerDecision : MonoBehaviour, Decision
{
    /// <summary>
    /// Decides on actions to execute and passes them to AgentAction().
    /// Should mirror action space in AgentAction().
    /// </summary>
    /// <param name="vectorObs"></param>
    /// <param name="visualObs"></param>
    /// <param name="reward"></param>
    /// <param name="done"></param>
    /// <param name="memory"></param>
    /// <returns></returns>
    public float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        var actions = new float[3];
        // X movement.
        actions[0] = Random.Range(-1f, 1f);
        // Y movement.
        actions[1] = Random.Range(0f, 1f);
        /// Z movement.
        /// Forces positive z movement because the goal is always in that direction.
        actions[2] = Random.Range(0f, 1f);

        return actions;
    }

    public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        return default(List<float>);
    }
}
