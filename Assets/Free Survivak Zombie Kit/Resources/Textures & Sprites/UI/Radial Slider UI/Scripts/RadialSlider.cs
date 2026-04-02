using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RadialSlider: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	bool isPointerDown=false;

	public void OnPointerEnter( PointerEventData eventData )
	{
		StartCoroutine( "TrackPointer" );            
	}
	

	public void OnPointerExit( PointerEventData eventData )
	{
		StopCoroutine( "TrackPointer" );
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isPointerDown= true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isPointerDown= false;
	}

	IEnumerator TrackPointer()
	{
		var ray = GetComponentInParent<GraphicRaycaster>();
		var input = FindObjectOfType<StandaloneInputModule>();

		var text = GetComponentInChildren<Text>();
		
		if( ray != null && input != null )
		{
			while( Application.isPlaying )
			{                    

				if (isPointerDown)
				{

					Vector2 localPos;
					RectTransformUtility.ScreenPointToLocalPointInRectangle( transform as RectTransform, Input.mousePosition, ray.eventCamera, out localPos );

					float angle = (Mathf.Atan2(-localPos.y, localPos.x)*180f/Mathf.PI+180f)/360f;

					GetComponent<Image>().fillAmount = angle;

					GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, angle);

					text.text = ((int)(angle*360f)).ToString();

				}

				yield return 0;
			}        
		}   
	}





}
