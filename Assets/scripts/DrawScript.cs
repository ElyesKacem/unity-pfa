using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DrawScript : MonoBehaviour
{
    public Camera m_camera;
    public GameObject brush;
    public static LineRenderer currentLineRenderer;
    //public static List<LineRenderer> finalDraw;
    Vector2 lastPos;
   
 void Draw()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateBrush();
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos != lastPos)
            {
                AddPoint(mousePos);
                lastPos = mousePos;
            }

        }
        else
        {
            
            currentLineRenderer = null;
        }

    }
    void CreateBrush()
    {
        GameObject brushInstance = Instantiate(brush);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        currentLineRenderer.SetPosition(0, mousePos);
        currentLineRenderer.SetPosition(1, mousePos);
    }
    void AddPoint(Vector2 pointPos)
    {
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(positionIndex, pointPos);

    }
    public void OnClickScreenCaptureButton()
    {
        StartCoroutine(CaptureScreen());
    }
    public IEnumerator CaptureScreen()
    {
        // Wait till the last possible moment before screen rendering to hide the UI
        yield return null;
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;

        // Wait for screen rendering to complete
        yield return new WaitForEndOfFrame();

        // Take screenshot
        ScreenCapture.CaptureScreenshot("screenshot.png");

        // Show UI after we're done
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
    }
    //public void ExportToImage()
    //{
    //    // Create a texture with the same size as the Line Renderer
    //    Texture2D texture = new Texture2D(currentLineRenderer.positionCount, 1);

    //    // Loop through the positions of the Line Renderer and set the pixels in the texture
    //    for (int i = 0; i < currentLineRenderer.positionCount; i++)
    //    {
    //        Vector3 position = currentLineRenderer.GetPosition(i);
    //        Color color = currentLineRenderer.startColor;
    //        texture.SetPixel(i, 0, color);
    //    }

    //    // Apply changes to the texture
    //    texture.Apply();

    //    // Convert the texture to a PNG byte array
    //    byte[] bytes = texture.EncodeToPNG();

    //    // Save the byte array as an image file
    //    File.WriteAllBytes("line.png", bytes);

    //    // Destroy the texture to free up memory
    //    Destroy(texture);

    //    Debug.Log("Line exported to image.");
    //}

    // Update is called once per frame
    void Update()
    {
        Draw();
    }
}
