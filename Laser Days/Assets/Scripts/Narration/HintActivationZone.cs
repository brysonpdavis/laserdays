using System;
using UnityEngine;

public class HintActivationZone : MonoBehaviour
{
    [SerializeField] private float activeDist;
    [SerializeField] private bool activatedByProximity;
    [SerializeField] private bool activateByTrigger;
    private bool _activated = false;
    private Transform _playerTransform;

    private void Start()
    {
        _playerTransform = Toolbox.Instance.GetPlayer().transform;
    }

    private void Update()
    {
        if (!_activated && activatedByProximity)
        {
            if (Vector3.Distance(_playerTransform.position, transform.position) < activeDist)
            {
                GetComponent<HintObject>().Activate();
                _activated = true;
            }
        }
    }

    private void DrawGizmo(bool selected)
    {
        if(selected)
        {
            var green = new Color(0.1f, 0.8f, 0.4f, 0.2f);
            Gizmos.color = green;
            Gizmos.DrawSphere(transform.position, activeDist);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<HintObject>().Activate();
        }
    }

    public void OnDrawGizmos()
    {
        DrawGizmo(false);
    }
    public void OnDrawGizmosSelected()
    {
        DrawGizmo(true);
    }
}
