using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetTextFromIndex : MonoBehaviour
{
	protected Text	m_Text;
    public static string year;

	void Awake()
	{
		m_Text = GetComponent<Text>();
	}

	public void SetItemText( int i )
	{
        year = (2015 - i).ToString();
        //Debug.Log(year);
		//m_Text.text = (2015 - i).ToString();
	}
}
