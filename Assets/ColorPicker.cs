using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Rob Shewaga @Wabolas 2018/04
/// A color picker using a UI Button. Clicking it expands the picker and allows you to click a color to select it. After selection, the picker shrinks back.
/// </summary>

public class ColorPicker:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler{
	public enum CORNER {
		TOPLEFT,
		TOPRIGHT,
		BOTTOMLEFT,
		BOTTOMRIGHT
	}

	public Color		m_color;			//Resulting color.
	public Color		m_hoverColor;		//Color while hovering the mouse over.
	public Texture2D	m_TexPicker;		//Texture of the color picker.
	public CORNER		m_expandingCorner;	//Which corner to expand from.
	public Vector2		m_expandedScale;	//How large to expand the picker.

	private bool		mp_picking;
	private Vector2		mp_originalScale;
	private Vector3		mp_originalPosition;

	void Start(){
		GetComponent<Button>().onClick.AddListener(()=>UI_Clicked());

		mp_picking=false;
		mp_originalScale=new Vector2(transform.localScale.x,transform.localScale.y);
		mp_originalPosition=transform.localPosition;
	}

	void Update(){
		if(mp_picking) {
			Vector3[] _array = new Vector3[4];  //Bottom left, top left, top right, bottom right.
			GetComponent<RectTransform>().GetWorldCorners(_array);

			Vector2 _pickerDimensions = new Vector2(_array[2].x-_array[0].x, _array[2].y-_array[0].y);
			Vector2 _pickPos = new Vector2(Input.mousePosition.x-_array[0].x, -1*(_array[0].y-Input.mousePosition.y));

			Color _result = m_TexPicker.GetPixel((int)(_pickPos.x/_pickerDimensions.x*m_TexPicker.width), (int)(_pickPos.y/_pickerDimensions.y*m_TexPicker.height));
			m_hoverColor=_result;
		}
	}

	public void OnPointerEnter(PointerEventData eventData) { }
	public void OnPointerExit(PointerEventData eventData) {
		Close();
	}

	public void UI_Clicked() {
		if(!mp_picking) Expand();
		else Close(true);
	}

	public void Expand() {
		mp_picking=true;

		//Record the existing corner's position.
		Vector3 _oldPosition=transform.localPosition;
		Vector3 _oldCornerPos=new Vector3();
		Vector3[] _array=new Vector3[4];	//Bottom left, top left, top right, bottom right.
		GetComponent<RectTransform>().GetWorldCorners(_array);
		if(m_expandingCorner==CORNER.BOTTOMLEFT) _oldCornerPos=_array[0];
		else if(m_expandingCorner==CORNER.TOPLEFT) _oldCornerPos=_array[1];
		else if(m_expandingCorner==CORNER.TOPRIGHT) _oldCornerPos=_array[2];
		else if(m_expandingCorner==CORNER.BOTTOMRIGHT) _oldCornerPos=_array[3];

		transform.localScale=new Vector3(m_expandedScale.x, m_expandedScale.y, transform.localScale.z);
		
		//Find the new corner's position.
		Vector3 _newCornerPos=new Vector3();
		GetComponent<RectTransform>().GetWorldCorners(_array);
		if(m_expandingCorner==CORNER.BOTTOMLEFT) _newCornerPos=_array[0];
		else if(m_expandingCorner==CORNER.TOPLEFT) _newCornerPos=_array[1];
		else if(m_expandingCorner==CORNER.TOPRIGHT) _newCornerPos=_array[2];
		else if(m_expandingCorner==CORNER.BOTTOMRIGHT) _newCornerPos=_array[3];

		//Move picker so that the expanding corner remains still.
		Vector3 _movement=new Vector3(_oldCornerPos.x-_newCornerPos.x,_newCornerPos.y-_oldCornerPos.y,0);
		transform.Translate(new Vector3(_movement.x,-_movement.y),Space.Self);
	}
	public void Close(bool _selectionMade=false) {
		if(mp_picking) {
			if(_selectionMade) {
				Vector3[] _array=new Vector3[4];	//Bottom left, top left, top right, bottom right.
				GetComponent<RectTransform>().GetWorldCorners(_array);
						
				Vector2 _pickerDimensions=new Vector2(_array[2].x-_array[0].x, _array[2].y-_array[0].y);
				Vector2 _pickPos=new Vector2(Input.mousePosition.x-_array[0].x,-1*(_array[0].y-Input.mousePosition.y));

				Color _result=m_TexPicker.GetPixel((int)(_pickPos.x/_pickerDimensions.x*m_TexPicker.width),(int)(_pickPos.y/_pickerDimensions.y*m_TexPicker.height));
				m_color=_result;
			}

			m_hoverColor=m_color;
			transform.localScale=new Vector3(mp_originalScale.x, mp_originalScale.y, transform.localScale.z);
			transform.localPosition=mp_originalPosition;
			mp_picking=false;
		}
	}
	public void SetColor(Color _color) {
		//Set the result and hover color manually.
		m_color=_color;
		m_hoverColor=_color;
	}
}
