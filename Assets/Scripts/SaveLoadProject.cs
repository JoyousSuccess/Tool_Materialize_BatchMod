﻿using UnityEngine;
using System.Collections;
using System.IO;

using FreeImageAPI;
using System.Runtime.InteropServices;

using System.Xml;
using System.Xml.Serialization;

using System;
using System.Diagnostics;

public enum MapType {
	height,
	diffuse,
	diffuseOriginal,
	metallic,
	smoothness,
	normal,
	edge,
	ao,
	property,
	blank,
	folder
}

public enum FileFormat {
	bmp,
	jpg,
	png,
	tga,
	tiff
}

public class ProjectObject {
	
	public HeightFromDiffuseSettings HFDS;
	public string heightMapPath;

	public EditDiffuseSettings EDS;
	public string diffuseMapPath;
	public string diffuseMapOriginalPath;

	public NormalFromHeightSettings NFHS;
	public string normalMapPath;

	public MetallicSettings MS;
	public string metallicMapPath;

	public SmoothnessSettings SS;
	public string smoothnessMapPath;

	public EdgeSettings ES;
	public string edgeMapPath;

	public AOSettings AOS;
	public string aoMapPath;

	public MaterialSettings MatS;
	
}

public class SaveLoadProject : MonoBehaviour {

	public MainGui mainGui;
	public HeightFromDiffuseGui heightFromDiffuseGui;
	public EditDiffuseGui editDiffuseGui;
	public NormalFromHeightGui normalFromHeightGui;
	public MetallicGui metallicGui;
	public SmoothnessGui SmoothnessGui;
	public EdgeFromNormalGui edgeFromNormalGui;
	public AOFromNormalGui aoFromNormalGui;
	public MaterialGui materailGui;

	ProjectObject thisProject;

	char pathChar;

	public bool busy = false;

	[DllImport ("FreeImage")]
	private static extern FIBITMAP FreeImage_Load( FREE_IMAGE_FORMAT fif, string filename, int flags );
	
	[DllImport ("FreeImage")]
	private static extern void FreeImage_Unload( FIBITMAP dib );
	
	[DllImport ("FreeImage")]
	private static extern bool FreeImage_Save( FREE_IMAGE_FORMAT fif, FIBITMAP dib, string filename, FREE_IMAGE_SAVE_FLAGS flags );
	
	[DllImport ("FreeImage")]
	private static extern int FreeImage_GetHeight( FIBITMAP dib );
	
	[DllImport ("FreeImage")]
	private static extern int FreeImage_GetWidth( FIBITMAP dib );
	
	[DllImport ("FreeImage")]
	private static extern bool FreeImage_GetPixelColor( FIBITMAP dib, int x, int y, RGBQUAD value );
	
	[DllImport ("FreeImage")]
	private static extern FIBITMAP FreeImage_MakeThumbnail( FIBITMAP dib, int max_pixel_size, bool convert );


	
	void Start () {

		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
			pathChar = '\\';
		} else {
			pathChar = '/';
		}

