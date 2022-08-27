using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;

public class Trajectory : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private LevelField _levelField;
    [SerializeField] private TrajectoryLine _trajectoryLinePrefab;
    [Min(1)]
    [SerializeField] private int _linesCount = 1;
    
    private List<TrajectoryLine> _trajectoryLines;
    
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
    #endregion

    private void Awake()
    {
        if (_levelField == null)
        {
            Debug.LogError("Level field object is not set!");
            return;
        }
            
        if (_trajectoryLinePrefab == null)
        {
            Debug.LogError("Trajectory prefab is not set!");
            return;
        }
            
        _trajectoryLines = new List<TrajectoryLine>();

        for (int i = 0; i < _linesCount; i++)
        {
            TrajectoryLine lineRenderer = Instantiate(_trajectoryLinePrefab, transform);
            _trajectoryLines.Add(lineRenderer);
        }

    }

    public void ShowTrajectory(Ball activeBall, Vector2 force)
    {
        float time = 0f;
        bool isPointIntersectBall = false;
        List<Vector3> points = new List<Vector3>();

        Vector3 currentPoint = activeBall.RbBall.position;
        
        foreach (TrajectoryLine trajectoryLine in _trajectoryLines)
        {
            trajectoryLine.UpdateLineScale();
            points.Add(currentPoint);

            for (int i = 0; i < trajectoryLine.MaxLinePointCount; i++)
            {
                if (isPointIntersectBall)
                    break;

                currentPoint = (Vector2)points[i] + force + Mathf.Pow(time, 2) * Physics2D.gravity / 2;

                HexCell pointCell = _levelField.FieldGrid.GetCellFromPosition(currentPoint);
                if (pointCell != null && pointCell.GetBall() != null)
                {
                    isPointIntersectBall = true;
                    break;
                }

                if (currentPoint.x >= CameraSettings.WorldViewportMax.x)
                {
                    Vector2 minLinePoint = new Vector2(CameraSettings.WorldViewportMax.x, CameraSettings.WorldViewportMax.y * -1);
                    currentPoint = Intersection(points[i], currentPoint, minLinePoint, CameraSettings.WorldViewportMax);
                    force = Vector2.Reflect(force, Vector2.left);
                    points.Add(currentPoint);
                    break;
                }

                if (currentPoint.x <= CameraSettings.WorldViewportMin.x)
                {
                    Vector2 minLinePoint = new Vector2(CameraSettings.WorldViewportMin.x, CameraSettings.WorldViewportMin.y * -1);
                    currentPoint = Intersection(points[i], currentPoint, minLinePoint, CameraSettings.WorldViewportMin);
                    force = Vector2.Reflect(force, Vector2.right);
                    points.Add(currentPoint);
                    break;
                }

                if (currentPoint.y >= CameraSettings.WorldViewportMax.y)
                {
                    Vector2 minLinePoint = new Vector2(CameraSettings.WorldViewportMax.x * -1, CameraSettings.WorldViewportMax.y);
                    currentPoint = Intersection(points[i], currentPoint, minLinePoint, CameraSettings.WorldViewportMax);
                    force = Vector2.Reflect(force, Vector2.down);
                    points.Add(currentPoint);
                    break;
                }

                points.Add(currentPoint);

                time += Time.fixedDeltaTime;
            }

            trajectoryLine.GenerateTrajectoryLine(points);
            points.Clear();
        }
    }

    public void HideTrajectory(Ball activeBall)
    {
        foreach (TrajectoryLine trajectoryLine in _trajectoryLines)
        {
            trajectoryLine.ClearTrajectoryLinePoints();
        }
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
