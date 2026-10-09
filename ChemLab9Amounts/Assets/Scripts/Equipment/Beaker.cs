using System.Collections;
using UnityEngine;
using ChemLab.Data;
using ChemLab.Interaction;
using ChemLab.Chemistry;

namespace ChemLab.Equipment
{
    /// <summary>
    /// Điều khiển thiết bị Cốc/Ống thí nghiệm 3D (Beaker).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Beaker : MonoBehaviour, IInteractable
    {
        [Header("--- DỮ LIỆU HÓA CHẤT CHỨA BÊN TRONG ---")]
        [SerializeField] private ChemicalData currentChemical;
        [SerializeField] private MeshRenderer liquidRenderer;
        [SerializeField] private ParticleSystem pourParticleSystem;

        private Rigidbody rb;
        private Collider col;
        private bool isHeld = false;
        private bool isPouring = false;
        private Transform currentHand;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            EnsureLiquidMesh();

            if (pourParticleSystem == null)
            {
                CreateDefaultPourParticle();
            }
            else
            {
                ConfigureParticleSystem(pourParticleSystem);
            }

            UpdateLiquidVisual();
        }

        private void Update()
        {
            if (isHeld && currentHand != null && !isPouring)
            {
                transform.position = currentHand.position;
                transform.rotation = currentHand.rotation;
            }
        }

        private void EnsureLiquidMesh()
        {
            if (liquidRenderer == null)
            {
                Transform existingLiquid = transform.Find("Liquid");
                if (existingLiquid != null)
                {
                    liquidRenderer = existingLiquid.GetComponent<MeshRenderer>();
                }
                else
                {
                    GameObject liquidObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    liquidObj.name = "Liquid";
                    liquidObj.transform.SetParent(transform);

                    Collider liquidCol = liquidObj.GetComponent<Collider>();
                    if (liquidCol != null) Destroy(liquidCol);

                    liquidRenderer = liquidObj.GetComponent<MeshRenderer>();
                    liquidRenderer.material = new Material(Shader.Find("Standard"));
                }
            }

            AlignLiquidTransform();
        }

        public void SetChemical(ChemicalData data)
        {
            currentChemical = data;
            UpdateLiquidVisual();
        }

        public void SetCustomColor(Color color)
        {
            EnsureLiquidMesh();

            if (liquidRenderer != null)
            {
                liquidRenderer.gameObject.SetActive(true);
                liquidRenderer.material.color = color;
            }

            if (pourParticleSystem != null)
            {
                var main = pourParticleSystem.main;
                main.startColor = color;
            }
        }

        public ChemicalData GetChemical() => currentChemical;

        private void UpdateLiquidVisual()
        {
            EnsureLiquidMesh();

            if (currentChemical != null)
            {
                liquidRenderer.gameObject.SetActive(true);
                liquidRenderer.material.color = currentChemical.liquidColor;

                if (pourParticleSystem != null)
                {
                    var main = pourParticleSystem.main;
                    main.startColor = currentChemical.liquidColor;
                }
            }
            else
            {
                if (liquidRenderer != null) liquidRenderer.gameObject.SetActive(false);
            }
        }

        private void AlignLiquidTransform()
        {
            if (liquidRenderer == null) return;

            Transform liquidTransform = liquidRenderer.transform;
            liquidTransform.localScale = new Vector3(0.08f, 0.04f, 0.08f);
            liquidTransform.localPosition = new Vector3(0f, 0.04f, 0f);
            liquidTransform.localRotation = Quaternion.identity;
        }

        private void CreateDefaultPourParticle()
        {
            GameObject particleObj = new GameObject("PourParticle");
            particleObj.transform.SetParent(transform);
            // Vị trí mép miệng cốc
            particleObj.transform.localPosition = new Vector3(0f, 0.15f, 0.06f);
            particleObj.transform.localRotation = Quaternion.Euler(80f, 0f, 0f);

            pourParticleSystem = particleObj.AddComponent<ParticleSystem>();
            ConfigureParticleSystem(pourParticleSystem);
        }

