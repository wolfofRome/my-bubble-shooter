using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Trajectory : MonoBehaviour
{
    [SerializeField] private int _trajectoryPointsCount;

    #region Cache components
    private CameraSettings _cameraSettings;
    public CameraSettings CameraSettings
    {
        get
        {
            if (_cameraSettings == null)
                _cameraSettings = Camera.main.GetComponent<CameraSettings>();
            return _cameraSettings;
        }
    }

    private LineRenderer _lineRenderer;
    public LineRenderer LineRenderer
    {
        get
        {
            if (_lineRenderer == null)
                _lineRenderer = GetComponent<LineRenderer>();
            return _lineRenderer;
        }
    }
    #endregion

    public void ShowTrajectory(Ball ball, Vector2 force)
    {
        Vector3[] points = new Vector3[_trajectoryPointsCount];
        LineRenderer.positionCount = _trajectoryPointsCount;

        float time = 0;

        points[0] = ball.RbBall.position;
        for(int i = 1; i < points.Length; i++)
        {
            Vector2 point = (Vector2)points[i - 1] + force + Mathf.Pow(time, 2) * Physics2D.gravity / 2;

            if(point.x >= CameraSettings.WorldViewportMax.x)
            {
                Vector2 minLinePoint = new Vector2(CameraSettings.WorldViewportMax.x, CameraSettings.WorldViewportMax.y * -1);
                point = Intersection(points[i - 1], point, minLinePoint, CameraSettings.WorldViewportMax);
                force = Vector2.Reflect(force, Vector2.left);
            }

            if(point.x <= CameraSettings.WorldViewportMin.x)
            {
                Vector2 minLinePoint = new Vector2(CameraSettings.WorldViewportMin.x, CameraSettings.WorldViewportMin.y * -1);
                point = Intersection(points[i - 1], point, minLinePoint, CameraSettings.WorldViewportMin);
                force = Vector2.Reflect(force, Vector2.right);
            }

            if (point.y >= CameraSettings.WorldViewportMax.y)
            {
                Vector2 minLinePoint = new Vector2(CameraSettings.WorldViewportMax.x * -1, CameraSettings.WorldViewportMax.y );
                point = Intersection(points[i - 1], point, minLinePoint, CameraSettings.WorldViewportMax);
                force = Vector2.Reflect(force, Vector2.down);
            }

            points[i] = point;
            time += Time.fixedDeltaTime;
        }

        LineRenderer.SetPositions(points);
    }

    private Vector2 Intersection(Vector2 linePointMin1, Vector2 linePointMax1, Vector2 linePointMin2, Vector2 linePointMax2)
    {
        float p = linePointMax1.x - linePointMin1.x, p1 = linePointMax2.x - linePointMin2.x;
        float q = linePointMax1.y - linePointMin1.y, q1 = linePointMax2.y - linePointMin2.y;

        float x = (linePointMin1.x * q * p1 - linePointMin2.x * q1 * p - linePointMin1.y * p * p1 + linePointMin2.y * p * p1) / (q * p1 - q1 * p);
        float y = (linePointMin1.y * p * q1 - linePointMin2.y * p1 * q - linePointMin1.x * q * q1 + linePointMin2.x * q * q1) / (p * q1 - p1 * q);

        return new Vector2(x, y);
    }
}
