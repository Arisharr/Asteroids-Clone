using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private Rigidbody2D rb2d;
    private Camera mainCam;
    private AudioSource audioSource;
    private float lastShoot = 0f;
    public bool isAlive = true;
    public float fireRate = .5f;
    public float accelerationSpeed;
    public float touchAccelerationSpeed;
    public float rotationSpeed;
    public float touchRotationSpeed;
    public float startCooldown = 3f;
    public bool inCooldown = false;
    public float explosionArea;
    public float explosionFore;
    [Space]
    [SerializeField] private Bullets bulletPrefab;
    [SerializeField] private ParticleSystem exhaustParticle;
    [SerializeField] private GameObject explodedPlayer;
    [SerializeField] private AudioClip deahtClip;
    [SerializeField] public ParticleSystem idleParticle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        mainCam = Camera.main;

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {

        if (startCooldown > 0)
        {
            inCooldown = true;
            startCooldown -= Time.deltaTime;
        }
        else
        {
            inCooldown = false;
        }

        if (isAlive)
        {
            Movements();
            EdgePortal();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid") && !inCooldown && isAlive)
        {
            Die();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, explosionArea);
    }

    private void Movements()
    {
        bool accelerating, decelerating;
        float direction;

        accelerating = Input.GetAxis("Vertical") > 0;
        decelerating = Input.GetAxis("Vertical") < 0;
        direction = Input.GetAxis("Horizontal") * -1;
        

        //PC Input
        if (accelerating)
        {
            rb2d.AddForce(this.transform.up * accelerationSpeed * Time.deltaTime);

            exhaustParticle.Emit(1);
            var eMain = exhaustParticle.main;
            eMain.simulationSpeed = rb2d.velocity.sqrMagnitude * 10f;
        }

        if (decelerating)
        {
            rb2d.AddForce(this.transform.up * -accelerationSpeed * Time.deltaTime);
        }

        if (direction != 0)
        {
            rb2d.AddTorque(direction * rotationSpeed * Time.deltaTime);
        }

        if (Input.GetAxis("Jump") > 0)
        {
            if (Time.time > fireRate + lastShoot)
            {
                Shoot();
                lastShoot = Time.time;
            }
        }

        //Mobile Input
        if (Input.touchCount > 0)
        {
            Touch finger1 = Input.GetTouch(0);

            if (finger1.deltaPosition.y > 0)
            {
                rb2d.AddForce(this.transform.up * touchAccelerationSpeed * Time.deltaTime);

                exhaustParticle.Emit(1);
                var eMain = exhaustParticle.main;
                eMain.simulationSpeed = rb2d.velocity.sqrMagnitude * 10f;
            }
            if (finger1.deltaPosition.y < 0)
            {
                rb2d.AddForce(this.transform.up * -touchAccelerationSpeed * Time.deltaTime);
            }
            if (finger1.deltaPosition.x < 0)
            {
                rb2d.AddTorque(1 * touchRotationSpeed * Time.deltaTime);
            }
            if (finger1.deltaPosition.x > 0)
            {
                rb2d.AddTorque(-1 * touchRotationSpeed * Time.deltaTime);
            }

            if (Input.touchCount>1)
            {
                if (Time.time > fireRate + lastShoot)
                {
                    Shoot();
                    lastShoot = Time.time;
                }
            }
        }

    }

    private void EdgePortal()
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

    public void Die()
    {
        isAlive = false;
        rb2d.velocity = Vector3.zero;
        rb2d.angularVelocity = 0f;
        GameManager.Instance.Respawn();
        audioSource.PlayOneShot(deahtClip);
        Explode();
    }

    private void Shoot()
    {
        Bullets _bullet = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation);

        _bullet.Fire(this.transform.up);
    }

    private void Explode()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        idleParticle.gameObject.SetActive(false);

        GameObject explodingEffect = Instantiate(explodedPlayer, this.transform);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionArea);

        foreach (Collider2D item in colliders)
        {
            Vector2 dir = item.transform.position - transform.position;
            if(item.GetComponent<Rigidbody2D>()) item.GetComponent<Rigidbody2D>().AddForce(dir * explosionFore);
        }

        Destroy(explodingEffect, 2f);
    }
}
