﻿using UnityEngine;
using System.Collections;
using System.ComponentModel;

public class MetallicSettings {
	
	//[DefaultValueAttribute(Color.black)]
	public Color MetalColor;

	//[DefaultValueAttribute(Vector2.zero)]
	public Vector2 SampleUV;
	
	[DefaultValueAttribute(1.0f)]
	public float HueWeight;
	[DefaultValueAttribute(0.5f)]
	public float SatWeight;
	[DefaultValueAttribute(0.2f)]
	public float LumWeight;

	[DefaultValueAttribute(0.0f)]
	public float MaskLow;
	[DefaultValueAttribute(1.0f)]
	public float MaskHigh;

	[DefaultValueAttribute(0)]
	public int BlurSize;
	[DefaultValueAttribute("0")]
	public string BlurSizeText;

	[DefaultValueAttribute(30)]
	public int OverlayBlurSize;
	[DefaultValueAttribute("30")]
	public string OverlayBlurSizeText;
	
	[DefaultValueAttribute(1.0f)]
	public float BlurOverlay;
	[DefaultValueAttribute("1")]
	public string BlurOverlayText;
	
	[DefaultValueAttribute(1.0f)]
	public float FinalContrast;
	[DefaultValueAttribute("1")]
	public string FinalContrastText;

	[DefaultValueAttribute(0.0f)]
	public float FinalBias;
	[DefaultValueAttribute("0")]
	public string FinalBiasText;

	[DefaultValueAttribute(false)]
	public bool useAdjustedDiffuse;
	[DefaultValueAttribute(true)]
	public bool useOriginalDiffuse;

	public MetallicSettings(){
		this.MetalColor = Color.black;

		this.SampleUV = Vector2.zero;

		this.HueWeight = 1.0f;
		this.SatWeight = 0.5f;
		this.LumWeight = 0.2f;

		this.MaskLow = 0.0f;
		this.MaskHigh = 1.0f;

		this.BlurSize = 0;
		this.BlurSizeText = "0";

		this.OverlayBlurSize = 30;
		this.OverlayBlurSizeText = "30";

		this.BlurOverlay = 1.0f;
		this.BlurOverlayText = "1";

		this.FinalContrast = 1.0f;
		this.FinalContrastText = "1";

		this.FinalBias = 0.0f;
		this.FinalBiasText = "0";

		this.useAdjustedDiffuse = false;
		this.useOriginalDiffuse = true;

	}
	
}

public class MetallicGui : MonoBehaviour {

	public MainGui MainGuiScript;
	
	Texture2D _DiffuseMap;
	Texture2D _DiffuseMapOriginal;

	Texture2D _MetallicMap;
	Texture2D _MetalColorMap;

	RenderTexture _TempMap;
	RenderTexture _BlurMap;
	RenderTexture _OverlayBlurMap;

	public Material thisMaterial;
	Material blitMaterial;
	Material metallicBlitMaterial;

	int imageSizeX;
	int imageSizeY;

	MetallicSettings MS;

	float Slider = 0.5f;
	
	bool lastUseAdjustedDiffuse = false;

	Rect windowRect = new Rect (30, 300, 300, 530);
	
	public GameObject testObject;
	bool doStuff = false;
	bool newTexture = false;
	bool selectingColor = false;
	bool mouseButtonDown = false;

	bool settingsInitialized = false;

	public bool busy = true;
	
	public void GetValues( ProjectObject projectObject ) {
		InitializeSettings ();
		projectObject.MS = MS;
	}
	
	public void SetValues( ProjectObject projectObject ) {
		InitializeSettings ();
		if (projectObject.MS != null) { 
			MS = projectObject.MS;
		} else {
			settingsInitialized = false;
			InitializeSettings ();
		}

		_MetalColorMap.SetPixel (1, 1, MS.MetalColor);
		_MetalColorMap.Apply ();

		doStuff = true;
	}
	
