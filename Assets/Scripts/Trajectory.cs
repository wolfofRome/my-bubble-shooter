using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] private int _trajectoryPointsCount;
    private LineRenderer _lineRenderer;
    private Camera _cameraMain;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _cameraMain = Camera.main;
    }

    public void ShowTrajectory(Ball ball, Vector2 force)
    {
        Vector3[] points = new Vector3[_trajectoryPointsCount];
        _lineRenderer.positionCount = _trajectoryPointsCount;

        float time = 0;

        Vector2 prevPoint = ball.RbBall.position;
        points[0] = prevPoint;
        for(int i = 1; i < points.Length; i++)
        {
            if (_cameraMain.WorldToViewportPoint(prevPoint + ball.BallPositionOffset).x >= _cameraMain.rect.max.x ||
                _cameraMain.WorldToViewportPoint(prevPoint - ball.BallPositionOffset).x <= _cameraMain.rect.min.x)
            {
                force.x *= -1;
            }

            if (_cameraMain.WorldToViewportPoint(prevPoint + ball.BallPositionOffset).y >= _cameraMain.rect.max.y)
            {
                force.y *= -1;
            }
                

            points[i] = prevPoint + force + Mathf.Pow(time, 2) * Physics2D.gravity / 2;

            time += Time.fixedDeltaTime;
            prevPoint = points[i];
        }

        _lineRenderer.SetPositions(points);
    }
}
