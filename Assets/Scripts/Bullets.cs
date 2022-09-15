using UnityEngine;

public class Bullets : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public bool edgeTraveler = true;
    public float speed;
    public float lifeTime;
    private Camera mainCam;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (edgeTraveler)
        {
            float sceneWidth = mainCam.orthographicSize * 2 * mainCam.aspect;
            float sceneHeight = mainCam.orthographicSize * 2;

            float sceneEdgeRight = sceneWidth / 2;
            float sceneEdgeTop = sceneHeight / 2;
            float sceneEdgeLeft = -sceneEdgeRight;
            float sceneEdgeBottom = -sceneEdgeTop;

            if (transform.position.x > sceneEdgeRight) transform.position = new Vector2(sceneEdgeLeft, transform.position.y);

            if (transform.position.y > sceneEdgeTop) transform.position = new Vector2(transform.position.x, sceneEdgeBottom);

            if (transform.position.x < sceneEdgeLeft) transform.position = new Vector2(sceneEdgeRight, transform.position.y);

            if (transform.position.y < sceneEdgeBottom) transform.position = new Vector2(transform.position.x, sceneEdgeTop);
        }
    }

    public void Fire(Vector2 _dir)
    {
        rb2d.AddForce(_dir * speed * 1000 * Time.deltaTime);

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
