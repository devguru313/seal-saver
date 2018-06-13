using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Picker
{
	[System.Serializable]
	public class StringList : ItemList<string>{}

	[AddComponentMenu("UI/Picker/StringPicker",1030)]
	public class StringPicker : Picker<PickerItem,string,StringList>
	{
        [SerializeField] Font   m_Font = null;

		protected override void SetParameter (PickerItem item, string param)
		{
			Text textComponent = item.GetComponent<Text>();
			
			if( textComponent == null )
			{
				textComponent = item.gameObject.AddComponent<Text>();
				textComponent.resizeTextForBestFit = true;
				textComponent.color = Color.black;
				textComponent.alignment = TextAnchor.MiddleCenter;

                if(m_Font != null)
                {
                    textComponent.font = m_Font;
                }
            }

			textComponent.text = param;
		}
	}

}
