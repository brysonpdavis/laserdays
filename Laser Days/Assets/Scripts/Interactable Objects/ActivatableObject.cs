using UnityEngine;

public class ActivatableObject : MonoBehaviour, IActivatable
{
    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
