using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CustomFX: MonoBehaviour {
	public static CustomFX instance = null;
	public Shader zoomInterpolationShader;
	private Material zoomInterpolationMaterial;
	[SerializeField]
	[Range(0, 2)]
	private float zoomMagnitude;
	[Range(0, 2)]
	public float zoomInterpolation;
	private Material chromaticAberrationMaterial;
	[SerializeField]
#pragma warning disable CS0649
	private Shader chromaticAberrationShader;
#pragma warning restore CS0649
	[Range(0, 2)]
	public float chromaticAberrationIntensity;
	[SerializeField]
	private Material displaceMaterial = null;
	[Range(0, 0.5f)]
	public float displaceMagnitude;
	private float displaceYShift = 0;
	public float displaceSpeed = 0.3f;
	public float displaceZoom = 0.3f;


	private void Awake() {
		if(instance != null) {
			Destroy(this);
			return;
		}
		instance = this;

		chromaticAberrationMaterial = new Material(chromaticAberrationShader);
		zoomInterpolationMaterial = new Material(zoomInterpolationShader);
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		RenderTexture tmpTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
		RenderTexture tmpTexture2 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

		if(zoomMagnitude == 0 || zoomMagnitude == 1) {
			Graphics.Blit(source, tmpTexture);
		} else {
			zoomInterpolationMaterial.SetFloat("_Magnitude", zoomMagnitude);
			zoomInterpolationMaterial.SetFloat("_Interpolation", zoomInterpolation);
			Graphics.Blit(source, tmpTexture, zoomInterpolationMaterial);
		}

		if(chromaticAberrationIntensity == 0 || chromaticAberrationIntensity == 1) {
			Graphics.Blit(tmpTexture, tmpTexture2);
		} else {
			chromaticAberrationMaterial.SetFloat("_Amount", chromaticAberrationIntensity);
			Graphics.Blit(tmpTexture, tmpTexture2, chromaticAberrationMaterial);
		}

		if(displaceMagnitude == 0) {
			Graphics.Blit(tmpTexture2, destination);
		} else {
			displaceMaterial.SetFloat("_Magnitude", displaceMagnitude);
			displaceMaterial.SetFloat("_Shift", displaceYShift);
			displaceMaterial.SetFloat("_Zoom", displaceZoom);
			Graphics.Blit(tmpTexture2, destination, displaceMaterial);
		}

		tmpTexture.Release();
		tmpTexture2.Release();
	}

	private Coroutine distortionCoroutine;
	public void StartScreenDistortion() {
		if(distortionCoroutine != null) {
			return;
		}
		distortionCoroutine = StartCoroutine(_StartScreenDistortion());
	}

	public void StopScreenDistortion() {
		if(distortionCoroutine != null) {
			StartCoroutine(ChangeDistortionMagnitude(0f));
			StopCoroutine(distortionCoroutine);
			distortionCoroutine = null;
		}
	}

	private IEnumerator _StartScreenDistortion() {
		StartCoroutine(ChangeDistortionMagnitude(0.06f));
		while(true) {
			displaceYShift += Time.deltaTime * displaceSpeed;
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator ChangeDistortionMagnitude(float target, float time = 0.3f) {
		float t = 0;
		float start = displaceMagnitude;
		while(Mathf.Abs(displaceMagnitude - target) > float.Epsilon) {
			t += Time.deltaTime / time;
			displaceMagnitude = Mathf.SmoothStep(start, target, t);
			yield return new WaitForEndOfFrame();
		}
		displaceMagnitude = target;
	}

	public void ChromaJump(float time = 0.2f, float chromaIntensity = 0.015f, float zoomMagnitude = 1.1f) {
		StartCoroutine(_ChromaJump(time, chromaIntensity, zoomMagnitude));
    }
    private IEnumerator _ChromaJump(float time, float chromaIntensity, float zoomMagnitude)
    {
        chromaticAberrationIntensity = chromaIntensity;
        this.zoomMagnitude = zoomMagnitude;
        float chromabTarget = 1f;
        float zoomTarget = 1;
        float t = 0;
        float chromabStart = chromaticAberrationIntensity;
        float zoomStart = this.zoomMagnitude;
        while (Mathf.Abs(chromaticAberrationIntensity - chromabTarget) > float.Epsilon)
        {
            t += Time.deltaTime / time;
            chromaticAberrationIntensity = Mathf.SmoothStep(chromabStart, chromabTarget, t);
            this.zoomMagnitude = Mathf.SmoothStep(zoomStart, zoomTarget, t);
            yield return new WaitForEndOfFrame();
        }
        chromaticAberrationIntensity = chromabTarget;
        this.zoomMagnitude = zoomTarget;
    }

	public void Shake(float duration, float xRange, float yRange)
    {
		StartCoroutine(_Shake(duration, xRange, yRange));
    }

	private IEnumerator _Shake(float duration, float xRange, float yRange)
	{
		float elapsed = 0.0f;
		float magnitude = 0.5f;

		Vector3 originalCamPos = new Vector3(0, 0, -10);

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;

			float x = Random.Range(-xRange, xRange);
			float y = Random.Range(-yRange, yRange);

			Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);

			yield return new WaitForEndOfFrame();
		}

		Camera.main.transform.position = originalCamPos;
	}
}