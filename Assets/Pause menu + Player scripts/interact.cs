using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using UnityEngineInternal;

public class interact : MonoBehaviour
{    
    //reference na dr�en� p�edm�t, m�sto dr�en�, bod po��tku raycastu 
    public GameObject heldItem;
    public Transform RayCastPoint;
    public GameObject holdSpot;

    public LayerMask PickUpMask;
    public LayerMask InteractMask;
    
    //dosah hr��e
    public float HitRange = 5f;
    //dr�� hr�� n�co ?
    public bool holding;
    //raycast info
    public RaycastHit hit;

    public float tillFreez = 2f;

    void Update()
    {
        tillFreez -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(RayCastPoint.position, RayCastPoint.forward);
            if (!holding && Physics.Raycast(ray, out hit, HitRange))
            {
                Debug.DrawRay(RayCastPoint.position, RayCastPoint.forward * HitRange, Color.red);
                if (hit.collider.gameObject.TryGetComponent<storage>(out storage s))
                {
                    heldItem = Instantiate(s.FoodRef, holdSpot.transform.position, Quaternion.identity);
                    take();
                }
                
                else if (hit.collider.gameObject.TryGetComponent<dishes>(out dishes d))
                {
                    heldItem = Instantiate(d.dish, holdSpot.transform.position, Quaternion.identity);
                    take();
                }

                else if (hit.collider.gameObject.TryGetComponent<Table>(out Table table) && table.isPlaced)
                {
                    heldItem = table.placedItem;
                    table.placedItem.transform.SetParent(null);
                    take();
                    table.isPlaced = false;
                }

                else if (hit.collider.gameObject.TryGetComponent<cuttingBoard>(out cuttingBoard c))
                {
                    if (c.PlacedIngredienceB == null)
                    {
                        heldItem = c.PlacedIngredienceA;
                        c.isPlaced = false;
                        take();
                        //c.PlacedIngredienceA = null;
                    }

                    else if (c.PlacedIngredienceB != null) {

                        if (c.PlacedIngredienceA != null && c.PlacedIngredienceB != null)
                        {
                            heldItem = c.PlacedIngredienceA;
                            take();
                            c.PlacedIngredienceA = c.PlacedIngredienceB;
                            c.PlacedIngredienceB.gameObject.Equals(null);
                        }
                    }
                }
            }
            else if (holding && Physics.Raycast(ray, out hit, HitRange, InteractMask))
            {

                if(hit.collider.gameObject.TryGetComponent<Table>(out Table table))
                {
                    if (!table.isPlaced)
                    {
                        table.placedItem = heldItem;
                        table.isPlaced = true;
                        Place(table.placeSpot);
                    }

                    else if (table.placedItem.TryGetComponent<dish>(out dish Dish))
                    {
                        Dish.PlacedIngredience.Add(heldItem);
                        Place(Dish.dropSpot);
                        heldItem.transform.localPosition = Vector3.zero;
                        heldItem.TryGetComponent<Rigidbody>(out Rigidbody R);
                        Dish.placed = true;
                        //if (tillFreez == 0f)
                        //{
                        //    R.constraints = RigidbodyConstraints.FreezeAll;
                        //    tillFreez = 2f;
                        //}
                        
                    }

                }

                if (heldItem.GetComponent<ingred>())
                {
                    if (hit.collider.TryGetComponent<cuttingBoard>(out cuttingBoard c))
                    {
                        c.PlacedIngredienceA = heldItem;
                        c.isPlaced = true;
                        Place(c.PSpot);
                    }

                    if (hit.collider.TryGetComponent<trash>(out trash Trash))
                    {
                        Destroy(heldItem);
                        holding = false;
                    }
                }
            }
        }
        //else if (holding && Input.GetKeyDown(KeyCode.Q))
        //{
        //    drop();
        //}
    }
       
    //public void drop()
    //{
    //    heldItem.transform.SetParent(null);
    //    holding = false;
    //    heldItem.GetComponent<Rigidbody>().isKinematic = false;
    //}
    public void take()
    {
        holding = true;
        heldItem.transform.SetParent(holdSpot.transform, false);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.rotation = Quaternion.identity;
        heldItem.GetComponent<Rigidbody>().isKinematic = true;
    }
    public void Place(Transform transform)
    {
        heldItem.transform.SetParent(transform, true);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.GetComponent<Rigidbody>().isKinematic = false;
        holding = false;
    }
    //public void placeOnPlate()
    //{

    //}
}


