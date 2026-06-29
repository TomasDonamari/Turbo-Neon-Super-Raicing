using UnityEngine;

public class Circuit : MonoBehaviour
{
    public GameObject[] wayPoints;

    void OnDrawGizmos()
    {
        DrawGizmos(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    private void DrawGizmos(bool selected)
    {
        if (selected == false) return;
        if (wayPoints.Length > 1)
        {
            Vector3 previous = wayPoints[0].transform.position;
            for (int i = 1; i < wayPoints.Length; i++)
            {
                Vector3 next = wayPoints[i].transform.position;
                Gizmos.DrawLine(previous, next);
                previous = next;
            }
            Gizmos.DrawLine(previous, wayPoints[0].transform.position);
        }
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
