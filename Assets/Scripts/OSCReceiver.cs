using System;
using UnityEngine;

public class OSCReceiver : MonoBehaviour
{
    const string BASE_PATH = "cv_reader";
    const string A_JACK_PATH = "a_jack";

    [SerializeField] private OSC _osc;
    [SerializeField] private AnalogInputManager _inputManager;

    private void Start()
    {
        for(int i = 0; i < 6; i++)
        {
            int jackNum = i;
            _osc.SetAddressHandler("/" + BASE_PATH + "/" + A_JACK_PATH, ReceiveAJacks);
        }
    }

    private void ReceiveAJacks(OscMessage msg)
    {
        //for(int i = 0; i < 6; i++)
        //{
        //    _inputManager.SetJackVal(i, msg.GetFloat(i));
        //}
    }
}

