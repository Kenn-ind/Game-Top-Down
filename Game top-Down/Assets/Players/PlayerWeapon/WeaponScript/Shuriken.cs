using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public int damage = 1;
    public float rotateSpeed = 720f;


    void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DummyEnemy enemy = collision.GetComponent<DummyEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}