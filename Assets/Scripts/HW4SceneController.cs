using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class HW4SceneController : MonoBehaviour {
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
	
	public bool canReveal {
		get {return _secondRevealed == null;}
	}

	//public static HW4SceneController Instance;

	void Awake()
	{
		if (PlayerPrefs.GetInt("rows") != 0)
        {
			gridRows = PlayerPrefs.GetInt("rows", 2);
			gridCols = PlayerPrefs.GetInt("columns", 4);

		}
	
	}

	// Use this for initialization
	void Start() {
		Vector3 startPos = originalCard.transform.position;

		// create shuffled list of cards
		int[] numbers = {0, 0, 1, 1, 2, 2, 3, 3};
		numbers = ShuffleArray(numbers);

		// place cards in a grid
		for (int i = 0; i < gridCols; i++) {
			for (int j = 0; j < gridRows; j++) {
				MemoryCard card;

				// use the original for the first grid space
				if (i == 0 && j == 0) {
					card = originalCard;
				} else {
					card = Instantiate(originalCard) as MemoryCard;
				}

				// next card in the list for each grid space
				int index = j * gridCols + i;
				int id = numbers[index];
				card.SetCard(id, images[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);
			}
		}
		
	}

    private void Update()
    {
		if (Input.GetKey(KeyCode.Escape)){
			Debug.Log("Application Quit!");
			Application.Quit();
		}

		if (!updateRan)
		{/* prevent infinite loop if dropdown onValueChanged reloads scene*/
			updateRan = true;
		}
			
    }
    // Knuth shuffle algorithm
    private int[] ShuffleArray(int[] numbers) {
		int[] newArray = numbers.Clone() as int[];
		for (int i = 0; i < newArray.Length; i++ ) {
			int tmp = newArray[i];
			int r = Random.Range(i, newArray.Length);
			newArray[i] = newArray[r];
			newArray[r] = tmp;
		}
		return newArray;
	}

	public void CardRevealed(MemoryCard card) {
		if (_firstRevealed == null) {
			_firstRevealed = card;
		} else {
			_secondRevealed = card;
			StartCoroutine(CheckMatch());
		}
	}


    private IEnumerator CheckMatch() {

		// increment score if the cards match
		if (_firstRevealed.id == _secondRevealed.id) {
			_score++;
			scoreLabel.text = "Score: " + _score;
		}

		// otherwise turn them back over after .5s pause
		else {
			yield return new WaitForSeconds(.5f);

			_firstRevealed.Unreveal();
			_secondRevealed.Unreveal();
		}
		
		_firstRevealed = null;
		_secondRevealed = null;
	}

	public void Restart() {
	
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
		}
        GameObject c_card;
        GameObject grid = GameObject.Find("GridLayout").gameObject;
        GridLayoutGroup glg = grid.GetComponent<GridLayoutGroup>();

        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = gridCols;
        glg.cellSize.Set(100f, 100f);
        glg.spacing.Set(5f, 5f);

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
		glg.cellSize.Set(100f, 100f);
		glg.spacing.Set(5f, 5f);

        for (int i = 0; i < gridCols; i++)
		{
			for (int j = 0; j < gridRows; j++)
			{
				c_card = Instantiate(canvasCard) as GameObject;
				c_card.transform.SetParent(grid.transform);
			}
		}
	}
}
