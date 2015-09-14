using UnityEngine;
using System.Collections;

public class menuController : MonoBehaviour
{
	public Texture menuLogo;
	private float buttonX;

	void menuLogoPosition(){
		GUITexture menuLogo = this.GetComponent<GUITexture>();

		// Position the billboard in the center, 
		// but respect the picture aspect ratio
		int textureHeight = GetComponent<GUITexture>().texture.height;
		int textureWidth = GetComponent<GUITexture>().texture.width;
		int screenHeight = Screen.height;
		int screenWidth = Screen.width;
		
		int screenAspectRatio = (screenWidth / screenHeight);
		int textureAspectRatio = (textureWidth / textureHeight);
		
		int scaledHeight;
		int scaledWidth;
		if (textureAspectRatio <= screenAspectRatio)
		{
			// The scaled size is based on the height
			scaledHeight = screenHeight;
			scaledWidth = (screenHeight * textureAspectRatio);
		}
		else
		{
			// The scaled size is based on the width
			scaledWidth = screenWidth;
			scaledHeight = (scaledWidth / textureAspectRatio);
		}
		menuLogo.pixelInset = new Rect(10,10,scaledWidth, scaledHeight);
		buttonX = scaledWidth;
	}

	void createButtons(){
		GUILayout.BeginVertical();
		if(GUILayout.Button("Story Mode")){
			
		}
		if(GUILayout.Button("Multiplayer")){
			
		}
		if(GUILayout.Button("Options")){
			
		}
		
		// End the Groups and Area
		GUILayout.EndVertical();
	}

	void OnGUI()
	{
		menuLogoPosition();
		// Wrap everything in the designated GUI Area
		float posHModifier = Screen.width / 10;
		float posVModifier = Screen.height / 3;
		GUILayout.BeginArea(new Rect((buttonX + posHModifier),posVModifier,(Screen.width/2 - posHModifier),Screen.height/10));

		createButtons();

		GUILayout.EndArea();
	}
}