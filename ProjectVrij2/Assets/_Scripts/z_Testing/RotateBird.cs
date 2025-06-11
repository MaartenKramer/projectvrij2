using UnityEngine;

public class RotateBird : MonoBehaviour
{
    [SerializeField] private BirdBehaviour bird;
    [SerializeField] private float turnSpeed;

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = bird.TotalForce.normalized;
        //transform.rotation = Quaternion.LookRotation(direction);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

    }
}
