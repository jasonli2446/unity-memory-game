using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MemoryCard : MonoBehaviour
{
	[SerializeField] private GameObject cardBack;
	[SerializeField] private HW4SceneController controller;

	private int _id;
	public int id
	{
		get { return _id; }
	}

	public void SetCard(int id, Sprite image)
	{
		_id = id;
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = image;

		// Use Simple mode instead of Tiled mode to prevent repeating grid pattern
		spriteRenderer.drawMode = SpriteDrawMode.Simple;
	}
	public void OnMouseDown()
	{
		if (//!EventSystem.current.IsPointerOverGameObject() &&
			cardBack.activeSelf && controller.canReveal)
		{
			cardBack.SetActive(false);
			controller.CardRevealed(this);
		}
	}

	public void Unreveal()
	{
		cardBack.SetActive(true);
	}

	public IEnumerator PlayMatchAnimation()
	{
		// Shake animation
		Vector3 originalPosition = transform.position;
		float shakeDuration = 0.5f;
		float elapsedTime = 0f;

		while (elapsedTime < shakeDuration)
		{
			// Create random offset for shaking effect
			float offsetX = Random.Range(-0.1f, 0.1f);
			float offsetY = Random.Range(-0.1f, 0.1f);

			// Apply offset to card position for shaking effect
			transform.position = new Vector3(
					originalPosition.x + offsetX,
					originalPosition.y + offsetY,
					originalPosition.z
			);

			// Small delay between position changes for visible shaking
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		// Reset position and create smoke effect
		transform.position = originalPosition;
		CreateSmokeEffect();

		// Hide the card after the smoke effect starts
		yield return new WaitForSeconds(0.2f);
		gameObject.SetActive(false);
	}

	private void CreateSmokeEffect()
	{
		// Create the smoke particle effect slightly above the card
		Vector3 smokePosition = transform.position;
		smokePosition.z = transform.position.z - 1f; // Adjust this value as needed to ensure visibility

		GameObject smokeEffect = Instantiate(controller.smokeEffectPrefab, smokePosition, Quaternion.identity);

		// Set the sorting layer and order in layer
		ParticleSystemRenderer renderer = smokeEffect.GetComponent<ParticleSystemRenderer>();
		if (renderer != null)
		{
			renderer.sortingLayerName = "Effects";
			renderer.sortingOrder = 10;
		}

		Destroy(smokeEffect, 1f);
	}
}