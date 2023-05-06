using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IPlayer, IStompable
{
    private Rigidbody2D rb;

    public static Transform playerTransform;

    [SerializeField] private int grassMoveSfxIndex;
    [SerializeField] private int mudMoveSfxIndex;
    [SerializeField] private int sandMoveSfxIndex;
    [SerializeField] private Tilemap landTilemap;
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase mudTile;
    [SerializeField] private Tilemap coastTilemap;
    [SerializeField] private TileBase coastTile;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_dashSpeed;
    private Vector2 m_input;
    public Vector2 m_lastInput;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sr;
    private bool inControl = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = transform;
        GlobalInputManager.InputMaster.Player.Dash.performed += _ => Dash();
    }
    private void Update()
    {
        if (GetComponent<Fishing>().currentState == Fishing.FishingState.Idle)
        {

            anim.SetFloat("LastMoveX", m_lastInput.x);
            anim.SetFloat("LastMoveY", m_lastInput.y);
            sr.flipX = m_lastInput.x < 0;
        }

        m_input = GlobalInputManager.InputMaster.Player.Move.ReadValue<Vector2>();
        if (m_input != Vector2.zero) m_lastInput = m_input;
    }
    private void FixedUpdate()
    {
        rb.velocity *= 0.4f;
        if (GetComponent<Fishing>().currentState == Fishing.FishingState.Idle)
        {
            if (inControl)
            {
                rb.AddForce(m_input * m_moveSpeed);
                if (m_input != Vector2.zero)
                {
                    Vector3Int tilePos = new(Mathf.CeilToInt(transform.position.x + 0.49f), Mathf.CeilToInt(transform.position.y + 0.49f), 0);
                    if (landTilemap.GetTile(tilePos) == grassTile)
                    {
                        AudioManager.i.PlaySoundEffect(grassMoveSfxIndex);
                    }
                    else if (landTilemap.GetTile(tilePos) == mudTile)
                    {
                        AudioManager.i.PlaySoundEffect(mudMoveSfxIndex);
                    }
                    else if (coastTilemap.GetTile(tilePos) == coastTile)
                    {
                        AudioManager.i.PlaySoundEffect(sandMoveSfxIndex);
                    }
                }
            }
        }
    }
    private void Dash()
    {
        if (GetComponent<Fishing>().currentState == Fishing.FishingState.Idle && inControl)
        {
            rb.AddForce(m_dashSpeed * m_lastInput);
        }
    }

    public void Stomp(GoblinStompAnimator stomper)
    {
        StartCoroutine(AsyncStomp());
    }
    public IEnumerator AsyncStomp()
    {
        inControl = false;
        rb.AddForce(-14.73f * m_moveSpeed * m_lastInput);
        yield return new WaitForSeconds(.5f);
        inControl = true;
    }
}