	void InitializeSettings() {
		
		if (settingsInitialized == false) {
			Debug.Log ("Initializing Metallic Settings");
			MS = new MetallicSettings ();

			_MetalColorMap = new Texture2D (1, 1, TextureFormat.ARGB32, false, true);
			_MetalColorMap.SetPixel (1, 1, MS.MetalColor);
			_MetalColorMap.Apply ();

			settingsInitialized = true;
		}
		
	}

	// Use this for initialization
	void Start () {

		testObject.GetComponent<Renderer>().sharedMaterial = thisMaterial;
		
		blitMaterial = new Material (Shader.Find ("Hidden/Blit_Shader"));

		metallicBlitMaterial = new Material (Shader.Find ("Hidden/Blit_Metallic"));

		InitializeSettings ();
		
	}

	public void DoStuff() {
		doStuff = true;
	}
	
	public void NewTexture() {
		newTexture = true;
	}

	void SelectColor() {
		if ( Input.GetMouseButton(0) ) {

			mouseButtonDown = true;

			RaycastHit hit;
			if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
				return;

			Renderer rend = hit.transform.GetComponent<Renderer>();
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
				return;

			Vector2 pixelUV = hit.textureCoord;

			MS.SampleUV = pixelUV;

			if( MS.useAdjustedDiffuse ){
				MS.MetalColor = _DiffuseMap.GetPixelBilinear(pixelUV.x, pixelUV.y);
			}else{
				MS.MetalColor = _DiffuseMapOriginal.GetPixelBilinear(pixelUV.x, pixelUV.y);
			}

			_MetalColorMap.SetPixel (1, 1, MS.MetalColor);
			_MetalColorMap.Apply ();

		}

		if ( Input.GetMouseButtonUp(0) && mouseButtonDown ) {

			mouseButtonDown = false;
			selectingColor = false;

		}

	}
	
	// Update is called once per frame
	void Update () {
		
		if (selectingColor) {
			SelectColor();
		}
		
		if (newTexture) {
			InitializeTextures();
			newTexture = false;
		}

		if (MS.useAdjustedDiffuse != lastUseAdjustedDiffuse) {
			lastUseAdjustedDiffuse = MS.useAdjustedDiffuse;
			doStuff = true;
		}
		
		if (doStuff) {
			StartCoroutine( ProcessBlur () );
			doStuff = false;
		}
		
		//thisMaterial.SetFloat ("_BlurWeight", BlurWeight);

		thisMaterial.SetVector ( "_MetalColor", MS.MetalColor);
		thisMaterial.SetVector ( "_SampleUV", new Vector4 (MS.SampleUV.x, MS.SampleUV.y, 0, 0));

		thisMaterial.SetFloat ( "_HueWeight", MS.HueWeight );
		thisMaterial.SetFloat ( "_SatWeight", MS.SatWeight );
		thisMaterial.SetFloat ( "_LumWeight", MS.LumWeight );
		thisMaterial.SetFloat ( "_MaskLow", MS.MaskLow );
		thisMaterial.SetFloat ( "_MaskHigh", MS.MaskHigh );

		thisMaterial.SetFloat ( "_Slider", Slider);
		thisMaterial.SetFloat ( "_BlurOverlay", MS.BlurOverlay);
		thisMaterial.SetFloat ( "_FinalContrast", MS.FinalContrast);
		thisMaterial.SetFloat ( "_FinalBias", MS.FinalBias); 

		if ( MS.useAdjustedDiffuse ) {
			thisMaterial.SetTexture ("_MainTex", _DiffuseMap);
		} else {
			thisMaterial.SetTexture ("_MainTex", _DiffuseMapOriginal);
		}
		
	}

	string FloatToString ( float num, int length ) {

		string numString = num.ToString ();
		int numStringLength = numString.Length;
		int lastIndex = Mathf.FloorToInt( Mathf.Min ( (float)numStringLength , (float)length ) );

		return numString.Substring (0, lastIndex);
	}

