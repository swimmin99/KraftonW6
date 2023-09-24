using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorState : MonoBehaviour
{
    public Animator animator;
    private Chicken chicken;
    private AnimalState currentState;

    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsHunting = Animator.StringToHash("isHunting");
    public static readonly int Dying = Animator.StringToHash("Dying");
    public static readonly int Eating = Animator.StringToHash("Eating");
    public static readonly int Calling = Animator.StringToHash("Calling");
    public static readonly int Hit = Animator.StringToHash("BeingHit");
    public static readonly int Biting = Animator.StringToHash("Biting");
    public static readonly int Mating = Animator.StringToHash("Mating");

    private Coroutine animationCoroutine;
    private bool isTriggerAnimationPlaying = false;

    void Start()
    {
        chicken = GetComponent<Chicken>();
        currentState = chicken.animalstate;
    }

    void Update()
    {
        if (!isTriggerAnimationPlaying) // Allow boolean animations only when no trigger animation is playing.
        {
            switch (currentState)
            {
                case AnimalState.Neutral:
                    animator.SetBool(IsHunting, false);
                    if (chicken.isMoving)
                    {
                        animator.SetBool(IsRunning, true);
                    }
                    else
                    {
                        animator.SetBool(IsRunning, false);
                    }
                    break;
                case AnimalState.Hungry:
                    animator.SetBool(IsRunning, false);
                    if (chicken.isMoving)
                    {
                        animator.SetBool(IsHunting, true);
                    }
                    else
                    {
                        animator.SetBool(IsHunting, false);
                    }
                    break;
                case AnimalState.Mate:
                    if (chicken.isMoving)
                    {
                        animator.SetBool(IsRunning, true);
                    }
                    else
                    {
                        animator.SetBool(IsRunning, false);
                    }
                    break;
            }
        }
    }

    private void CancelAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        animator.SetBool(IsHunting, false);
        animator.SetBool(IsRunning, false);
        animator.ResetTrigger(Dying);
        animator.ResetTrigger(Eating);
        animator.ResetTrigger(Calling);
        animator.ResetTrigger(Hit);
        animator.ResetTrigger(Biting);
        animator.ResetTrigger(Mating);
    }

    private void PlayAnimationTrigger(string triggerName)
    {
        CancelAnimation(); // Cancel any ongoing animations before triggering a new one.
        animationCoroutine = StartCoroutine(PlayAnimationCoroutine(triggerName));
    }

    private IEnumerator PlayAnimationCoroutine(string triggerName)
    {
        isTriggerAnimationPlaying = true; // Set the flag to indicate a trigger animation is playing.

        switch (triggerName)
        {
            case "Dying":
                animator.SetTrigger(Dying);
                break;
            case "Eating":
                animator.SetTrigger(Eating);
                break;
            case "Calling":
                animator.SetTrigger(Calling);
                break;
            case "Hit":
                animator.SetTrigger(Hit);
                break;
            case "Biting":
                animator.SetTrigger(Biting);
                break;
            case "Mating":
                animator.SetTrigger(Mating);
                break;
        }

        yield return new WaitForSeconds(3f); // Wait for one frame to ensure the trigger is processed.

        // Check if the trigger animation has finished playing (assuming transitions are set correctly).
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        // Animation has finished, so clear the coroutine reference and reset the trigger animation flag.
        animationCoroutine = null;
        isTriggerAnimationPlaying = false;
    }

    public void PlayDying()
    {
        PlayAnimationTrigger("Dying");
    }

    public void PlayEating()
    {
        PlayAnimationTrigger("Eating");
    }

    public void PlayCalling()
    {
        PlayAnimationTrigger("Calling");
    }

    public void PlayHit()
    {
        PlayAnimationTrigger("Hit");
    }

    public void PlayBiting()
    {
        PlayAnimationTrigger("Biting");
    }

    public void PlayMating()
    {
        PlayAnimationTrigger("Mating");
    }
}
