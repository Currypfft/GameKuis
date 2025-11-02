using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public QuestionData[] categories;

    private QuestionData selectedCategory;
    private int currentQuestionIndex = 0;

    // UI Elements
    public TMP_Text questionText;
    public Image questionImage;
    public Button[] replyButtons;

    [Header("Score Configuration")]
    public ScoreManager score;
    public int pointsForCorrectAnswer = 10; // Renamed for clarity
    public int pointsForWrongAnswer = 5;    // Renamed for clarity
    public TextMeshProUGUI scoreText;      // Used for displaying final score in GameFinishedPanel

    [Header("Game State Tracking")]
    private int correctAnswersCount = 0; // FIXED: Changed 'correctReplies' to a private field and renamed
    public float delayAfterReply = 1.5f; // New: Time to wait before showing next question

    [Header("Game Finished Panel")]
    public GameObject GameFinished;


    void Start()
    {
        int selectedCategoryIndex = PlayerPrefs.GetInt("SelectedCategory", 0);

        // Lindungi dari index salah
        if (selectedCategoryIndex < 0 || selectedCategoryIndex >= categories.Length)
        {
            Debug.LogWarning($"Invalid category index {selectedCategoryIndex}, resetting to 0.");
            selectedCategoryIndex = 0;
        }

        GameFinished.SetActive(false);
        SelectCategory(selectedCategoryIndex);
    }


    // --- Category and Question Loading ---

    public void SelectCategory(int categoryIndex)
    {
        // FIXED: koreksi pengecekan batas index
        if (categoryIndex < 0 || categoryIndex >= categories.Length)
        {
            Debug.LogError($"Category index {categoryIndex} is out of bounds!");
            return;
        }

        selectedCategory = categories[categoryIndex];
        currentQuestionIndex = 0;
        correctAnswersCount = 0; // Reset count for new game/category

        DisplayQuestion();
    }

    public void DisplayQuestion()
    {
        if (selectedCategory == null) return;

        // Check if we've reached the end of the quiz
        if (currentQuestionIndex >= selectedCategory.questions.Length)
        {
            ShowGameFinishedPanel();
            return;
        }

        ResetButtons();

        var question = selectedCategory.questions[currentQuestionIndex];
        questionText.text = question.questionText;

        // Set image and handle null case
        if (question.questionImage != null)
        {
            questionImage.sprite = question.questionImage;
            questionImage.gameObject.SetActive(true);
        }
        else
        {
            questionImage.gameObject.SetActive(false);
        }

        // Populate reply buttons
        for (int i = 0; i < replyButtons.Length; i++)
        {
            // Get the TextMeshPro component from the button's children
            TMP_Text buttonText = replyButtons[i].GetComponentInChildren<TMP_Text>();

            if (i < question.replies.Length)
            {
                buttonText.text = question.replies[i];
                replyButtons[i].gameObject.SetActive(true); // Ensure button is active
            }
            else
            {
                // If there are fewer replies than buttons, hide the extra buttons
                replyButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // --- Answer Handling ---

    public void OnReplySelected(int replyIndex)
    {
        // Disable all buttons immediately to prevent multiple clicks
        SetButtonsInteractable(false);

        var currentQuestion = selectedCategory.questions[currentQuestionIndex];
        bool isCorrect = (replyIndex == currentQuestion.correctReplyIndex);

        if (isCorrect)
        {
            // FIXED: Increment the counter for correct answers
            correctAnswersCount++;
            score.AddScore(pointsForCorrectAnswer);
            Debug.Log("Correct Reply! (+ " + pointsForCorrectAnswer + ")");
        }
        else
        {
            score.SubstractScore(pointsForWrongAnswer);
            Debug.Log("Wrong Reply! (- " + pointsForWrongAnswer + ")");
        }

        // Start a Coroutine to introduce a delay before moving to the next question
        StartCoroutine(NextQuestionWithDelay());
    }

    private IEnumerator NextQuestionWithDelay()
    {
        yield return new WaitForSeconds(delayAfterReply);

        currentQuestionIndex++;

        // Tambahan perlindungan agar tidak keluar dari batas array
        if (currentQuestionIndex >= selectedCategory.questions.Length)
        {
            ShowGameFinishedPanel();
            Debug.Log("Quiz Finished!");
            yield break;
        }

        DisplayQuestion();
    }

    // --- UI and Flow Helpers ---

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in replyButtons)
        {
            button.interactable = interactable;
        }
    }

    public void ResetButtons()
    {
        SetButtonsInteractable(true);
        // Optional: Reset button colors/visuals here
    }

    public void ShowGameFinishedPanel()
    {
        GameFinished.SetActive(true);

        // FIXED: Display the total points AND the raw correct count
        string finalScoreMessage = $"Final Score: {score.score}\nCorrect Answers: {correctAnswersCount} / {selectedCategory.questions.Length}";
        scoreText.text = finalScoreMessage;
    }
}
