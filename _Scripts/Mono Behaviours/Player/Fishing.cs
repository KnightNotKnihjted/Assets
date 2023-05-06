using UnityEngine;

public class Fishing : MonoBehaviour
{
    public float castTime = 2.0f;
    public float minWaitTime = 3.0f;
    public float maxWaitTime = 7.0f;
    public float reelTime = 1.5f;

    public enum FishingState { Idle, Casting, Waiting, Reeling }
    [HideInInspector] public FishingState currentState = FishingState.Idle;
    private float timer = 0.0f;

    private bool claimedCrystal;
    [SerializeField] private LootTable fishLootTable;
    [SerializeField] private Animator anim;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentState == FishingState.Idle)
        {
            Vector2 mouse = JaasUtilities.MouseUtils.MouseWorldPos();
            Vector3Int mousePosInt = new (
                Mathf.CeilToInt(mouse.x + 0.49f),
                Mathf.CeilToInt(mouse.y + 0.49f),
                0
                );
            bool coast = !IslandGenerator.i.coastTilemap.HasTile(mousePosInt);
            bool ocean = IslandGenerator.i.oceanTilemap.HasTile(mousePosInt);
            if (coast && ocean)
            {
                Vector3 dir = PlayerController.playerTransform.position - mousePosInt;
                PlayerController.playerTransform.position = mousePosInt + dir * 2.5f;
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
        // Play casting animation
        transform.position -= new Vector3(-3f,1f,0f);
        anim.SetBool("Fishing", true);
        //Set The State
        currentState = FishingState.Casting;
        timer = castTime;
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
        // Reward player with a fish
        Debug.Log("You Got A Fish!");
        anim.SetBool("Fishing", false);
        transform.position += new Vector3(-3f,1f,0f);
        SpawnFish();
        //Set State Back
        currentState = FishingState.Idle;
    }

    public void SpawnFish()
    {
        if (claimedCrystal)
        {
            foreach (ItemQuantityComposite comp in fishLootTable.Roll())
            {
                PlayerInventoryManager.SpawnItem(comp.item, comp.quantity, PlayerController.playerTransform.position + Vector3.up * 1.9f);
            }
        }
        else
        {
            PlayerInventoryManager.SpawnItem(ItemDataBase.GetItem("BlueCrystal"), 1, PlayerController.playerTransform.position + Vector3.up * 1.9f);
            claimedCrystal = true;
        }
    }
}