	void DoMyWindow ( int windowID ) {

		int spacingX = 0;
		int spacingY = 50;
		int spacing2Y = 70;
		
		int offsetX = 10;
		int offsetY = 30;

		if (_DiffuseMap != null) { GUI.enabled = true; } else { GUI.enabled = false; }
		if( GUI.Toggle (new Rect (offsetX, offsetY, 140, 30), MS.useAdjustedDiffuse, " Use Edited Diffuse")) {
			MS.useAdjustedDiffuse = true;
			MS.useOriginalDiffuse = false;
		}
		GUI.enabled = true;
		if (GUI.Toggle (new Rect (offsetX + 150, offsetY, 140, 30), MS.useOriginalDiffuse, " Use Original Diffuse")) {
			MS.useAdjustedDiffuse = false;
			MS.useOriginalDiffuse = true;
		}

		offsetY += 30;

		GUI.Label (new Rect (offsetX, offsetY, 250, 30), "Metalic Reveal Slider" );
		Slider = GUI.HorizontalSlider( new Rect( offsetX, offsetY + 20, 280, 10 ),Slider,0.0f, 1.0f );
		
		offsetY += 40;

		if (GUI.Button (new Rect (offsetX, offsetY + 10, 80, 30), "Pick Color")) {
			selectingColor = true;
		}

		GUI.DrawTexture( new Rect (offsetX, offsetY + 50, 80, 80), _MetalColorMap );
		
		GUI.Label (new Rect (offsetX + 90, offsetY, 250, 30), "Hue");
		MS.HueWeight = GUI.VerticalSlider( new Rect( offsetX + 95, offsetY + 30, 10, 100 ),MS.HueWeight,1.0f, 0.0f );

		GUI.Label (new Rect (offsetX + 130, offsetY, 250, 30), "Sat" );
		MS.SatWeight = GUI.VerticalSlider( new Rect( offsetX + 135, offsetY + 30, 10, 100 ),MS.SatWeight,1.0f, 0.0f );

		GUI.Label (new Rect (offsetX + 170, offsetY, 250, 30), "Lum" );
		MS.LumWeight = GUI.VerticalSlider( new Rect( offsetX + 175, offsetY + 30, 10, 100 ),MS.LumWeight,1.0f, 0.0f );

		GUI.Label (new Rect (offsetX + 220, offsetY, 250, 30), "Low" );
		MS.MaskLow = GUI.VerticalSlider( new Rect( offsetX + 225, offsetY + 30, 10, 100 ),MS.MaskLow,1.0f, 0.0f );

		GUI.Label (new Rect (offsetX + 250, offsetY, 250, 30), "High" );
		MS.MaskHigh = GUI.VerticalSlider( new Rect( offsetX + 255, offsetY + 30, 10, 100 ),MS.MaskHigh,1.0f, 0.0f );

		offsetY += 150;

		if (GuiHelper.Slider (new Rect (offsetX, offsetY, 280, 50), "Blur Size", MS.BlurSize, MS.BlurSizeText, out MS.BlurSize, out MS.BlurSizeText, 0, 100)) {
			doStuff = true;
		}
		offsetY += 40;
		
		if( GuiHelper.Slider (new Rect (offsetX, offsetY, 280, 50), "Overlay Blur Size", MS.OverlayBlurSize, MS.OverlayBlurSizeText, out MS.OverlayBlurSize, out MS.OverlayBlurSizeText, 10, 100 ) ){
			doStuff = true;
		}
		offsetY += 40;

		GuiHelper.Slider (new Rect (offsetX, offsetY, 280, 50), "High Pass Overlay", MS.BlurOverlay, MS.BlurOverlayText, out MS.BlurOverlay, out MS.BlurOverlayText, -10.0f, 10.0f );
		offsetY += 40;

		GuiHelper.Slider (new Rect (offsetX, offsetY, 280, 50), "Final Contrast", MS.FinalContrast, MS.FinalContrastText, out MS.FinalContrast, out MS.FinalContrastText, -2.0f, 2.0f );
		offsetY += 40;

		GuiHelper.Slider (new Rect (offsetX, offsetY, 280, 50), "Final Bias", MS.FinalBias, MS.FinalBiasText, out MS.FinalBias, out MS.FinalBiasText, -0.5f, 0.5f );
		offsetY += 50;
		
		if( GUI.Button (new Rect (offsetX + 150, offsetY, 130, 30), "Set as Metallic" ) ){
			StartCoroutine( ProcessMetallic() );
		}


		GUI.DragWindow();

	}

