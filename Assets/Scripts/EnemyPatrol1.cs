using UnityEngine;
using System;

public class EnemyPatrol1 : MonoBehaviour
{

    public float direction = 1f;
    public float speed = 2f;
    public float LimitePatrouilleDroite;
    public float LimitePatrouilleGauche;
    private Vector3 LimiteDroite;
    private Vector3 LimiteGauche;

    private bool canFlip;
    private bool isFacingRight = true;
    private Vector3 currentScale;

    private Rigidbody2D rb;
    private SpriteRenderer skin;
    private BoxCollider2D monCollider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out skin);
        TryGetComponent(out monCollider);

        LimiteDroite = new Vector3(transform.position.x + LimitePatrouilleDroite, transform.position.y, transform.position.z);
        LimiteGauche = new Vector3(transform.position.x - LimitePatrouilleGauche, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        patrouilleCheck();
    }

    void patrouilleCheck()
    {
        rb.linearVelocityX = direction * speed;

        if (transform.position.x > LimiteDroite.x)
        {
            direction = -1f;
        }

        if (transform.position.x < LimiteGauche.x)
        {
            direction = 1f;
        }

        if (canFlip)
        {
            //Si on va à gauche
            if ((Math.Round(rb.linearVelocityX) < 0) && isFacingRight == true)
            {
                currentScale.x = -currentScale.x;
                transform.localScale = currentScale;
                isFacingRight = false;
            }
            //Sinon si on va à droite
            else if ((Math.Round(rb.linearVelocityX) > 0) && isFacingRight == false)
            {
                currentScale.x = -currentScale.x;
                transform.localScale = currentScale;
                isFacingRight = true;
            }
        }
    }
}
