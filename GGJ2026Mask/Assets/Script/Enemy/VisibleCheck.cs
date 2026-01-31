using UnityEngine;

public class VisibleCheck : MonoBehaviour
{
    Camera m_MainCamera;
    bool m_Visible;

    void Start()
    {
        m_MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //IsObscured might not be a good idea, only checks spawn center, not whole enemy size.
        if (IsInCameraView() && !IsObscured())
        {
            m_Visible = true;
        }
        else
        {
            m_Visible = false;
        }
    }

    bool IsObscured()
    {
        Ray ray = new Ray( m_MainCamera.transform.position, (transform.position - m_MainCamera.transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the first object hit is the target object
            if (hit.transform == transform)
            {
                return false; // Visible and unobstructed
            }
        }
        return true;
    }

    bool IsInCameraView()
    {
        Vector3 viewportPoint = m_MainCamera.WorldToViewportPoint(transform.position);

        // Check if point is within the viewport (0,0 to 1,1) and in front of the camera (Z > 0)
        bool inView = viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                      viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
                      viewportPoint.z > 0;

        return inView;
    }

}
