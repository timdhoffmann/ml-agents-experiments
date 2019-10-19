using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab = null;
    [SerializeField] private float _spawnStart = 1f;
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private float _force = 100f;
    [SerializeField] private float _lifeTime = 10f;

    // Use this for initialization
    private void Start()
    {
        InvokeRepeating("Spawn", _spawnStart, _spawnInterval);
    }

    // Update is called once per frame
    private void Spawn()
    {
        GameObject obstacle = Instantiate(_prefab, transform.position, Quaternion.identity);
        var rigidbody = obstacle.GetComponent<Rigidbody2D>();
        rigidbody.AddForce(Vector2.left * _force);
        Destroy(obstacle, _lifeTime);
    }
}
