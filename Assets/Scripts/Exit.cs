using UnityEngine;

public class Exit : MonoBehaviour {

    [SerializeField]
    GameObject youWon;
    public bool open;

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        
    }
    public void PlayAnim(bool open)
    {
        
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
