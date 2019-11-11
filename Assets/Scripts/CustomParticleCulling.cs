using UnityEngine;

public class CustomParticleCulling : MonoBehaviour
{
    public float cullingRadius = 10;
    public ParticleSystem target;

    CullingGroup m_CullingGroup;

    void OnEnable()
    {

        if (m_CullingGroup == null)
        {
            m_CullingGroup = new CullingGroup();
            m_CullingGroup.targetCamera = Camera.main;
            m_CullingGroup.SetBoundingSpheres(new[] { new BoundingSphere(transform.position, cullingRadius) });
            m_CullingGroup.SetBoundingSphereCount(1);
            m_CullingGroup.onStateChanged += OnStateChanged;

            // We need to start in a culled state
            Cull(m_CullingGroup.IsVisible(0));
        }

        m_CullingGroup.enabled = true;
    }

    void OnDisable()
    {
        if (m_CullingGroup != null)
            m_CullingGroup.enabled = false;

        target.Play(true);
    }

    void OnDestroy()
    {
        if (m_CullingGroup != null)
            m_CullingGroup.Dispose();
    }

    void OnStateChanged(CullingGroupEvent sphere)
    {
        Cull(sphere.isVisible);
        print(sphere.index);
        print("Has become invisible: " + sphere.hasBecomeInvisible.ToString());
        Debug.Break();
    }

    void Cull(bool visible)
    {
        print("Visibility: " + visible);
        if (visible)
        {
            // We could simulate forward a little here to hide that the system was not updated off-screen.
            target.Play(true);

        }
        else
        {
            target.Pause(true);
            target.Clear(true);
        }
    }

    void OnDrawGizmos()
    {
        if (enabled)
        {
            // Draw gizmos to show the culling sphere.
            Color col = Color.yellow;
            if (m_CullingGroup != null && !m_CullingGroup.IsVisible(0))
                col = Color.gray;

            Gizmos.color = col;
            Gizmos.DrawWireSphere(transform.position, cullingRadius);
        }
    }
}
