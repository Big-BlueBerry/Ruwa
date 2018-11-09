using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurveRenderer : MonoBehaviour
{
    public float width = 2;

    public Transform point1;
    public Transform point2;
    public Transform point3;
    public LineRenderer LineRenderer;
    public List<LineRenderer> LineRenderers = new List<LineRenderer>();

    public float Ratio;

    // Use this for initialization
    void Start () {
        for (var i = 0f; i < width; i += 0.01f)
        {
            var go = new GameObject();
            go.transform.SetParent(transform);
            var lr = go.AddComponent<LineRenderer>();

            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            LineRenderers.Add(lr);
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
	    var pointList = new List<Vector3>();

	    for (var ratio = 0f; ratio <= 1; ratio += 1f / 15)
	    {
	        var v1 = Vector3.Lerp(point1.position, point2.position, ratio);
	        var v2 = Vector3.Lerp(point2.position, point3.position, ratio);
            var res = Vector3.Lerp(v1, v2, ratio);

            pointList.Add(res);
        }

	    LineRenderer.positionCount = pointList.Count;
        LineRenderer.SetPositions(pointList.ToArray());

	    for (var i = 1; i < LineRenderers.Count + 1; i++)
	    {
	        var ps = pointList.Select(x => new Vector3(x.x + 0.01f * i, x.y, x.z)).ToArray();
	        LineRenderers[i - 1].positionCount = ps.Length;
            LineRenderers[i - 1].SetPositions(ps);
	    }
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(point1.position, point2.position);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(point2.position, point3.position);

        for (var ratio = 0f; ratio < 1; ratio += 1 / Ratio)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(
                Vector3.Lerp(point1.position, point2.position, ratio),
                Vector3.Lerp(point2.position, point3.position, ratio)
            );
        }
    }
}
