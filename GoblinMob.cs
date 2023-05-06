using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinMob : StateMachineBehaviour
{
    [SerializeField] private int whoAmIId;
    [SerializeField] private float jumpForce;
    [SerializeField] private string[] nextSteps;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (whoAmIId == 0)
        {
            int rand = Random.Range(0, nextSteps.Length);
            animator.Play(nextSteps[rand]);
            Debug.Log("" + rand);
        }
        else if(whoAmIId == 1)
        {
            animator.GetComponent<MonoBehaviour>().StartCoroutine(JumpAsync(animator));
        }
    }

    private IEnumerator JumpAsync(Animator animator)
    {
        var rb = animator.GetComponent<Rigidbody2D>();
        Vector3 playerPos = PlayerController.playerTransform.position;
        Vector2 dir = playerPos - animator.transform.position;
        Vector2 force = new(dir.x, jumpForce);
        rb.velocity = force;
        playerPos = PlayerController.playerTransform.position;
        yield return new WaitForSeconds(0.65f);

        dir = playerPos - animator.transform.position;
        rb.velocity = dir * 3f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var rb = animator.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
