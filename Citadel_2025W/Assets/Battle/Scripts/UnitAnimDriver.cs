using UnityEngine;

public class UnitAnimDriver : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void SetMove(bool moving)
    {
        anim.SetBool("IsMove", moving);
    }

    public void PlayAttack()
    {
        anim.SetTrigger("Attack");
    }

    public void PlayHit()
    {
        anim.SetTrigger("Hit");
    }

    public void PlayDead()
    {
        anim.SetTrigger("Dead");
    }
}