	void OnGUI () {

		windowRect.width = 300;
		windowRect.height = 500;

		windowRect = GUI.Window(15, windowRect, DoMyWindow, "Metallic From Diffuse");

	}

	public void Close(){
		CleanupTextures ();
		this.gameObject.SetActive (false);
	}

	void CleanupTexture( RenderTexture _Texture ) {
		
		if (_Texture != null) {
			_Texture.Release();
			_Texture = null;
		}
		
	}
	
	void CleanupTextures() {
		
		Debug.Log ("Cleaning Up Textures");
		
		CleanupTexture( _BlurMap );
		CleanupTexture( _OverlayBlurMap );
		CleanupTexture( _TempMap );
		
	}

	public void InitializeTextures() {
		
		testObject.GetComponent<Renderer>().sharedMaterial = thisMaterial;
		
		CleanupTextures ();

		_DiffuseMap = MainGuiScript._DiffuseMap;
		_DiffuseMapOriginal = MainGuiScript._DiffuseMapOriginal;

		if (_DiffuseMap) {
			thisMaterial.SetTexture ("_MainTex", _DiffuseMap);
			imageSizeX = _DiffuseMap.width;
			imageSizeY = _DiffuseMap.height;
		} else {
			thisMaterial.SetTexture ("_MainTex", _DiffuseMapOriginal);
			imageSizeX = _DiffuseMapOriginal.width;
			imageSizeY = _DiffuseMapOriginal.height;

			MS.useAdjustedDiffuse = false;
			MS.useOriginalDiffuse = true;
		}

		Debug.Log ( "Initializing Textures of size: " + imageSizeX.ToString() + "x" + imageSizeY.ToString() );
		
		_TempMap = new RenderTexture (imageSizeX, imageSizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		_TempMap.wrapMode = TextureWrapMode.Repeat;
		_BlurMap = new RenderTexture (imageSizeX, imageSizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		_BlurMap.wrapMode = TextureWrapMode.Repeat;
		_OverlayBlurMap = new RenderTexture (imageSizeX, imageSizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		_OverlayBlurMap.wrapMode = TextureWrapMode.Repeat;
		
	}

	public IEnumerator ProcessMetallic() {

		busy = true;
		
		Debug.Log ("Processing Height");
		
		metallicBlitMaterial.SetVector ("_ImageSize", new Vector4 (imageSizeX, imageSizeY, 0, 0));

		metallicBlitMaterial.SetVector ( "_MetalColor", MS.MetalColor);
		metallicBlitMaterial.SetVector ( "_SampleUV", new Vector4 (MS.SampleUV.x, MS.SampleUV.y, 0, 0));
		
		metallicBlitMaterial.SetFloat ( "_HueWeight", MS.HueWeight );
		metallicBlitMaterial.SetFloat ( "_SatWeight", MS.SatWeight );
		metallicBlitMaterial.SetFloat ( "_LumWeight", MS.LumWeight );
		
		metallicBlitMaterial.SetFloat ( "_MaskLow", MS.MaskLow );
		metallicBlitMaterial.SetFloat ( "_MaskHigh", MS.MaskHigh );
		
		metallicBlitMaterial.SetFloat ( "_Slider", Slider);
		
		metallicBlitMaterial.SetFloat ( "_BlurOverlay", MS.BlurOverlay);
		
		metallicBlitMaterial.SetFloat ( "_FinalContrast", MS.FinalContrast);
		
		metallicBlitMaterial.SetFloat ( "_FinalBias", MS.FinalBias);

		metallicBlitMaterial.SetTexture ("_BlurTex", _BlurMap);

		metallicBlitMaterial.SetTexture ("_OverlayBlurTex", _OverlayBlurMap);
		
		CleanupTexture (_TempMap);
		_TempMap = new RenderTexture (imageSizeX, imageSizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		_TempMap.wrapMode = TextureWrapMode.Repeat;

		if ( MS.useAdjustedDiffuse ) {
			Graphics.Blit( _DiffuseMap, _TempMap, metallicBlitMaterial, 0);
		} else {
			Graphics.Blit( _DiffuseMapOriginal, _TempMap, metallicBlitMaterial, 0);
		}

		RenderTexture.active = _TempMap;

		if (MainGuiScript._MetallicMap != null) {
			Destroy (MainGuiScript._MetallicMap);
		}

		MainGuiScript._MetallicMap = new Texture2D (_TempMap.width, _TempMap.height, TextureFormat.ARGB32, true, true);
		MainGuiScript._MetallicMap.ReadPixels (new Rect (0, 0, _TempMap.width, _TempMap.height), 0, 0);
		MainGuiScript._MetallicMap.Apply ();
		
		yield return new WaitForSeconds(0.01f);
		
		CleanupTexture ( _TempMap );

		busy = false;
		
	}

	public IEnumerator ProcessBlur () {

		busy = true;
		
		Debug.Log ("Processing Blur");
		
		blitMaterial.SetVector ("_ImageSize", new Vector4( imageSizeX, imageSizeY, 0, 0 ) );
		blitMaterial.SetFloat ("_BlurContrast", 1.0f);
		blitMaterial.SetFloat ("_BlurSpread", 1.0f);
		
		// Blur the image 1
		blitMaterial.SetInt ("_BlurSamples", MS.BlurSize);
		blitMaterial.SetVector ("_BlurDirection", new Vector4(1,0,0,0) );
		if (MS.useAdjustedDiffuse) {
			if (MS.BlurSize == 0) {
				Graphics.Blit (_DiffuseMap, _TempMap);
			} else {
				Graphics.Blit (_DiffuseMap, _TempMap, blitMaterial, 1);
			}
		} else {
			if (MS.BlurSize == 0) {
				Graphics.Blit (_DiffuseMapOriginal, _TempMap);
			} else {
				Graphics.Blit (_DiffuseMapOriginal, _TempMap, blitMaterial, 1);
			}
		}
		blitMaterial.SetVector ("_BlurDirection", new Vector4(0,1,0,0) );
		if (MS.BlurSize == 0) {
			Graphics.Blit (_TempMap, _BlurMap);
		} else {
			Graphics.Blit (_TempMap, _BlurMap, blitMaterial, 1);
		}
		thisMaterial.SetTexture ("_BlurTex", _BlurMap);

		// Blur the image for overlay
		blitMaterial.SetInt ("_BlurSamples", MS.OverlayBlurSize);
		blitMaterial.SetVector ("_BlurDirection", new Vector4(1,0,0,0) );
		if ( MS.useAdjustedDiffuse ) {
			Graphics.Blit( _DiffuseMap, _TempMap, blitMaterial, 1 );
		} else {
			Graphics.Blit( _DiffuseMapOriginal, _TempMap, blitMaterial, 1 );
		}
		blitMaterial.SetVector ("_BlurDirection", new Vector4(0,1,0,0) );
		Graphics.Blit(_TempMap, _OverlayBlurMap, blitMaterial, 1);
		thisMaterial.SetTexture ("_OverlayBlurTex", _OverlayBlurMap);
		
		yield return new WaitForSeconds(0.01f);

		busy = false;
		
	}

	public void DoTheJob() {
		StartCoroutine( ProcessMetallic() );
	}
}
