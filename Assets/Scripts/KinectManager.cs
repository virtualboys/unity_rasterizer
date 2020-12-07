using UnityEngine;
using System.Collections;
using freenect;
using System;
using System.Threading;

public class KinectManager : MonoBehaviour
{
	private Kinect _kinect;
	private bool _isRunning;
	private Thread _updateThread;

    void Start()
    {
		// Get count
		int deviceCount = Kinect.DeviceCount;

		Debug.Log("Device count: " + deviceCount);

		Kinect.LogLevel = LoggingLevel.Debug;
		Kinect.Log += OnKinectLog;

		if (deviceCount > 0)
        {
			Debug.Log("Connecting to device 0");
			Connect(0);
        }
	}

    private void OnDestroy()
    {
        if(_isRunning)
        {
			Disconnect();
        }
    }

    private void Connect(int deviceID)
	{
		_isRunning = true;

		// Create instance
		_kinect = new Kinect(deviceID);

		// Open kinect
		_kinect.Open();

		// Setup image handlers
		_kinect.VideoCamera.DataReceived += HandleKinectVideoCameraDataReceived;
		_kinect.DepthCamera.DataReceived += HandleKinectDepthCameraDataReceived;

		// LED is set to none to start
		_kinect.LED.Color = LEDColor.BlinkRedYellow;
        //_kinect.VideoCamera.Mode = _kinect.VideoCamera.Modes[0];
        //_kinect.VideoCamera.Start();
        _kinect.DepthCamera.Mode = _kinect.DepthCamera.Modes[0];
        _kinect.DepthCamera.Start();

        //// Setup update thread
        //_updateThread = new Thread(delegate ()
        //{
        //	while (_isRunning)
        //	{
        //		try
        //		{
        //			// Update instance's status
        //			_kinect.UpdateStatus();

        //			// Let preview control render another frame
        //			//this.previewControl.Render();

        //			Kinect.ProcessEvents();
        //		}
        //		catch (ThreadInterruptedException e)
        //		{
        //			return;
        //		}
        //		catch (Exception ex)
        //		{

        //		}
        //	}
        //});

        //// Start update thread
        //_updateThread.Start();
    }

    private void OnKinectLog(object sender, LogEventArgs e)
    {
		Debug.Log(e.Message);
    }

    private void Update()
    {

        //// Update instance's status
        _kinect.UpdateStatus();

        //// Let preview control render another frame
        ////this.previewControl.Render();

        //Kinect.ProcessEvents();
    }

    private void Disconnect()
    {
		//_updateThread.Interrupt();

		_kinect.Close();
	}

    private void HandleKinectDepthCameraDataReceived(object sender, BaseCamera.DataReceivedEventArgs e)
    {
		Debug.Log("Got depth camera data: " + e.Data.Width + ", " + e.Data.Height);
    }

    private void HandleKinectVideoCameraDataReceived(object sender, BaseCamera.DataReceivedEventArgs e)
    {
		Debug.Log("Got video camera data: " + e.Data.Width + ", " + e.Data.Height);

    }
}
