using System.Collections;
using UnityEngine;
using System.IO;
using System;
using System.Net.Http;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

public class DrawScript : MonoBehaviour
{
    public Camera m_camera;
    public GameObject brush;
    public static LineRenderer currentLineRenderer;
    
    public GameObject charToDraw;
    public GameObject success;
    public GameObject fail;
    public String charToDrawString;
    char randomChar;
    //public static List<LineRenderer> finalDraw;
    Vector2 lastPos;

    private void Start()
    {
        
        System.Random random = new System.Random();
        
        // Generate a random number between 0 and 35 (inclusive)
        int randomNumber = random.Next(36);

        // If the random number is between 0 and 25, it represents a character from A to Z
        if (randomNumber < 26)
        {
            randomChar = (char)('A' + randomNumber);
            Console.WriteLine("Random character: " + randomChar);
        }
        // If the random number is between 26 and 35, it represents a number from 0 to 9
        else
        {
            int randomNum = randomNumber - 26;
            randomChar = (char)('0' + randomNum);
            //randomChar= (string)(randomNumber - 26);

        }
        TextMeshProUGUI charTextMeshPro = charToDraw.GetComponent<TextMeshProUGUI>();
        charToDrawString= randomChar.ToString();
        charTextMeshPro.text = charToDrawString;


        Debug.Log(randomChar);
        Debug.Log(randomNumber);
    }

    public void Draw()
    {
        RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 2f))
            {
                if (hit.collider.gameObject.name != "VerifyButton")
                {
                    CreateBrush();
                }
            }
            else
            {

            CreateBrush();
            }
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
        Debug.Log("cccccccccccccccccccccccccccccccccccccccccccc1");

        StartCoroutine(CaptureScreenAndSend());
        Debug.Log("cccccccccccccccccccccccccccccccccccccccccccc2");
    }

    private IEnumerator CaptureScreenAndSend()
    {
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1");
        // Hide UI temporarily
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;

        // Wait for the end of the frame to ensure everything is rendered
        yield return new WaitForEndOfFrame();

        // Capture screenshot
        Texture2D screenTexture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytes = screenTexture.EncodeToJPG();
        Destroy(screenTexture);

        // Generate a unique filename for each screenshot
        //string filename = "screenshot_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        string imagePath = "Assets/screenshot.jpg";
        File.WriteAllBytes(imagePath, bytes);

        // Show UI again
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa2");
        // Send the screenshot to the API
        yield return SendScreenshotToAPI(imagePath);
    }

    private async Task<IEnumerator> SendScreenshotToAPI(string imagePath)
    {
        Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb1");
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
                // Process the API response
                string responseContent = await response.Content.ReadAsStringAsync();
                Debug.Log(responseContent);

                JObject responseObject = JObject.Parse(responseContent);
                string ocrResult = (string)responseObject["ocr_result"];
                Debug.Log(ocrResult);

                if (ocrResult == charToDrawString)
                {
                    success.SetActive(true);
                }
                else
                {
                    fail.SetActive(true);
                }
            }
        }

        // Delete the temporary screenshot file
        File.Delete(imagePath);
        Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb2");

        return null;
    }

    void Update()
    {
        Draw();
    }
}
