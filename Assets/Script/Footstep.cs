using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace SPKitStart
{

    public class Footstep : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed = 5;
        [SerializeField] private float m_RunSpeed = 10;
        [SerializeField] private float m_StepInterval = 5;
        [SerializeField] private float m_volumeWalk = 0.05f;
        [SerializeField] private float m_volumeRun = 0.1f;
        [SerializeField] private float m_volumeLand = 0.3f;
        [SerializeField] private float m_volumeJump = 0.03f;

        [System.Serializable]
        public class FootStepSound {
            // Список материалов
            public List<Material> m_woodMaterial; // Дерево
            public List<Material> m_metalMaterial; // Метал
            public List<Material> m_grassMaterial; // Трава
            public List<Material> m_dirtMaterial; // Земля
            public List<Material> m_concreteMaterial; // Бетон
            public List<Material> m_carpetMaterial; // Ткань
            public List<Material> m_sandMaterial; // Песок

            // Список индексов текстур
            public List<int> m_woodTexture; // Дерево
            public List<int> m_metalTexture; // Метал
            public List<int> m_grassTexture; // Трава
            public List<int> m_dirtTexture; // Земля
            public List<int> m_concreteTexture; // Бетон
            public List<int> m_carpetTexture; // Ткань
            public List<int> m_sandTexture; // Песок

            // Список звуковых эффектов
            public List<AudioClip> m_woodAudioClip; // Дерево
            public List<AudioClip> m_metalAudioClip; // Метал
            public List<AudioClip> m_grassAudioClip; // Трава
            public List<AudioClip> m_dirtAudioClip; // Земля
            public List<AudioClip> m_concreteAudioClip; // Бетон
            public List<AudioClip> m_carpetAudioClip; // Ткань
            public List<AudioClip> m_sandAudioClip; // Песок
        }

        public FootStepSound m_footStepSound = new FootStepSound();
        
        private AudioSource m_AudioSource;
        private CharacterController m_CharacterController;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jump;
        private bool m_PreviouslyGrounded = true;

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_CharacterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // Прыжок
            if (!m_Jump)
            {
                m_Jump = Input.GetButtonDown("Jump");
            }

            if (m_CharacterController.isGrounded && m_Jump)
            {
                PlayFootStepSound(m_volumeJump);
                m_Jump = false;
            }

            // Приземление
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                PlayFootStepSound(m_volumeLand);
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        private void FixedUpdate()
        {
            if (m_CharacterController.isGrounded)
            {
                float speed;
                GetInput(out speed);

                // Шаги
                ProgressStepCycle(speed);
            } 
        }
		
		private void PlayFootStepSound(float volume)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            List<AudioClip> currentAudioClip = new List<AudioClip>();

            // Игрок находится по объекте с материалом
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<MeshRenderer>())
                {
                    // Проверяем материал на котором стоит игрок и добавляем соответсвующие этому материалу звуковые эффекты
                    if (m_footStepSound.m_woodMaterial.Any() && m_footStepSound.m_woodMaterial.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && m_footStepSound.m_woodAudioClip.Any()) 
                    {
                        currentAudioClip = m_footStepSound.m_woodAudioClip;
                    } else if (m_footStepSound.m_metalMaterial.Any() && m_footStepSound.m_metalMaterial.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && m_footStepSound.m_metalAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_metalAudioClip;
                    } else if (m_footStepSound.m_grassMaterial.Any() && m_footStepSound.m_grassMaterial.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && m_footStepSound.m_grassAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_grassAudioClip;
                    } else if (m_footStepSound.m_dirtMaterial.Any() && m_footStepSound.m_dirtMaterial.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && m_footStepSound.m_dirtAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_dirtAudioClip;
                    } else if (m_footStepSound.m_concreteMaterial.Any() && m_footStepSound.m_concreteMaterial.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && m_footStepSound.m_concreteAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_concreteAudioClip;
                    } else if (m_footStepSound.m_carpetMaterial.Any() && m_footStepSound.m_carpetMaterial.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && m_footStepSound.m_carpetAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_carpetAudioClip;
                    } else if (m_footStepSound.m_sandMaterial.Any() && m_footStepSound.m_sandMaterial.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && m_footStepSound.m_sandAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_sandAudioClip;
                    } 
                }
            }

            // Игрок находится на террейне
            int index;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                TextureTerrainData textureTerrainData = hit.transform.GetComponent<TextureTerrainData>();
                
                if (textureTerrainData != null)
                {
                    index = textureTerrainData.GetTextureIndex(hit.point);

                    // Проверяем материал на котором стоит игрок и добавляем соответсвующие этому материалу звуковые эффекты
                    if (m_footStepSound.m_woodTexture.Any() && m_footStepSound.m_woodTexture.Contains(index) && m_footStepSound.m_woodAudioClip.Any()) 
                    {
                        currentAudioClip = m_footStepSound.m_woodAudioClip;
                    } else if (m_footStepSound.m_metalTexture.Any() && m_footStepSound.m_metalTexture.Contains(index) && m_footStepSound.m_metalAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_metalAudioClip;
                    } else if (m_footStepSound.m_grassTexture.Any() && m_footStepSound.m_grassTexture.Contains(index) && m_footStepSound.m_grassAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_grassAudioClip;
                    } else if (m_footStepSound.m_dirtTexture.Any() && m_footStepSound.m_dirtTexture.Contains(index) && m_footStepSound.m_dirtAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_dirtAudioClip;
                    } else if (m_footStepSound.m_concreteTexture.Any() && m_footStepSound.m_concreteTexture.Contains(index) && m_footStepSound.m_concreteAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_concreteAudioClip;
                    } else if (m_footStepSound.m_carpetTexture.Any() && m_footStepSound.m_carpetTexture.Contains(index) && m_footStepSound.m_carpetAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_carpetAudioClip;
                    } else if (m_footStepSound.m_sandTexture.Any() && m_footStepSound.m_sandTexture.Contains(index) && m_footStepSound.m_sandAudioClip.Any())
                    {
                        currentAudioClip = m_footStepSound.m_sandAudioClip;
                    }
                }
            }

            if (currentAudioClip.Any()) 
            {
                m_AudioSource.PlayOneShot(currentAudioClip[Random.Range(0, currentAudioClip.Count)], volume);
            }
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0)
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + speed) * Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval; 

            float volume = m_IsWalking ? m_volumeWalk : m_volumeRun;

            PlayFootStepSound(volume);
        }

        private void GetInput(out float speed)
        {
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        }
    }
}
