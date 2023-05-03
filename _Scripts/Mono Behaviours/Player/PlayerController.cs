using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IPlayer
{
    private Rigidbody2D rb;

    public static Transform playerTransform;

    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_dashSpeed;
    private Vector2 m_input;
    public Vector2 m_lastInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = transform;
        GlobalInputManager.InputMaster.Player.Dash.performed += _ => Dash();
    }
    private void Update()
    {
        m_input = GlobalInputManager.InputMaster.Player.Move.ReadValue<Vector2>();
        if (m_input != Vector2.zero) m_lastInput = m_input;
    }
    private void FixedUpdate()
    {
        rb.AddForce(m_input * m_moveSpeed);
        rb.velocity *= 0.4f;
    }
    private void Dash()
    {
        rb.AddForce(m_dashSpeed * m_lastInput);
    }
}
