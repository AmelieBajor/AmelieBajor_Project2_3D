using UnityEngine;

public class ItemScript : MonoBehaviour
{

    private Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PickUp(Transform parent, Vector3 pos)
    {
        rb.isKinematic = true;
        transform.SetParent(parent);
        transform.position = pos;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        //position
        //set parent
    }

    public void Throw(float force, Vector3 direction)
    {
        rb.isKinematic = false;
        transform.SetParent(null);
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

}
