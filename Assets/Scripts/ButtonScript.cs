using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

    List<GameObject> stepping = new List<GameObject>();
    [SerializeField]
    public Exit exit;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!stepping.Contains(collision.gameObject))
        {
            stepping.Add(collision.gameObject);
        }
        if(stepping.Count > 0)
        {
            exit.open = true;
            exit.PlayAnim(true);
            Debug.Log("Door is open");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        stepping.Remove(collision.gameObject);
        if(stepping.Count == 0)
        {
            exit.open = false;
            //exit.PlayAnim(false);
            Debug.Log("Door is closed");
        }
    }
}
