using UnityEngine;
using System.Collections;

//
public class MaskCamera : MonoBehaviour
{
    public Material EraserMaterial;
    private bool firstFrame;
    private Vector2? newHolePosition;

	//https://stackoverflow.com/questions/23358798/creating-cleaning-effect-in-unity3d
    private void CutHole(Vector2 imageSize, Vector2 imageLocalPosition)
    {
        Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        Rect positionRect = new Rect(
            (imageLocalPosition.x - 0.5f * EraserMaterial.mainTexture.width) / imageSize.x,
            (imageLocalPosition.y - 0.5f * EraserMaterial.mainTexture.height) / imageSize.y,
            EraserMaterial.mainTexture.width / imageSize.x,
            EraserMaterial.mainTexture.height / imageSize.y
        );
        GL.PushMatrix();
        GL.LoadOrtho();
        for (int i = 0; i < EraserMaterial.passCount; i++)
        {
            EraserMaterial.SetPass(i);
            GL.Begin(GL.QUADS);
            GL.Color(Color.white);
            GL.TexCoord2(textureRect.xMin, textureRect.yMax);
            GL.Vertex3(positionRect.xMin, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMax);
            GL.Vertex3(positionRect.xMax, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMin);
            GL.Vertex3(positionRect.xMax, positionRect.yMin, 0.0f);
            GL.TexCoord2(textureRect.xMin, textureRect.yMin);
            GL.Vertex3(positionRect.xMin, positionRect.yMin, 0.0f);
            GL.End();
        }
        GL.PopMatrix();
    }

    public void Start()
    {
        firstFrame = true;
    }

    public void Update()
    {
        newHolePosition = null;
       // if (Input.GetMouseButton(0))
      //  {
			GameObject hand = GameObject.Find("hand");
		hand.transform.position = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y,0.50389f));
			Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Rect worldRect = new Rect(-8.0f, -6.0f, 16.0f, 12.0f);
            if (worldRect.Contains(v))
                newHolePosition = new Vector2(1600 * (v.x - worldRect.xMin) / worldRect.width, 1200 * (v.y - worldRect.yMin) / worldRect.height);
       // }
    }

	public void OnPostRender()
	{
	    if (firstFrame)
	    {
	        firstFrame = false;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
	    }
        if (newHolePosition != null)
            CutHole(new Vector2(1600.0f, 1200.0f), newHolePosition.Value);
	}
}
