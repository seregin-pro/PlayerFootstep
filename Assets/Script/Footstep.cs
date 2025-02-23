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
            // ������ ����������
            public List<Material> m_woodMaterial; // ������
            public List<Material> m_metalMaterial; // �����
            public List<Material> m_grassMaterial; // �����
            public List<Material> m_dirtMaterial; // �����
            public List<Material> m_concreteMaterial; // �����
            public List<Material> m_carpetMaterial; // �����
            public List<Material> m_sandMaterial; // �����

            // ������ �������� �������
            public List<int> m_woodTexture; // ������
            public List<int> m_metalTexture; // �����
            public List<int> m_grassTexture; // �����
            public List<int> m_dirtTexture; // �����
            public List<int> m_concreteTexture; // �����
            public List<int> m_carpetTexture; // �����
            public List<int> m_sandTexture; // �����

            // ������ �������� ��������
            public List<AudioClip> m_woodAudioClip; // ������
            public List<AudioClip> m_metalAudioClip; // �����
            public List<AudioClip> m_grassAudioClip; // �����
            public List<AudioClip> m_dirtAudioClip; // �����
            public List<AudioClip> m_concreteAudioClip; // �����
            public List<AudioClip> m_carpetAudioClip; // �����
            public List<AudioClip> m_sandAudioClip; // �����
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
            // ������
            if (!m_Jump)
            {
                m_Jump = Input.GetButtonDown("Jump");
            }

            if (m_CharacterController.isGrounded && m_Jump)
            {
                PlayFootStepSound(m_volumeJump);
                m_Jump = false;
            }

            // �����������
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

                // ����
                ProgressStepCycle(speed);
            } 
        }
		
		private void PlayFootStepSound(float volume)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            List<AudioClip> currentAudioClip = new List<AudioClip>();

            // ����� ��������� �� ������� � ����������
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<MeshRenderer>())
                {
                    // ��������� �������� �� ������� ����� ����� � ��������� �������������� ����� ��������� �������� �������
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

            // ����� ��������� �� ��������
            int index;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                TextureTerrainData textureTerrainData = hit.transform.GetComponent<TextureTerrainData>();
                
                if (textureTerrainData != null)
                {
                    index = textureTerrainData.GetTextureIndex(hit.point);

                    // ��������� �������� �� ������� ����� ����� � ��������� �������������� ����� ��������� �������� �������
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
