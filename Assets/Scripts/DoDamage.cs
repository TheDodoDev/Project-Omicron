using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    [SerializeField] int damage;
    public bool isActive;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isActive)
        {
            other.gameObject.transform.parent.GetComponent<PlayerControl>().TakeDamage(damage);
        }
    }
}
