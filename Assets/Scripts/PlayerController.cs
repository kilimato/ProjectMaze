using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Rigidbody2D rb2D;

    public float moveTime = 0.05f;
    private float inverseMoveTime;

    public int moveX = 0;
    public int moveY = 0;
    private bool isCoroutineStarted = false;

    // Start is called before the first frame update
    void Start()
    {

        rb2D = GetComponent<Rigidbody2D>();

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
                Move(moveX, moveY);
            }

        }

    }

    /// <summary>
    /// Unityn Coroutine, joka tuottaa sulavan liikkeen end-koordinaatin suuntaan.
    /// Myöhemmin ehkä järkevämpää tutkia tile-kohtaisesti, eikä Vector3 koordinaatti-kohtaisesti.
    /// </summary>
    /// <param name="end">Liikkumisen päätepiste.</param>
    /// <returns></returns>
    protected IEnumerator SmoothMovement(Vector3 end)
    {

        float remainingDistance = (transform.position - end).sqrMagnitude;
        isCoroutineStarted = true;
        while (remainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            remainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

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
        Vector2 end = start + new Vector2(xDir, yDir);
        StartCoroutine(SmoothMovement(end));
        return true;

    }

}
