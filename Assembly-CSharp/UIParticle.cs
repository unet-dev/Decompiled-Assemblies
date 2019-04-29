using System;
using UnityEngine;
using UnityEngine.UI;

public class UIParticle : BaseMonoBehaviour
{
	public Vector2 LifeTime;

	public Vector2 Gravity = new Vector2(1000f, 1000f);

	public Vector2 InitialX;

	public Vector2 InitialY;

	public Vector2 InitialScale = Vector2.one;

	public Vector2 InitialDelay;

	public Vector2 ScaleVelocity;

	public Gradient InitialColor;

	private float lifetime;

	private float gravity;

	private Vector2 velocity;

	private float scaleVelocity;

	public UIParticle()
	{
	}

	public static void Add(UIParticle particleSource, RectTransform spawnPosition, RectTransform particleCanvas)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(particleSource.gameObject);
		gameObject.transform.SetParent(spawnPosition, false);
		Transform vector3 = gameObject.transform;
		Rect rect = spawnPosition.rect;
		float single = UnityEngine.Random.Range(0f, rect.width);
		rect = spawnPosition.rect;
		float single1 = single - rect.width * spawnPosition.pivot.x;
		rect = spawnPosition.rect;
		float single2 = UnityEngine.Random.Range(0f, rect.height);
		rect = spawnPosition.rect;
		vector3.localPosition = new Vector3(single1, single2 - rect.height * spawnPosition.pivot.y, 0f);
		gameObject.transform.SetParent(particleCanvas, true);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
	}

	private void Die()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Start()
	{
		Transform transforms = base.transform;
		transforms.localScale = transforms.localScale * UnityEngine.Random.Range(this.InitialScale.x, this.InitialScale.y);
		this.velocity.x = UnityEngine.Random.Range(this.InitialX.x, this.InitialX.y);
		this.velocity.y = UnityEngine.Random.Range(this.InitialY.x, this.InitialY.y);
		this.gravity = UnityEngine.Random.Range(this.Gravity.x, this.Gravity.y);
		this.scaleVelocity = UnityEngine.Random.Range(this.ScaleVelocity.x, this.ScaleVelocity.y);
		Image component = base.GetComponent<Image>();
		if (component)
		{
			component.color = this.InitialColor.Evaluate(UnityEngine.Random.Range(0f, 1f));
		}
		this.lifetime = UnityEngine.Random.Range(this.InitialDelay.x, this.InitialDelay.y) * -1f;
		if (this.lifetime < 0f)
		{
			base.GetComponent<CanvasGroup>().alpha = 0f;
		}
		base.Invoke(new Action(this.Die), UnityEngine.Random.Range(this.LifeTime.x, this.LifeTime.y) + this.lifetime * -1f);
	}

	private void Update()
	{
		if (this.lifetime >= 0f)
		{
			this.lifetime += Time.deltaTime;
		}
		else
		{
			this.lifetime += Time.deltaTime;
			if (this.lifetime < 0f)
			{
				return;
			}
			base.GetComponent<CanvasGroup>().alpha = 1f;
		}
		Vector3 vector3 = base.transform.position;
		Vector3 vector31 = base.transform.localScale;
		ref float singlePointer = ref this.velocity.y;
		singlePointer = singlePointer - this.gravity * Time.deltaTime;
		ref float singlePointer1 = ref vector3.x;
		singlePointer1 = singlePointer1 + this.velocity.x * Time.deltaTime;
		ref float singlePointer2 = ref vector3.y;
		singlePointer2 = singlePointer2 + this.velocity.y * Time.deltaTime;
		vector31 = vector31 + ((Vector3.one * this.scaleVelocity) * Time.deltaTime);
		if (vector31.x <= 0f || vector31.y <= 0f)
		{
			this.Die();
			return;
		}
		base.transform.position = vector3;
		base.transform.localScale = vector31;
	}
}