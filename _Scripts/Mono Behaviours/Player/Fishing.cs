using UnityEngine;

public class Fishing : MonoBehaviour
{
    public float castTime = 2.0f;
    public float minWaitTime = 3.0f;
    public float maxWaitTime = 7.0f;
    public float reelTime = 1.5f;

    private enum FishingState { Idle, Casting, Waiting, Reeling }
    private FishingState currentState = FishingState.Idle;
    private float timer = 0.0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentState == FishingState.Idle)
        {
            Vector2 mouse = GlobalInputManager.InputMaster.Player.MousePos.ReadValue<Vector2>();
            Vector3Int mousePosInt = new (
                Mathf.CeilToInt(mouse.x + 0.49f),
                Mathf.CeilToInt(mouse.y + 0.49f),
                0
                );
            if (!IslandGenerator.i.coastTilemap.HasTile(mousePosInt) && IslandGenerator.i.oceanTilemap.HasTile(mousePosInt))
            {
                StartCasting();
            }
        }

        switch (currentState)
        {
            case FishingState.Casting:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    StartWaiting();
                }
                break;

            case FishingState.Waiting:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    StartReeling();
                }
                break;

            case FishingState.Reeling:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    EndFishing();
                }
                break;
        }
    }

    private void StartCasting()
    {
        currentState = FishingState.Casting;
        timer = castTime;
        // Play casting animation
    }

    private void StartWaiting()
    {
        currentState = FishingState.Waiting;
        timer = Random.Range(minWaitTime, maxWaitTime);
        // Play waiting animation
    }

    private void StartReeling()
    {
        currentState = FishingState.Reeling;
        timer = reelTime;
        // Play reeling animation
    }

    private void EndFishing()
    {
        currentState = FishingState.Idle;
        // Reward player with a fish
        Debug.Log("You Got A Fish!");
    }
}
