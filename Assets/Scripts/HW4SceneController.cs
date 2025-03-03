using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class HW4SceneController : MonoBehaviour
{
	public int gridRows = 2;
	public int gridCols = 4;
	public float offsetX = 2f;
	public float offsetY = 2.5f;

	[SerializeField] private GameObject canvasCard;
	[SerializeField] private MemoryCard originalCard;
	[SerializeField] private Sprite[] images;
	[SerializeField] private TextMeshProUGUI scoreLabel;
	[SerializeField] private GameObject grid;

	private MemoryCard _firstRevealed;
	private MemoryCard _secondRevealed;
	private int _score = 0;
	private bool updateRan;

	// Helper method to calculate scale factor based on grid dimensions
	private float GetScaleFactorForGrid(int rows, int cols)
	{
		float scaleFactor = 1.0f;
		if (rows > 2 || cols > 4)
		{
			scaleFactor = 0.8f; // Smaller cards for larger grids
		}
		if (rows >= 4 && cols >= 4)
		{
			scaleFactor = 0.7f; // Even smaller for the largest grids
		}
		return scaleFactor;
	}

	// Helper method to adjust spacing based on grid dimensions
	private void AdjustSpacingForGrid(int rows, int cols)
	{
		offsetX = 2f;
		offsetY = 2.5f;

		if (rows > 2 || cols > 4)
		{
			offsetX = 1.6f; // Reduce horizontal spacing for larger grids
			offsetY = 2.0f; // Reduce vertical spacing for larger grids
		}
		if (rows >= 4 && cols >= 4)
		{
			offsetX = 1.4f; // Even smaller spacing for largest grids
			offsetY = 1.8f; // Even smaller spacing for largest grids
		}
	}

	// Helper method to get appropriate cell size based on grid dimensions
	private float GetCellSizeForGrid(int rows, int cols)
	{
		float cellSize = 100f;
		if (rows > 2 || cols > 4)
		{
			cellSize = 80f; // Smaller cards for larger grids
		}
		if (rows >= 4 && cols >= 4)
		{
			cellSize = 70f; // Even smaller for the largest grids
		}
		return cellSize;
	}

	public bool canReveal
	{
		get { return _secondRevealed == null; }
	}

	void Awake()
	{
		if (PlayerPrefs.GetInt("rows") != 0)
		{
			gridRows = PlayerPrefs.GetInt("rows", 2);
			gridCols = PlayerPrefs.GetInt("columns", 4);
		}
	}

	// Use this for initialization
	void Start()
	{
		Vector3 startPos = originalCard.transform.position;

		// Adjust spacing using helper method
		AdjustSpacingForGrid(gridRows, gridCols);

		// Create shuffled list of cards with appropriate number of pairs
		int pairsNeeded = (gridRows * gridCols) / 2;
		int[] numbers = new int[gridRows * gridCols];

		// Fill the array with pairs
		for (int i = 0; i < pairsNeeded; i++)
		{
			numbers[i * 2] = i;
			numbers[i * 2 + 1] = i;
		}

		numbers = ShuffleArray(numbers);

		float scaleFactor = GetScaleFactorForGrid(gridRows, gridCols);

		// place cards in a grid
		for (int i = 0; i < gridCols; i++)
		{
			for (int j = 0; j < gridRows; j++)
			{
				MemoryCard card;

				// use the original for the first grid space
				if (i == 0 && j == 0)
				{
					card = originalCard;
				}
				else
				{
					card = Instantiate(originalCard) as MemoryCard;
				}

				// next card in the list for each grid space
				int index = j * gridCols + i;
				int id = numbers[index];
				card.SetCard(id, images[id]);

				// Apply calculated scale factor
				card.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);
			}
		}
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Debug.Log("Application Quit!");
			Application.Quit();
		}

		if (!updateRan)
		{/* prevent infinite loop if dropdown onValueChanged reloads scene*/
			updateRan = true;
		}
	}

	// Knuth shuffle algorithm
	private int[] ShuffleArray(int[] numbers)
	{
		int[] newArray = numbers.Clone() as int[];
		for (int i = 0; i < newArray.Length; i++)
		{
			int tmp = newArray[i];
			int r = Random.Range(i, newArray.Length);
			newArray[i] = newArray[r];
			newArray[r] = tmp;
		}
		return newArray;
	}

	public void CardRevealed(MemoryCard card)
	{
		if (_firstRevealed == null)
		{
			_firstRevealed = card;
		}
		else
		{
			_secondRevealed = card;
			StartCoroutine(CheckMatch());
		}
	}

	private IEnumerator CheckMatch()
	{
		// increment score if the cards match
		if (_firstRevealed.id == _secondRevealed.id)
		{
			_score++;
			scoreLabel.text = "Score: " + _score;
		}
		// otherwise turn them back over after .5s pause
		else
		{
			yield return new WaitForSeconds(.5f);

			_firstRevealed.Unreveal();
			_secondRevealed.Unreveal();
		}

		_firstRevealed = null;
		_secondRevealed = null;
	}

	public void Restart()
	{
		if (updateRan) /* prevents infinite loop if onValueChanged reloads scene */
		{
			SceneManager.LoadScene("CH5Scene");
		}
	}

	public void LoadScene2()
	{
		/* Load using scene filename */
		SceneManager.LoadScene("CH5Scene2");
		/* Or load scene using index from Build Settings*/
		//SceneManager.LoadScene(1); /      
	}

	public void SetSize(int row, int col)
	{
		gridRows = row;
		gridCols = col;
	}

	public void SetupGridLayout(TMP_Dropdown ddown)
	{
		switch (ddown.value)
		{
			case 0: //2 x 4
				gridRows = 2;
				gridCols = 4;
				break;
			case 1: //2 x 3
				gridRows = 2;
				gridCols = 3;
				break;
			case 2: //2 x 5
				gridRows = 2;
				gridCols = 5;
				break;
			case 3: //3 x 4
				gridRows = 3;
				gridCols = 4;
				break;
			case 4: //4 x 4
				gridRows = 4;
				gridCols = 4;
				break;
			case 5: //4 x 5
				gridRows = 4;
				gridCols = 5;
				break;
		}

		GameObject c_card;
		GameObject grid = GameObject.Find("GridLayout").gameObject;
		GridLayoutGroup glg = grid.GetComponent<GridLayoutGroup>();

		glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		glg.constraintCount = gridCols;

		// Get cell size using helper method
		float cellSize = GetCellSizeForGrid(gridRows, gridCols);
		glg.cellSize = new Vector2(cellSize, cellSize);

		// Adjust spacing based on grid size
		float spacing = 5f;
		if (gridRows > 2 || gridCols > 4)
		{
			spacing = 4f;
		}
		if (gridRows >= 4 && gridCols >= 4)
		{
			spacing = 3f;
		}
		glg.spacing = new Vector2(spacing, spacing);

		float scaleFactor = GetScaleFactorForGrid(gridRows, gridCols);

		for (int i = 0; i < gridCols; i++)
		{
			for (int j = 0; j < gridRows; j++)
			{
				if (i == 0 && j == 0)
				{
					c_card = canvasCard;
				}
				else
				{
					c_card = Instantiate(canvasCard) as GameObject;
				}

				c_card.transform.SetParent(grid.transform);

				// Apply calculated scale factor
				c_card.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
			}
		}
	}

	public void SetupGridLayout()
	{
		GameObject c_card;
		GameObject grid = GameObject.Find("GridLayout").gameObject;
		GridLayoutGroup glg = grid.GetComponent<GridLayoutGroup>();

		glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		glg.constraintCount = gridCols;

		// Get cell size using helper method
		float cellSize = GetCellSizeForGrid(gridRows, gridCols);
		glg.cellSize = new Vector2(cellSize, cellSize);

		// Adjust spacing based on grid size
		float spacing = 5f;
		if (gridRows > 2 || gridCols > 4)
		{
			spacing = 4f;
		}
		if (gridRows >= 4 && gridCols >= 4)
		{
			spacing = 3f;
		}
		glg.spacing = new Vector2(spacing, spacing);

		float scaleFactor = GetScaleFactorForGrid(gridRows, gridCols);

		for (int i = 0; i < gridCols; i++)
		{
			for (int j = 0; j < gridRows; j++)
			{
				c_card = Instantiate(canvasCard) as GameObject;
				c_card.transform.SetParent(grid.transform);

				// Apply calculated scale factor
				c_card.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
			}
		}
	}
}