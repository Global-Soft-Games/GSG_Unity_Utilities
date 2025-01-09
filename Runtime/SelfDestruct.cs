using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField]
    private float delay = 3f;

    [SerializeField]
    private bool destroyOnStart = true;

    public void InvokeDestroy()
    {
        // Call the DestroyObject method after the specified delay
        Invoke("DestroyObject", delay);
    }

    private void Start()
    {
        if (destroyOnStart)
        {
            InvokeDestroy();
        }
    }    

    private void DestroyObject()
    {
        // Destroy the game object this script is attached to
        Destroy(gameObject);
    }
}