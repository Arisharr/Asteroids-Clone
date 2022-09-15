using UnityEngine;

public class Asteroids : MonoBehaviour
{
    [SerializeField] Sprite[] asteroidSprites;
    [SerializeField] [Range(.1f, 1f)] float asteroidLevel = 0.1f;
    [SerializeField] bool randomizeLevel = false;
    [SerializeField] float speed;
    [SerializeField] float lifeTime;

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private float minLevel = .1f;
    private float maxLevel = 1f;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (randomizeLevel)
        {
            asteroidLevel = Random.Range(.1f, 1f);
        }
    }

    private void Start()
    {
        spriteRenderer.sprite = asteroidSprites[Random.Range(0, asteroidSprites.Length)];

        transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);

        transform.localScale = Vector3.one * asteroidLevel;
        rb2d.mass = asteroidLevel * 2f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Player"))
        {
            if (this.asteroidLevel *.5f >= minLevel)
            {
                Split();
                Split();
            }
            GameManager.Instance.DestroyAsteroid(this);
            Destroy(gameObject);
        }
    }

    public void SetOnFire(Vector2 _dir)
    {
        rb2d.AddForce(_dir * speed);
        Destroy(gameObject, lifeTime);
    }

    private void Split()
    {
        Vector2 splitPosition = this.transform.position;
        splitPosition += Random.insideUnitCircle * .5f;

        Asteroids half = Instantiate(this, splitPosition, transform.rotation);
        half.asteroidLevel = this.asteroidLevel * .5f;
        half.SetOnFire(Random.insideUnitCircle.normalized * this.speed);
    }

}