		thisProject = new ProjectObject ();
	
	}
	
	string SwitchFormats(  FileFormat selectedFormat ) {
		
		string extension = "bmp";
		switch (selectedFormat) {
		case FileFormat.bmp:
			extension = "bmp";
			break;
		case FileFormat.jpg:
			extension = "jpg";
			break;
		case FileFormat.png:
			extension = "png";
			break;
		case FileFormat.tga:
			extension = "tga";
			break;
		case FileFormat.tiff:
			extension = "tiff";
			break;
		}
		
		return extension;
	}

	public void LoadProject ( string pathToFile ) {
		UnityEngine.Debug.Log ("Loading Project: " + pathToFile);

		var serializer = new XmlSerializer(typeof(ProjectObject));
		var stream = new FileStream(pathToFile, FileMode.Open);
		thisProject = serializer.Deserialize(stream) as ProjectObject;
		stream.Close();

		heightFromDiffuseGui.SetValues(thisProject);
		editDiffuseGui.SetValues(thisProject);
		normalFromHeightGui.SetValues (thisProject);
		metallicGui.SetValues (thisProject);
		SmoothnessGui.SetValues (thisProject);
		edgeFromNormalGui.SetValues (thisProject);
		aoFromNormalGui.SetValues (thisProject);
		materailGui.SetValues (thisProject);

		mainGui.ClearAllTextures ();

		StartCoroutine ( LoadAllTextures( pathToFile ) );
	}

	public void SaveProject ( string pathToFile,  FileFormat selectedFormat ) {
		UnityEngine.Debug.Log ("Saving Project: " + pathToFile);

		string extension = SwitchFormats(selectedFormat);

		if ( pathToFile.Contains (".") ) {
			pathToFile = pathToFile.Substring (0, pathToFile.LastIndexOf ("."));
		}

		int fileIndex = pathToFile.LastIndexOf (pathChar);
		string projectName = pathToFile.Substring (fileIndex+1, pathToFile.Length-fileIndex-1);

		heightFromDiffuseGui.GetValues(thisProject);
		if (mainGui._HeightMap != null) {
			thisProject.heightMapPath = projectName + "_h." + extension;
		} else {
			thisProject.heightMapPath = "null";
		}

		editDiffuseGui.GetValues(thisProject);
		if (mainGui._DiffuseMap != null) {
			thisProject.diffuseMapPath = projectName + "." + extension;
		} else {
			thisProject.diffuseMapPath = "null";
		}

		if (mainGui._DiffuseMapOriginal != null) {
			thisProject.diffuseMapOriginalPath = projectName + "_." + extension;
		} else {
			thisProject.diffuseMapOriginalPath = "null";
		}

		normalFromHeightGui.GetValues(thisProject);
		if (mainGui._NormalMap != null) {
			thisProject.normalMapPath = projectName + "_n." + extension;
		} else {
			thisProject.normalMapPath = "null";
		}

		metallicGui.GetValues(thisProject);
		if (mainGui._MetallicMap != null) {
			thisProject.metallicMapPath = projectName + "_m." + extension;
		} else {
			thisProject.metallicMapPath = "null";
		}

		SmoothnessGui.GetValues(thisProject);
		if (mainGui._SmoothnessMap != null) {
			thisProject.smoothnessMapPath = projectName + "_s." + extension;
		} else {
			thisProject.smoothnessMapPath = "null";
		}

		edgeFromNormalGui.GetValues(thisProject);
		if (mainGui._EdgeMap != null) {
			thisProject.edgeMapPath = projectName + "_e." + extension;
		} else {
			thisProject.edgeMapPath = "null";
		}

		aoFromNormalGui.GetValues(thisProject);
		if (mainGui._AOMap != null) {
			thisProject.aoMapPath = projectName + "_ao." + extension;
		} else {
			thisProject.aoMapPath = "null";
		}

		materailGui.GetValues (thisProject);

		var serializer = new XmlSerializer(typeof(ProjectObject));
		var stream = new FileStream(pathToFile + ".mtz", FileMode.Create);
		serializer.Serialize(stream, thisProject);
		stream.Close();

		SaveAllFiles (pathToFile, selectedFormat);
	}

	public void SaveAllFiles (string pathToFile, FileFormat selectedFormat) {
		//int fileIndex = pathToFile.LastIndexOf (pathChar);
		//UnityEngine.Debug.Log = "You're saving all files: " + pathToFile.Substring (fileIndex+1, pathToFile.Length-fileIndex-1);
		string extension = SwitchFormats(selectedFormat);
		if (pathToFile.Contains (".")) {
			pathToFile = pathToFile.Substring (0, pathToFile.LastIndexOf ("."));
		}
		StartCoroutine ( SaveAllTextures( extension, pathToFile ) );
	}

	public void SaveFile ( string pathToFile, FileFormat selectedFormat, Texture2D textureToSave, string mapType ) {
		//int fileIndex = pathToFile.LastIndexOf (pathChar);
		//UnityEngine.Debug.Log = "You're saving file: " + pathToFile.Substring (fileIndex+1, pathToFile.Length-fileIndex-1);
		if (pathToFile.Contains (".")) {
			pathToFile = pathToFile.Substring (0, pathToFile.LastIndexOf ("."));
		}
		string extension = SwitchFormats(selectedFormat);
		StartCoroutine ( SaveTexture( extension, textureToSave, pathToFile + mapType ) );
	}

	public void PasteFile( MapType mapTypeToLoad ){

		string tempImagePath = Application.dataPath + "/temp.png";
		//string tempImagePath = Application.persistentDataPath + "/temp.png";
		UnityEngine.Debug.Log (tempImagePath);

		try{
			Process myProcess = new Process ();
			myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			myProcess.StartInfo.CreateNoWindow = true;
			myProcess.StartInfo.UseShellExecute = false;
			myProcess.StartInfo.FileName = Application.streamingAssetsPath.Replace ("/", "\\") + "\\c2i.exe";
			myProcess.StartInfo.Arguments = tempImagePath.Replace ("/", "\\");
			myProcess.EnableRaisingEvents = true;
			myProcess.Start();
			myProcess.WaitForExit ();

			StartCoroutine ( LoadTexture (mapTypeToLoad, tempImagePath) );
		} catch (Exception e ){
			UnityEngine.Debug.Log (e);
		}

	}

	public void CopyFile( Texture2D textureToSave ){

		SaveFile(Application.dataPath + "/temp.png",FileFormat.png,textureToSave, "" );
		//SaveFile(Application.persistentDataPath + "/temp.png",FileFormat.png,textureToSave, "" );

		try{
			Process myProcess = new Process ();
			myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			myProcess.StartInfo.CreateNoWindow = true;
			myProcess.StartInfo.UseShellExecute = false;
			myProcess.StartInfo.FileName = Application.streamingAssetsPath.Replace ("/", "\\") + "\\i2c.exe";
			myProcess.StartInfo.Arguments = Application.dataPath.Replace ("/", "\\") + "\\temp.png";
			myProcess.EnableRaisingEvents = true;
			myProcess.Start();
			myProcess.WaitForExit ();
		} catch (Exception e ){
			UnityEngine.Debug.Log (e);
		}
	}

	//==============================================//
	//			Texture Saving Coroutines			//
	//==============================================//


	public IEnumerator SaveAllTextures( string extension, string pathToFile ) {

		StartCoroutine( SaveTexture ( extension, mainGui._HeightMap, pathToFile + "_h" ) );
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		StartCoroutine( SaveTexture ( extension, mainGui._DiffuseMap, pathToFile + "" ) );
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		StartCoroutine( SaveTexture ( extension, mainGui._DiffuseMapOriginal, pathToFile + "_" ) );
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		StartCoroutine( SaveTexture ( extension, mainGui._NormalMap, pathToFile + "_n" ) );
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		StartCoroutine( SaveTexture ( extension, mainGui._MetallicMap, pathToFile + "_m" ) );
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		StartCoroutine( SaveTexture ( extension, mainGui._SmoothnessMap, pathToFile + "_s" ) );
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		StartCoroutine( SaveTexture ( extension, mainGui._EdgeMap, pathToFile + "_e" ) );
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		StartCoroutine( SaveTexture ( extension, mainGui._AOMap, pathToFile + "_ao" ) );
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }
		
		yield return new WaitForSeconds( 0.01f );
	}

	
	public IEnumerator SaveTexture( string extension, Texture2D textureToSave, string pathToFile ) {
		busy = true;

		if (textureToSave != null) {

			FREE_IMAGE_FORMAT imageFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			FREE_IMAGE_SAVE_FLAGS flags = FREE_IMAGE_SAVE_FLAGS.DEFAULT;
			
			bool useFIF = true;

			switch ( extension.ToLower() ) {
			case "bmp":
				imageFormat = FREE_IMAGE_FORMAT.FIF_BMP;
				break;
			case "tga":
				imageFormat = FREE_IMAGE_FORMAT.FIF_TARGA;
				break;
			case "tiff":
				imageFormat = FREE_IMAGE_FORMAT.FIF_TIFF;
				flags = FREE_IMAGE_SAVE_FLAGS.TIFF_NONE;
				break;
			case "png":
				byte[] pngBytes = textureToSave.EncodeToPNG ();
				File.WriteAllBytes (pathToFile + ".png", pngBytes);
				useFIF = false;
				//imageFormat = FREE_IMAGE_FORMAT.FIF_PNG;
				break;
			case "jpg":
				byte[] jpgBytes = textureToSave.EncodeToJPG ();
				File.WriteAllBytes (pathToFile + ".jpg", jpgBytes);
				useFIF = false;
				//imageFormat = FREE_IMAGE_FORMAT.FIF_JPEG;
				//flags = FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB;
				break;
			default:
				imageFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
				break;
			}

			if( useFIF ){
				byte[] bytes = textureToSave.EncodeToPNG ();

				string tempFilePath = Application.dataPath + pathChar + "temp.png";
				//string tempFilePath = Application.persistentDataPath + pathChar + "temp.png";
				File.WriteAllBytes (tempFilePath, bytes);

				FIBITMAP bitmap = FreeImage_Load (FREE_IMAGE_FORMAT.FIF_PNG, tempFilePath, 0);
				bool saveSuccess = FreeImage_Save (imageFormat, bitmap, pathToFile + "." + extension, flags );
				FreeImage_Unload(bitmap);

				//File.Delete (tempFilePath);
			}

			Resources.UnloadUnusedAssets();
		
		}
		yield return new WaitForSeconds (0.01f);
		busy = false;

	}

	//==============================================//
	//			Texture Loading Coroutines			//
	//==============================================//

	public IEnumerator LoadAllTextures( string pathToFile ) {
		pathToFile = pathToFile.Substring (0, pathToFile.LastIndexOf (pathChar));
		pathToFile += pathChar;

		if (thisProject.heightMapPath != "null") {
			StartCoroutine (LoadTexture (MapType.height, pathToFile + thisProject.heightMapPath));
		}
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		if (thisProject.diffuseMapOriginalPath != "null") {
			StartCoroutine (LoadTexture (MapType.diffuseOriginal, pathToFile + thisProject.diffuseMapOriginalPath));
		}
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		if (thisProject.diffuseMapPath != "null") {
			StartCoroutine (LoadTexture (MapType.diffuse, pathToFile + thisProject.diffuseMapPath));
		}
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		if (thisProject.normalMapPath != "null") {
			StartCoroutine (LoadTexture (MapType.normal, pathToFile + thisProject.normalMapPath));
		}
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		if (thisProject.metallicMapPath != "null") {
			StartCoroutine (LoadTexture (MapType.metallic, pathToFile + thisProject.metallicMapPath));
		}
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		if (thisProject.smoothnessMapPath != "null") {
			StartCoroutine (LoadTexture (MapType.smoothness, pathToFile + thisProject.smoothnessMapPath));
		}
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		if (thisProject.edgeMapPath != "null") {
			StartCoroutine (LoadTexture (MapType.edge, pathToFile + thisProject.edgeMapPath));
		}
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		if (thisProject.aoMapPath != "null") {
			StartCoroutine (LoadTexture (MapType.ao, pathToFile + thisProject.aoMapPath));
		}
		while( busy ){ yield return new WaitForSeconds( 0.01f ); }

		yield return new WaitForSeconds( 0.01f );
	}

	public IEnumerator LoadTexture( MapType textureToLoad, string pathToFile ) {
		busy = true;

		int fileIndex = pathToFile.LastIndexOf ('.');
		string extension = pathToFile.Substring (fileIndex+1, pathToFile.Length-fileIndex-1);

		bool loadSuccess = false;
		string newPathToFile = Application.dataPath + pathChar + "temp.png";
		//string newPathToFile = Application.persistentDataPath + pathChar + "temp.png";
		try {
			// Load The Image
			FREE_IMAGE_FORMAT imageFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			
			switch ( extension.ToLower() ) {
			case "bmp":
				imageFormat = FREE_IMAGE_FORMAT.FIF_BMP;
				break;
			case "tga":
				imageFormat = FREE_IMAGE_FORMAT.FIF_TARGA;
				break;
			case "tif":
				imageFormat = FREE_IMAGE_FORMAT.FIF_TIFF;
				break;
			case "tiff":
				imageFormat = FREE_IMAGE_FORMAT.FIF_TIFF;
				break;
			case "png":
				imageFormat = FREE_IMAGE_FORMAT.FIF_PNG;
				break;
			case "jpg":
				imageFormat = FREE_IMAGE_FORMAT.FIF_JPEG;
				break;
			case "jpeg":
				imageFormat = FREE_IMAGE_FORMAT.FIF_JPEG;
				break;
			default:
				imageFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
				break;
			}
			
			UnityEngine.Debug.Log ("Loading Image: " + pathToFile );

			FIBITMAP bitmap = FreeImage_Load (imageFormat, pathToFile, 0);

			loadSuccess = FreeImage_Save (FREE_IMAGE_FORMAT.FIF_PNG, bitmap, newPathToFile, 0);

			FreeImage_Unload(bitmap);

		}
		
		catch (System.Exception e) {
			UnityEngine.Debug.Log( e );
			UnityEngine.Debug.Log( "Could not import with Free Image Importer" );
		}

		if (loadSuccess) {
			Texture2D newTexture;

			//var path = System.IO.Path.Combine("file:///"+Application.streamingAssetsPath,"image.png");
			WWW www = new WWW ("file:///" + newPathToFile);
			yield return www;
			newTexture = www.texture;
			newTexture.anisoLevel = 9;

			switch( textureToLoad ){
			case MapType.height:
				mainGui._HeightMap = newTexture;
				break;
			case MapType.diffuse:
				mainGui._DiffuseMap = newTexture;
				break;
			case MapType.diffuseOriginal:
				mainGui._DiffuseMapOriginal = newTexture;
				break;
			case MapType.normal:
				mainGui._NormalMap = newTexture;
				break;
			case MapType.metallic:
				mainGui._MetallicMap = newTexture;
				break;
			case MapType.smoothness:
				mainGui._SmoothnessMap = newTexture;
				break;
			case MapType.edge:
				mainGui._EdgeMap = newTexture;
				break;
			case MapType.ao:
				mainGui._AOMap = newTexture;
				break;
			case MapType.folder:
				mainGui.folderDiffuseTexs.Add(newTexture);
				break;
			default:
				break;
			}

			//File.Delete(newPathToFile);
			if(textureToLoad != MapType.folder) {
				mainGui.SetLoadedTexture (textureToLoad);
			}
			www.Dispose();
			Resources.UnloadUnusedAssets();

		}

		yield return new WaitForSeconds (0.01f);

		busy = false;

	}

