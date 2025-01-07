using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class taking : MonoBehaviour
{    
    //reference na dr�en� p�edm�t, m�sto dr�en�, bod po��tku raycastu 
    public GameObject heldItem;
    public Transform RayCastPoint;
    public GameObject spot;
    
    //dosah hr��e
    public float HitRange = 5f;
    //dr�� hr�� n�co ?
    public bool holding;
    //raycast info
    public RaycastHit hit;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Ray ray = new Ray(RayCastPoint.position, RayCastPoint.forward);
            if (!holding && Physics.Raycast(ray, out hit, HitRange))
            {
                Debug.DrawRay(RayCastPoint.position, RayCastPoint.forward * HitRange, Color.red);
                //Rigidbody rb;
                if (hit.collider.gameObject.TryGetComponent<storage>(out storage s))
                {
                    heldItem = Instantiate(s.FoodRef, spot.transform.position, Quaternion.identity);
                    take();
                }
                if (hit.collider.GetComponent<ingred>())
                {
                    heldItem = hit.collider.gameObject;
                    take();
                }
                
            }
            else if (holding && Physics.Raycast(ray, out hit, HitRange))
            {
                if (hit.collider.gameObject.CompareTag("pan"))
                {
                    //heldItem.transform.position = hit.collider.transform.position;
                    heldItem.transform.SetParent(null);
                    heldItem.transform.SetParent(hit.collider.transform, true);
                    heldItem.transform.position = hit.collider.transform.position;
                    place();
                }
            }
        }
        else if (holding && Input.GetKeyDown(KeyCode.Q))
        {
            drop();
        }
        
        
    }
    public void StorageTake(RaycastHit hit)
    {
        
    }

    public void drop()
    {
        heldItem.transform.SetParent(null);
        holding = false;
        heldItem.GetComponent<Rigidbody>().isKinematic = false;

    }
    public void take()
    {
        holding = true;
        heldItem.transform.position = Vector3.zero;
        heldItem.transform.rotation = Quaternion.identity;
        heldItem.transform.SetParent(spot.transform, false); 
        heldItem.GetComponent<Rigidbody>().isKinematic = true;
    }
    public void place()
    {
        
        holding = false;
        
    }
}
