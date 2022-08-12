using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraSettings : MonoBehaviour
{
    private Camera _camera;
    private Vector2 _worldViewportMax;
    private Vector2 _worldViewportMin;

    public Camera Camera
    {
        get { return _camera; }
    }
    public Vector2 WorldViewportMax
    {
        get
        {
            return _worldViewportMax;
        }
    }
    public Vector2 WorldViewportMin
    {
        get
        {
            return _worldViewportMin;
        }
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        if (_camera != null)
        {
            _worldViewportMax = _camera.ViewportToWorldPoint(_camera.rect.max);
            _worldViewportMin = _camera.ViewportToWorldPoint(_camera.rect.min);
        }
    }
}
