using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YearSubjectSelection : MonoBehaviour {

    public Text yearText;
    public Text subjectText;

    public string[] subjects = { "Maths", "English" };

    public const int yearMax = 2019;
    public const int yearMin = 1950;
    public int currentYear;
    public int currentSubject;
    
    private void Start()
    {
        currentYear = 2012;
        yearText.text = currentYear.ToString();
        currentSubject = 0;
        subjectText.text = subjects[currentSubject];
    }

    public void OnYearLeft()
    {
        currentYear--;
        if(currentYear < yearMin)
        {
            currentYear = yearMax;
        }
        yearText.text = currentYear.ToString();
    }

    public void OnYearRight()
    {
        currentYear++;
        if (currentYear > yearMax)
        {
            currentYear = yearMin;
        }
        yearText.text = currentYear.ToString();
    }

    public void OnSubjectLeft()
    {
        currentSubject--;
        if(currentSubject < 0)
        {
            currentSubject = subjects.Length - 1;
        }
        subjectText.text = subjects[currentSubject];
    }

    public void OnSubjectRight()
    {
        currentSubject++;
        if (currentSubject > subjects.Length - 1)
        {
            currentSubject = 0;
        }
        subjectText.text = subjects[currentSubject];
    }
}
