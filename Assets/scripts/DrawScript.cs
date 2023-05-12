using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

public class DrawScript : MonoBehaviour
{
    public Camera m_camera;
    public GameObject brush;
    public static LineRenderer currentLineRenderer;
    //public static List<LineRenderer> finalDraw;
    Vector2 lastPos;
   
 public void Draw()
    {
        //RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateBrush();
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(ray, out hit, 2f))
            //{
                
            //}

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
    
    public async void OnClickScreenCaptureButtonAsync()
    {
        StartCoroutine(CaptureScreen());

        string imagePath = "Assets/screenshot.png";

        using (var httpClient = new HttpClient())
        using (var form = new MultipartFormDataContent())
        {
            // Read the file into a byte array
            byte[] fileBytes = File.ReadAllBytes(imagePath);

            // Add the file as binary data to the form data
            form.Add(new ByteArrayContent(fileBytes), "image", Path.GetFileName(imagePath));

            // Send the form data to the API endpoint
            var response = await httpClient.PostAsync("http://127.0.0.1:8000/process_image", form);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to upload file");
            }
            else
            {
               ////lehne el reponse terja3////////////////////////////////////
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }
    }
    
    
    public IEnumerator CaptureScreen()
    {
        // Wait till the last possible moment before screen rendering to hide the UI
        yield return null;
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;

        // Wait for screen rendering to complete
        yield return new WaitForEndOfFrame();

        // Take screenshot
        ScreenCapture.CaptureScreenshot("Assets/screenshot.png");


        // Show UI after we're done
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
    }
    
    void Update()
    {
        Draw();
    }
}
