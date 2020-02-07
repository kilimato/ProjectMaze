using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Rigidbody2D rb2D;
    private BoxCollider2D boxCollider;
    private List<Rigidbody2D> lineQueue;

    public float moveTime = 0.05f;
    private float inverseMoveTime;

    private Vector2 previousPosition;

    public GameObject visitedTile;
    public int moveX = 0;
    public int moveY = 0;
    private bool isCoroutineStarted = false;

    // Start is called before the first frame update
    void Start()
    {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        lineQueue = new List<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
       
    }

    // Update is called once per frame
    void Update()
    {

        moveX = (int)Input.GetAxisRaw("Horizontal");
        moveY = (int)Input.GetAxisRaw("Vertical");

        //Estetään diagonaalinen liikkuminen.
        if (moveX != 0) moveY = 0;

        if (moveX != 0 || moveY != 0)
        {

            //Muuten alirutiineja tulee lisää joka framella, jolla liikkumisnappi on pohjassa.
            if (isCoroutineStarted == false)
            {
                //Perusideat Unityn 4 vuotta vanhasta roguelike tutoriaalista.
                if(Move(moveX, moveY))
                {

                    if (lineQueue.Count != 0)
                    {
                        lineQueue[0].MovePosition(previousPosition);
                        lineQueue[0].GetComponent<PreviousPosition>().previousPosition = previousPosition;
                    }

                    for (int i = lineQueue.Count - 1; i > 0; i--)
                    {
                        lineQueue[i].GetComponent<PreviousPosition>().previousPosition = lineQueue[i].position;
                        lineQueue[i].MovePosition(lineQueue[i - 1].position);
                    }
                }
            }

        }

        if(Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    /// <summary>
    /// Unityn Coroutine, joka tuottaa sulavan liikkeen end-koordinaatin suuntaan.
    /// Myöhemmin ehkä järkevämpää tutkia tile-kohtaisesti, eikä Vector3 koordinaatti-kohtaisesti.
    /// </summary>
    /// <param name="end">Liikkumisen päätepiste.</param>
    /// <returns></returns>
    protected IEnumerator SmoothMovement(Rigidbody2D movingObject, Vector3 start, Vector3 end)
    {

        float remainingDistance = (start - end).sqrMagnitude;
        isCoroutineStarted = true;
        while (remainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(movingObject.position, end, inverseMoveTime * Time.deltaTime);
            movingObject.MovePosition(newPosition);
            remainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        Instantiate(visitedTile, start, Quaternion.identity);
        isCoroutineStarted = false;
        
    }


    /// <summary>
    /// Metodi, joka suorittaa liikkumisen,  ja myöhemmin mahdollisesti myös tarkistaa, että
    /// voidaanko ylipäätään liikkua haluttuun suuntaan.
    /// </summary>
    /// <param name="xDir">Liikkumissuunta x-akselin suhteen.</param>
    /// <param name="yDir">Liikkumissuunta y-akselin suhteen.</param>
    /// <returns>Palauttaa false, jos ei voida liikkua. Muuten true.</returns>
    protected bool Move (int xDir, int yDir)
    {

        Vector2 start = transform.position;
        previousPosition = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        RaycastHit2D hit;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end);
        boxCollider.enabled = true;

        if (hit.transform == null || (!hit.collider.CompareTag("Walls") && !hit.collider.CompareTag("Visited")))
        {
            StartCoroutine(SmoothMovement(rb2D, start, end));
            return true;
        }

        return false;

    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Collectable"))
        {
            other.GetComponent<Collider2D>().enabled = false;
            //other.gameObject.SetActive(false);
            other.GetComponent<Floater>().enabled = false;
            if (lineQueue.Count == 0) other.attachedRigidbody.GetComponent<PreviousPosition>().previousPosition = previousPosition;
            lineQueue.Add(other.attachedRigidbody);
        }
        
    }

}
