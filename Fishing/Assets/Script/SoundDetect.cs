using UnityEngine;

public class SoundDetect : MonoBehaviour
{
    [SerializeField] private Transform soundPos;

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 3) {
            soundPos.position = new Vector3(soundPos.position.x, soundPos.position.y, other.transform.position.z);
        }
    }
}
