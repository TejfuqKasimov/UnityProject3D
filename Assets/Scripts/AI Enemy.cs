using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform player; // Игрок, которого преследует враг
    public NavMeshAgent enemy; // NavMeshAgent для управления движением врага
    public NavMeshSurface navMeshSurface; // NavMeshSurface для перестройки сетки
    public GameObject bulletPrefab; // Префаб пули
    public Transform shootPoint; // Точка, из которой враг стреляет

    public float PlayerRast = 5.5f; // Расстояние до игрока для обновления уровня
    public float levelcount = 0; // Счётчик уровней
    public float rast = 5.5f; // Расстояние до следующего уровня
    public float stopDistance = 2.0f; // Дистанция, которую враг сохраняет от игрока
    public float fireRate = 1.5f; // Скорость стрельбы (раз в секунду)
    public float bulletSpeed = 10f; // Скорость пули
    public float bulletDamage = 25f; // Урон от пули

    private float nextFireTime; // Время следующего выстрела

    void Start()
    {
        navMeshSurface.BuildNavMesh();
        enemy.stoppingDistance = stopDistance; // Устанавливаем дистанцию остановки
        enemy.SetDestination(player.position);
    }

    void Update()
    {
        // Обновляем цель движения врага
        if (Vector3.Distance(enemy.transform.position, player.position) > stopDistance)
        {
            enemy.SetDestination(player.position);
        }

        // Стрельба, если враг может стрелять
        if (Time.time >= nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + fireRate; // Устанавливаем время следующего выстрела
        }

        if (player.position.x >= PlayerRast)
        {
            levelcount++;
            PlayerRast = rast;
            rast = 5.5f + (5 + 2 * (int)(levelcount / 4)) * 3.4f;
            navMeshSurface.BuildNavMesh();
        }
    }

    void ShootAtPlayer()
    {
        // Создаём пулю
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

        // Рассчитываем направление стрельбы с малой точностью (рандомный разброс)
        Vector3 direction = (player.position - shootPoint.position).normalized;
        direction.x += Random.Range(-0.1f, 0.1f); // Рандомный разброс по X
        direction.y += Random.Range(-0.1f, 0.1f); // Рандомный разброс по Y

        // Добавляем скорость пуле
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = direction * bulletSpeed;

        // Добавляем компонент, чтобы пуля наносила урон
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = bulletDamage;
        }

        // Уничтожаем пулю через 5 секунд
        Destroy(bullet, 5f);
    }
}
