using UnityEngine;

public class Exit : MonoBehaviour {

    [SerializeField]
    GameObject youWon;

    public bool open = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(open && collision.tag.Equals("Player"))
        {
            youWon.SetActive(true);
        }
    }
}
