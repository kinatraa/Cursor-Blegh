using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // Reset animator mỗi lần kích hoạt
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }

    // Gọi từ Animation Event ở frame cuối
    public void OnSlashFinished()
    {
        gameObject.SetActive(false);
    }
}