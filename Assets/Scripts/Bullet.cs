using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 25f; // Урон пули

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Находим скрипт здоровья игрока и наносим урон
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Уничтожаем пулю после попадания
            Destroy(gameObject);
        }
        
    }
}
