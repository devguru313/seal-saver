using UnityEngine.UI;

public class ScrollerSetYears : Picker.MassiveZoomPickerItem
{
    protected Text[] m_Texts;

    protected override void Awake()
    {
        base.Awake();
        m_Texts = GetComponentsInChildren<Text>();
    }

    public override void SetItemContents(Picker.MassivePickerScrollRect scrollRect, int itemIndex)
    {
        if (m_Texts != null && m_Texts.Length > 0)
        {
            int year = 2015 - itemIndex;
            string tmp = year.ToString();
            foreach (Text text in m_Texts)
            {
                text.text = tmp;
            }
        }
    }
}
