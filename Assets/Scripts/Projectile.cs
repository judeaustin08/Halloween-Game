using UnityEngine;

public class Projectile : MonoBehaviour
{
    public AudioClip bounceSound;

    private void OnCollisionEnter()
    {
        GameManager.active.universalSoundEffect.PlayOneShot(bounceSound);
    }
}