#region [green] 

	public IEnumerator LoadTexture( MapType textureToLoad, string pathToFile, bool doFolderProcess ) {
		busy = true;

		int fileIndex = pathToFile.LastIndexOf ('.');
		string extension = pathToFile.Substring (fileIndex+1, pathToFile.Length-fileIndex-1);

		bool loadSuccess = false;
		string newPathToFile = Application.dataPath + pathChar + "temp.png";
		//string newPathToFile = Application.persistentDataPath + pathChar + "temp.png";
		try {
			// Load The Image
			FREE_IMAGE_FORMAT imageFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			
			switch ( extension.ToLower() ) {
			case "bmp":
				imageFormat = FREE_IMAGE_FORMAT.FIF_BMP;
				break;
			case "tga":
				imageFormat = FREE_IMAGE_FORMAT.FIF_TARGA;
				break;
			case "tif":
				imageFormat = FREE_IMAGE_FORMAT.FIF_TIFF;
				break;
			case "tiff":
				imageFormat = FREE_IMAGE_FORMAT.FIF_TIFF;
				break;
			case "png":
				imageFormat = FREE_IMAGE_FORMAT.FIF_PNG;
				break;
			case "jpg":
				imageFormat = FREE_IMAGE_FORMAT.FIF_JPEG;
				break;
			case "jpeg":
				imageFormat = FREE_IMAGE_FORMAT.FIF_JPEG;
				break;
			default:
				imageFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
				break;
			}
			
			UnityEngine.Debug.Log ("Loading Image: " + pathToFile );

			FIBITMAP bitmap = FreeImage_Load (imageFormat, pathToFile, 0);

			loadSuccess = FreeImage_Save (FREE_IMAGE_FORMAT.FIF_PNG, bitmap, newPathToFile, 0);

			FreeImage_Unload(bitmap);

		}
		
		catch (System.Exception e) {
			UnityEngine.Debug.Log( e );
			UnityEngine.Debug.Log( "Could not import with Free Image Importer" );
		}

		if (loadSuccess) {
			Texture2D newTexture;

			//var path = System.IO.Path.Combine("file:///"+Application.streamingAssetsPath,"image.png");
			WWW www = new WWW ("file:///" + newPathToFile);
			yield return www;
			newTexture = www.texture;
			newTexture.anisoLevel = 9;

			switch( textureToLoad ){
			case MapType.height:
				mainGui._HeightMap = newTexture;
				break;
			case MapType.diffuse:
				mainGui._DiffuseMap = newTexture;
				break;
			case MapType.diffuseOriginal:
				mainGui._DiffuseMapOriginal = newTexture;
				break;
			case MapType.normal:
				mainGui._NormalMap = newTexture;
				break;
			case MapType.metallic:
				mainGui._MetallicMap = newTexture;
				break;
			case MapType.smoothness:
				mainGui._SmoothnessMap = newTexture;
				break;
			case MapType.edge:
				mainGui._EdgeMap = newTexture;
				break;
			case MapType.ao:
				mainGui._AOMap = newTexture;
				break;
			case MapType.folder:
				mainGui.folderDiffuseTexs.Add(newTexture);
				break;
			default:
				break;
			}

			//File.Delete(newPathToFile);
			if(textureToLoad != MapType.folder) {
				mainGui.SetLoadedTexture(textureToLoad);
			}
			www.Dispose();
			Resources.UnloadUnusedAssets();

		}

		yield return new WaitForSeconds (0.05f);

		busy = false;

		if(doFolderProcess == true) {
			mainGui.DoProcessFolder_HNSA();
		}

	}


#endregion

}
