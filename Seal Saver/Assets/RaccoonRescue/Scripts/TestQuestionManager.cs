using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestQuestionManager : MonoBehaviour {

    [SerializeField] public string[] questions;
    [SerializeField] public string[] solutions;
    [SerializeField] public string[] wrongAns;
    [SerializeField] public int indexQuestion;
    [SerializeField] public int indexCorrectOption;
    public GameObject questionButton;
    public GameObject optionButton1;
    public GameObject optionButton2;
    public GameObject optionButton3;
    public GameObject optionButton4;
    public GameObject optionSprite1;
    public GameObject optionSprite2;
    public GameObject optionSprite3;
    public GameObject optionSprite4;
    public GameObject questionUI;
    public Sprite defaultButton;
    public Sprite greenButton;
    public Sprite redButton;
    
    Text questionText;
    Text optionText1;
    Text optionText2;
    Text optionText3;
    Text optionText4;
    Image image1;
    Image image2;
    Image image3;
    Image image4;
    

    // Use this for initialization
    void Awake ()
    {
        questionText = questionButton.GetComponent<Text>();
        optionText1 = optionButton1.GetComponent<Text>();
        optionText2 = optionButton2.GetComponent<Text>();
        optionText3 = optionButton3.GetComponent<Text>();
        optionText4 = optionButton4.GetComponent<Text>();
        image1 = optionSprite1.GetComponent<Image>();
        image2 = optionSprite2.GetComponent<Image>();
        image3 = optionSprite3.GetComponent<Image>();
        image4 = optionSprite4.GetComponent<Image>();
        questions = new string[] { "5 x 1", "9 x 8", "7 x 3", "2 x 8", "12 x 11" };
        solutions = new string[] { "5", "72", "21", "16", "121" };
        wrongAns = new string[] { "45", "36", "28", "1000"};
        SelectRandomQuestionAndAnswer();
    }

    public void SelectRandomQuestionAndAnswer()
    {
        indexQuestion = Random.Range(0, questions.Length);              //Index to select random question from question array
        indexCorrectOption = Random.Range(0, 4);                        //Index to select which option (1-4) will contain the correct answer
        questionText.text = questions[indexQuestion];                   //Sets Question Text
        optionText1.text = wrongAns[0];                                 //Initialize options with wrong answers
        optionText2.text = wrongAns[1];
        optionText3.text = wrongAns[2];
        optionText4.text = wrongAns[3];
        switch (indexCorrectOption)                                     //Insert correct answer in selected option
        {
            case 0:
                optionText1.text = solutions[indexQuestion];
                break;
            case 1:
                optionText2.text = solutions[indexQuestion];
                break;
            case 2:
                optionText3.text = solutions[indexQuestion];
                break;
            case 3:
                optionText4.text = solutions[indexQuestion];
                break;
        }
    }

    public void ResetOnCorrect()
    {
        questionUI.GetComponent<AnimationManager>().CloseMenu();    //If correct answer is selected, close the menu to continue the game,
        SelectRandomQuestionAndAnswer();                            //and change the next question to be asked
        image1.sprite = defaultButton;
        image2.sprite = defaultButton;
        image3.sprite = defaultButton;
        image4.sprite = defaultButton;
    }

    public void ResetOnWrong()
    {
        SelectRandomQuestionAndAnswer();                                //If wrong answer is selected, generate another question
        image1.sprite = defaultButton;
        image2.sprite = defaultButton;
        image3.sprite = defaultButton;
        image4.sprite = defaultButton;
    }

    public void AnswerChecker()                                          //Returns pointer to image of correct answer
    {
        switch (indexCorrectOption)
        {
            case 0:
                image1.sprite = greenButton;
                break;
            case 1:
                image2.sprite = greenButton;
                break;
            case 2:
                image3.sprite = greenButton;
                break;
            case 3:
                image4.sprite = greenButton;
                break;
        }
    }

    public void OnOptionSelect1()                                       //If Option 1 is pressed
    {
        AnswerChecker();
        if(indexCorrectOption != 0)
        {
            image1.sprite = redButton;                                  //Change wrong selection to red
            Invoke("ResetOnWrong", 2);                                  //Reset buttons after 2 seconds
        }
        else
        {
            Invoke("ResetOnCorrect", 1);                                //Reset buttons after 1 second
        }
    }

    public void OnOptionSelect2()                                       //If Option 2 is pressed
    {
        AnswerChecker();
        if (indexCorrectOption != 1)
        {
            image2.sprite = redButton;
            Invoke("ResetOnWrong", 2);
        }
        else
        {
            Invoke("ResetOnCorrect", 1);
        }
    }

    public void OnOptionSelect3()                                       //If Option 3 is pressed
    {
        AnswerChecker();
        if (indexCorrectOption != 2)
        {
            image3.sprite = redButton;
            Invoke("ResetOnWrong", 1.5f);
        }
        else
        {
            Invoke("ResetOnCorrect", 0.8f);
        }
    }

    public void OnOptionSelect4()                                       //If Option 4 is pressed
    {
        AnswerChecker();
        if (indexCorrectOption != 3)
        {
            image4.sprite = redButton;
            Invoke("ResetOnWrong", 2);
        }
        else
        {
            Invoke("ResetOnCorrect", 1);
        }
    }
}
