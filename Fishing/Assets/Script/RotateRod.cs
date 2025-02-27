using System.Collections;
using UnityEngine;

public class RotateRod : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] private Transform rod;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRotate() {
        StopAllCoroutines();
        StartCoroutine(RotateObject());
    }
    
    IEnumerator RotateObject() {
        yield return new WaitForEndOfFrame();
        rod = transform.GetChild(0);
        while(rod != null) {
            rod.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