        private void ConfigureParticleSystem(ParticleSystem ps)
        {
            ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
            if (psRenderer != null)
            {
                Material defaultMat = Shader.Find("Particles/Standard Unlit") != null ?
                    new Material(Shader.Find("Particles/Standard Unlit")) :
                    new Material(Shader.Find("Sprites/Default"));

                psRenderer.material = defaultMat;
            }

            var main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = 2.5f;
            main.startSize = 0.015f;
            main.startSpeed = 0.3f;
            main.startLifetime = 0.35f;
            main.maxParticles = 80;

            if (currentChemical != null)
            {
                main.startColor = currentChemical.liquidColor;
            }

            var emitter = ps.emission;
            emitter.rateOverTime = 0;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 3f;
            shape.radius = 0.005f;

            // Bật va chạm nhưng bỏ qua chính Collider của cốc nguồn
            var collision = ps.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision3D;
            collision.dampen = 0.5f;
            collision.bounce = 0.1f;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // --- Triển khai IInteractable ---

        public string GetInteractPrompt()
        {
            if (isHeld) return "Nhấn [E] để đặt xuống bàn | Nhấp [Chuột trái] vào cốc khác để rót";
            string chemName = currentChemical != null ? currentChemical.chemicalName : "Trống";
            return $"Nhấn [E] để cầm Cốc ({chemName})";
        }

        public void Interact(Transform handPosition)
        {
            if (!isHeld)
            {
                isHeld = true;
                currentHand = handPosition;
                rb.isKinematic = true;
                col.enabled = false;
            }
            else
            {
                isHeld = false;
                currentHand = null;

                int dropLayerMask = LayerMask.GetMask("DropZone", "Default");
                if (Camera.main == null) { col.enabled = true; rb.isKinematic = true; return; }
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

                if (Physics.Raycast(ray, out RaycastHit hit, 5.0f, dropLayerMask))
                {
                    transform.position = hit.point + Vector3.up * 0.15f;
                    transform.rotation = Quaternion.identity;
                }

                col.enabled = true;
                rb.isKinematic = true;

            }
        }

        public void UseAction()
        {
            if (isPouring) return;

            if (Camera.main == null || PouringSystem.Instance == null || currentChemical == null) return;
            int interactMask = LayerMask.GetMask("Interactable");
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 3.5f, interactMask))
            {
                Beaker targetBeaker = hit.collider.GetComponent<Beaker>();
                if (targetBeaker != null && targetBeaker != this)
                {
                    StartCoroutine(PourRoutine(targetBeaker));
                }
            }
        }

        private IEnumerator PourRoutine(Beaker targetBeaker)
        {
            isPouring = true;

            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            // TÍNH TOÁN VỊ TRÍ CHUẨN: Đặt cốc phía trên miệng cốc đích 0.35m và lệch lùi lại 0.12m
            Vector3 directionToTarget = (targetBeaker.transform.position - startPos).normalized;
            if (directionToTarget == Vector3.zero) directionToTarget = Vector3.forward;

            Vector3 targetPourPos = targetBeaker.transform.position + Vector3.up * 0.35f - directionToTarget * 0.12f;
            Quaternion targetPourRot = Quaternion.LookRotation(targetBeaker.transform.position - targetPourPos) * Quaternion.Euler(55f, 0f, 0f);

            float duration = 0.45f;
            float elapsed = 0f;

            // Phase 1: Di chuyển & Nghiêng cốc đến vị trí chuẩn
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.position = Vector3.Lerp(startPos, targetPourPos, t);
                transform.rotation = Quaternion.Slerp(startRot, targetPourRot, t);
                yield return null;
            }

            // BẬT TIA NƯỚC CHẢY
            if (pourParticleSystem != null)
            {
                var emitter = pourParticleSystem.emission;
                emitter.rateOverTime = 45;
                pourParticleSystem.Play();
            }

            yield return new WaitForSeconds(0.5f);

            // TẮT TIA NƯỚC CHẢY
            if (pourParticleSystem != null)
            {
                var emitter = pourParticleSystem.emission;
                emitter.rateOverTime = 0;
                pourParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            // KÍCH HOẠT PHẢN ỨNG VÀ ĐỔI MÀU DUNG DỊCH
            PouringSystem.Instance.PourChemical(this, targetBeaker);

            // Phase 2: Trở về vị trí ban đầu / tay cầm
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.position = Vector3.Lerp(targetPourPos, currentHand != null ? currentHand.position : startPos, t);
                transform.rotation = Quaternion.Slerp(targetPourRot, currentHand != null ? currentHand.rotation : startRot, t);
                yield return null;
            }

            if (currentHand != null)
            {
                transform.position = currentHand.position;
                transform.rotation = currentHand.rotation;
            }

            isPouring = false;
        }
    }
}