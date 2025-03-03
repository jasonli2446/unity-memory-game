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
}