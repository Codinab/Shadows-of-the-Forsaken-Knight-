using System.Collections;
using Interfaces;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Entity : Updatable, IMovable, IHealth
    {
        [SerializeField] private int _maxHealth = 5;
        [SerializeField] private int _health;
        public float speed;

        protected override void Start()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            if (Rigidbody2D == null) Debug.LogError("Rigidbody2D not found");
            MovementDisabledByGetPushed = false;
            MaxHealth = _maxHealth;
            CurrentHealth = MaxHealth;
        }

        // IMovable
        float IMovable.Speed
        {
            get => speed;
            set => speed = value;
        }

        public bool MovementDisabledByGetPushed { get; set; }
        public Rigidbody2D Rigidbody2D { get; set; }


        // IHealth

        public int CurrentHealth
        {
            get => _health;
            set => _health = value;
        }

        public virtual int MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        public void DamagedAnimation()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            StartCoroutine(ChangeColorTemporarily());
        }

        private MeshRenderer meshRenderer;

        private IEnumerator ChangeColorTemporarily()
        {
            if (meshRenderer != null)
            {
                var originalColor = meshRenderer.material.color;
                var material = meshRenderer.material;
                if (CurrentHealth >= 0) material.color = Color.red;
                yield return new WaitForSeconds(0.2f);
                if (CurrentHealth >= 0)
                {
                    material.color = originalColor;
                }
                else
                {
                    material.color = Color.yellow;
                }
            }
        }
        
        protected override bool ConditionsOnFixedUpdate()
        {
            return CurrentHealth >= 0;
        }
    }
}