﻿using UnityEngine;
using System.Collections;

public static class GuiHelper {

	public static string FloatToString ( float num, int length ) {
		
		string numString = num.ToString ();
		int numStringLength = numString.Length;
		int lastIndex = Mathf.FloorToInt( Mathf.Min ( (float)numStringLength , (float)length ) );
		
		return numString.Substring (0, lastIndex);
	}

	// Value is a float
	public static bool Slider( Rect rect, string title, float value, string textValue, out float outValue, out string outTextValue, float minValue, float maxValue ){
		
		if (textValue == null) {
			textValue = value.ToString ();
		}

		int offsetX = (int)rect.x;
		int offsetY = (int)rect.y;
		
		GUI.Label (new Rect (rect.x, rect.y, 250, 30), title);
		offsetY += 20;
		
		bool isChanged = false;
		
		float tempValue = value;
		value = GUI.HorizontalSlider( new Rect( offsetX, offsetY, rect.width - 60, 10 ),value, minValue, maxValue );
		if (value != tempValue) {
			textValue = FloatToString (value, 6);
			isChanged = true;
		}
		textValue = GUI.TextField (new Rect (offsetX + rect.width - 50, offsetY - 5, 50, 20), textValue);
		if (Event.current.type == EventType.KeyDown || Event.current.character == '\n') {
			float.TryParse( textValue, out value );
			value = Mathf.Clamp( value, minValue, maxValue );
			textValue = FloatToString (value, 6);

			if( value != tempValue ){
				isChanged = true;
			}
		}
		
		outValue = value;
		outTextValue = textValue;
		
		return isChanged;
		
	}

	// Value is an int
	public static bool Slider( Rect rect, string title, int value, string textValue, out int outValue, out string outTextValue, int minValue, int maxValue ){
		
		if (textValue == null) {
			textValue = value.ToString ();
		}

		int offsetX = (int)rect.x;
		int offsetY = (int)rect.y;
		
		GUI.Label (new Rect (rect.x, rect.y, 250, 30), title);
		offsetY += 20;
		
		bool isChanged = false;
		
		float tempValue = value;
		value = (int)GUI.HorizontalSlider( new Rect( offsetX, offsetY, rect.width - 60, 10 ),(float)value, (float)minValue, (float)maxValue );
		if (value != tempValue) {
			textValue = FloatToString (value, 6);
			isChanged = true;
		}
		textValue = GUI.TextField (new Rect (offsetX + rect.width - 50, offsetY - 5, 50, 20), textValue);
		if (Event.current.type == EventType.KeyDown || Event.current.character == '\n') {
			int.TryParse( textValue, out value );
			value = (int)Mathf.Clamp( (float)value, (float)minValue, (float)maxValue );
			textValue = value.ToString();

			if( value != tempValue ){
				isChanged = true;
			}
		}
		
		outValue = value;
		outTextValue = textValue;
		
		return isChanged;
		
	}

	// No Title, Value is a float
	public static bool Slider( Rect rect, float value, string textValue, out float outValue, out string outTextValue, float minValue, float maxValue ){
		
		if (textValue == null) {
			textValue = value.ToString ();
		}

		int offsetX = (int)rect.x;
		int offsetY = (int)rect.y;
		
		bool isChanged = false;
		
		float tempValue = value;
		value = GUI.HorizontalSlider( new Rect( offsetX, offsetY, rect.width - 60, 10 ),value, minValue, maxValue );
		if (value != tempValue) {
			textValue = FloatToString (value, 6);
			isChanged = true;
		}
		textValue = GUI.TextField (new Rect (offsetX + rect.width - 50, offsetY - 5, 50, 20), textValue);
		if (Event.current.type == EventType.KeyDown || Event.current.character == '\n') {
			float.TryParse( textValue, out value );
			value = Mathf.Clamp( value, minValue, maxValue );
			textValue = FloatToString (value, 6);

			if( value != tempValue ){
				isChanged = true;
			}
		}
		
		outValue = value;
		outTextValue = textValue;
		
		return isChanged;
		
	}

	// No Title, Value is an int
	public static bool Slider( Rect rect, int value, string textValue, out int outValue, out string outTextValue, int minValue, int maxValue ){
		
		if (textValue == null) {
			textValue = value.ToString ();
		}

		int offsetX = (int)rect.x;
		int offsetY = (int)rect.y;
		
		bool isChanged = false;
		
		float tempValue = value;
		value = (int)GUI.HorizontalSlider( new Rect( offsetX, offsetY, rect.width - 60, 10 ),(float)value, (float)minValue, (float)maxValue );
		if (value != tempValue) {
			textValue = FloatToString (value, 6);
			isChanged = true;
		}
		textValue = GUI.TextField (new Rect (offsetX + rect.width - 50, offsetY - 5, 50, 20), textValue);
		if (Event.current.type == EventType.KeyDown || Event.current.character == '\n') {
			int.TryParse( textValue, out value );
			value = (int)Mathf.Clamp( (float)value, (float)minValue, (float)maxValue );
			textValue = value.ToString();

			if( value != tempValue ){
				isChanged = true;
			}
		}
		
		outValue = value;
		outTextValue = textValue;
		
		return isChanged;
		
	}


	// Verticle Slider

	// No Title, Value is a float
	public static bool VerticalSlider( Rect rect, float value, out float outValue, float minValue, float maxValue, bool doStuff ){
		
		bool isChanged = false;
		
		float tempValue = value;
		value = GUI.VerticalSlider( rect,value, minValue, maxValue );
		if (value != tempValue || doStuff) {
			isChanged = true;
		}
		
		outValue = value;
		
		return isChanged;
		
	}

	// Toggle
	
	// No Title, Value is a float
	public static bool Toggle( Rect rect, bool value, out bool outValue, string Text, bool doStuff ){
		
		bool isChanged = false;
		
		bool tempValue = value;
		value = GUI.Toggle (rect, value, Text);
		if ( value != tempValue || doStuff ) {
			isChanged = true;
		}
		
		outValue = value;
		
		return isChanged;
		
	}
	
}
