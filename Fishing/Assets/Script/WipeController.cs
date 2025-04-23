using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WipeController : MonoBehaviour
{
    private Animator animator;
    private Image image;

    public float circleSize = 1.2f;
    private bool intoWater = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(intoWater) {
            image.materialForRendering.SetFloat("_Circle_Size", circleSize);
        }
    }

    public IEnumerator CircleIn() {
        animator.SetTrigger("in");
        yield return null;
        intoWater = true;
        float len = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(len);
    }

    public IEnumerator CircleOut() {
        animator.SetTrigger("out");
        yield return null;
        float len = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);
        intoWater = false;
    }
}
