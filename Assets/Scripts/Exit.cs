using UnityEngine;

public class Exit : MonoBehaviour {

    [SerializeField]
    GameObject youWon;
    public bool open;

    private Animator animator;

    public void PlayAnim(bool open)
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Open", open);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(open && collision.tag.Equals("Player"))
        {
            youWon.SetActive(true);
        }
    }
}
