using UnityEngine;
using UnityEngine.Events;
namespace Venogear2DPlatformer
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private int _maxHp = 10;
        public int _hp;
        public int MaxHp => _maxHp;

        public int Hp
        {
            get => _hp;
            private set
            {
                var isDamage = value < _hp;
                _hp = Mathf.Clamp(value, min: 0, _maxHp);
                if (isDamage)
                {
                    Damaged?.Invoke(_hp);
                }
                else
                {
                    Healed?.Invoke(_hp);
                }

                if (_hp <= 0)
                {
                    Died?.Invoke();
                }
            }
        }

        public UnityEvent<int> Healed;
        public UnityEvent<int> Damaged;
        public UnityEvent Died;


        private void Awake()
        {
            _hp = _maxHp;
        }

        public void Damage(int amount) => Hp -= amount;

        public void Heal(int amount) => Hp += amount;

        public void HealFull() => Hp = _maxHp;

        public void kill() => Hp = 0;

        public void Adjust(int value) => Hp = value;


        // If collision with weapon
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent<Health>(out var health))
            {
                //Debug.Log("dmg done");
                //Debug.Log(col.gameObject.tag);
                health.Damage(amount: 1);
            }
        }
    }
}
