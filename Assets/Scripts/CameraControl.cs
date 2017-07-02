using UnityEngine;

public class CameraControl : MonoBehaviour {

    [SerializeField]
    GameObject player;

    Vector3 offset;
    Vector3 defaultOffset = new Vector3(12.0f, -1.0f, -10.0f);

    // Use this for initialization
    void Start() {
        if (player != null)
        {
            offset = transform.position - player.transform.position;
        }
        else
        {
            offset = defaultOffset;
        }
    }

    // Update is called once per frame
    void Update() {
        if (player != null)
        {
            Vector3 curPos = player.transform.position + offset;
            curPos.y = player.transform.position.y;
            
            transform.position = curPos;
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
}
