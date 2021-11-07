using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWobble : MonoBehaviour
{
    [SerializeField] private float _period;
    [SerializeField] private float _amt;

    private Vector3 _startScale;

    private void Start()
    {
        _startScale = transform.localScale;
    }

    void Update()
    {
        //transform.localScale = _startScale * 
    }
}
