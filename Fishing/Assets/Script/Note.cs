using UnityEngine;

public class Note : MonoBehaviour
{

    public float fallSpeed;
    public RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition -= new Vector2(0, fallSpeed * Time.deltaTime);
    }
}
