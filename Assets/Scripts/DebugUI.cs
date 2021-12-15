using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _fpsCounter;
    [SerializeField] private GameObject _ui;

    private float _hudRefreshRate = 1.0f;
    private float _timer;
    private int _numFrames;
    private float _startTime;

    void Start()
    {
        _startTime = Time.unscaledTime;
    }

    public void SetUIActive(bool active)
    {
        _ui.SetActive(active);
    }

    void Update()
    {
        _numFrames++;

        if (Time.unscaledTime > _timer)
        {
            int fps = (int)(_numFrames / (Time.unscaledTime - _startTime));
            if(_fpsCounter != null)
            {
                _fpsCounter.text = "FPS: " + fps;
            }
            Debug.Log("FPS: " + fps);
            _timer = Time.unscaledTime + _hudRefreshRate;

            _startTime = Time.unscaledTime;
            _numFrames = 0;
        }
    }


}
