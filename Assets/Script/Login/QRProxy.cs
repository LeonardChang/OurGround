using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZXing;
using ZXing.QrCode;


public class QRProxy : MonoBehaviour
{
    protected Dictionary<string, Texture2D> m_qrCodes = new Dictionary<string, Texture2D>();
    protected WebCamTexture m_webCamera = null;
	protected UITexture m_cameraTexture;
    protected string m_scanResult;

    public Texture2D GetQRCode( string str)
    {
        if( m_qrCodes.ContainsKey( str ) == false )
        {
            Texture2D code = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            Color32[] color32 = encode(str, 256, 256);
            code.SetPixels32( color32);
            code.Apply();

            m_qrCodes[str] = code;
        }

        return m_qrCodes[str];
    }

	/// <summary>
	/// start scan QR Code 
	/// </summary>
	public bool OpenCamera( UITexture cameraTexture )
	{
        m_scanResult = null;
		m_cameraTexture = cameraTexture;

		return openCamera();
	}

	/// <summary>
	/// scan QR code 
	/// </summary>
	public string ScanQRCode()
	{
        m_scanResult = searchQRCode();

        if (m_scanResult != null)
		{
            return m_scanResult;
		}

        return null;
    }

	/// <summary>
	/// close scan 
	/// </summary>
	public void CloseCamera()
	{
		m_webCamera.Stop();
        m_webCamera = null;
	}
	
    /// <summary>
    /// return the scan result
    /// </summary>
    public string QR_RESULT
    {
        get
        {
            return m_scanResult;
        }
    }

    /// <summary>
    /// return the camera angle  
    /// </summary>
    /// <returns></returns>
    public float CAMERA_ANGLE
    {
        get
        {
            if (m_webCamera == null)
            {
                return 0.0f;
            }

            return m_webCamera.videoRotationAngle;
        }
    }

	/// <summary>
	/// open camera 
	/// </summary>
	protected bool openCamera()
	{
		WebCamDevice[] devices = WebCamTexture.devices;

        // no camera exist 
        if (devices.Length <= 0)
        {
            return false;
        }

		string deviceName = devices[0].name;
		m_webCamera = new WebCamTexture(deviceName, 400, 300, 12);
		m_cameraTexture.mainTexture = m_webCamera;
        
		m_webCamera.Play();
        m_cameraTexture.MakePixelPerfect();

        return true;
	}

	/// <summary>
	/// search QR Code
	/// </summary>
	protected string searchQRCode()
	{
        WebCamTexture texture = m_cameraTexture.mainTexture as WebCamTexture;
		Color32[] color32 = texture.GetPixels32();

		string text = Decode(color32, texture.width, texture.height);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}

		return null;
	}

    /// <summary>
    /// generate the QR Code
    /// </summary>
    /// <param name="textForEncoding"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    protected Color32[] encode(string textForEncoding, int width, int height)
    {
        BarcodeWriter writer = new BarcodeWriter();
        writer.Format = BarcodeFormat.QR_CODE;

        QrCodeEncodingOptions options = new QrCodeEncodingOptions();
        options.Height = height;
        options.Width = width; 
        options.ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H;
        writer.Options = options;

        return writer.Write(textForEncoding);
    }

	/// <summary>
	/// decode the QR Code to string 
	/// </summary>
	/// <param name="colorForDecoding">Color for decoding.</param>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	protected string Decode(Color32[] colorForDecoding, int width, int height)
	{
		BarcodeReader reader = new BarcodeReader();
		reader.AutoRotate = true;

        Result r = reader.Decode(colorForDecoding, width, height);

        if( r != null )
        {
            return r.Text;
        }

		return null;
	}

}
