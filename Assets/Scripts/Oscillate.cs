using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour
{
    [SerializeField] private float _period;
    [SerializeField] private float _amplitude;
    [SerializeField] private Vector3 _axis = Vector3.up;
    [SerializeField] private float _randomness;

    private Vector3 _startPos;
    //private float _randOffset;

    private float _time;

    void Start()
    {
        
    }
    private void OnEnable()
    {
        _startPos = transform.position;
        _period = _period + _period * Random.Range(-_randomness, _randomness);
        _amplitude = _amplitude + _amplitude * Random.Range(-_randomness, _randomness);
        _time = Random.Range(-_period, _period);
    }

    void Update()
    {
        _time += Time.deltaTime;
        transform.position = _startPos + _axis * Mathf.Sin(_time / _period) * _amplitude;
    }
}
