using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class UIEnergyManager : MonoBehaviour
{
    public TextMeshProUGUI energyText;
    public Image energyBarImage;
    public ParticleSystem energyFullEffect;
    private Button energyBarButton;

    [Header("Glow Effect")]
    public Color glowColor = new Color(1f, 1f, 0.5f, 1f); // A nice default yellow glow
    public float glowSpeed = 1.5f;

    private Color originalBarColor;
    private Coroutine glowCoroutine;
    private bool isEnergyFull = false;

    private void Awake()
    {
        energyBarButton = GetComponent<Button>();
    }

    private void Start()
    {
        if (energyBarImage != null)
            originalBarColor = energyBarImage.color;

        // Ensure particles are not playing on start, regardless of the "Play On Awake" setting in the Inspector.
        if (energyFullEffect != null)
            energyFullEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnergyUpdated += UpdateEnergy;
            GameManager.Instance.ResetEnergy();
            UpdateEnergy(GameManager.Instance.energy, GameManager.Instance.CurrentMaxEnergy);
        }
        energyBarButton.onClick.AddListener(OnEnergyBarClicked);
        // Start with the button being non-interactable. UpdateEnergy will manage its state.
        energyBarButton.interactable = false;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnergyUpdated -= UpdateEnergy;
        }
        if (energyBarButton != null)
            energyBarButton.onClick.RemoveListener(OnEnergyBarClicked);
    }

    public void UpdateEnergy(int energy, int maxEnergy)
    {
        if (energyText != null)
            energyText.text = $"{energy} / {maxEnergy}";
        if (energyBarImage != null)
        {
            // Clamp fill amount between 0 and 1 for the visual representation.
            energyBarImage.fillAmount = Mathf.Clamp01((float)energy / maxEnergy);
        }

        if (energy >= maxEnergy)
        {
            if (!isEnergyFull)
            {
                isEnergyFull = true;
                energyBarButton.interactable = true;
                if (energyFullEffect != null && !energyFullEffect.isPlaying)
                    energyFullEffect.Play();
                
                if (glowCoroutine == null)
                    glowCoroutine = StartCoroutine(GlowEffect());
            }
        }
        else
        {
            if (isEnergyFull)
            {
                isEnergyFull = false;
                energyBarButton.interactable = false;
                if (energyFullEffect != null)
                    energyFullEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                if (glowCoroutine != null)
                {
                    StopCoroutine(glowCoroutine);
                    glowCoroutine = null;
                    if (energyBarImage != null)
                        energyBarImage.color = originalBarColor;
                }
            }
        }
    }

    private void OnEnergyBarClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TryDrawCardsFromEnergy();
    }

    private IEnumerator GlowEffect()
    {
        while (true)
        {
            // Use Mathf.PingPong to create a smooth back-and-forth pulse for the glow
            float lerpFactor = Mathf.PingPong(Time.time * glowSpeed, 1.0f);
            if (energyBarImage != null)
                energyBarImage.color = Color.Lerp(originalBarColor, glowColor, lerpFactor);
            yield return null;
        }
    }
}