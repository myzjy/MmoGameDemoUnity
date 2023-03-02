using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZJYFrameWork.UI.UIModel
{
 [RequireComponent(typeof(Image))]
public abstract class SlicedImageFillBase : UIBehaviour, IMeshModifier
{
   // 極小(イコールMathf.Epsilonではない)
	const double Epsilon = 1.192093E-07;

	[SerializeField, Range(0, 1)]
	protected float fillAmount = 1.0f;
	private float tmpAmount;

	[SerializeField, Tooltip("只以Slice延伸的部分为对象")]
	private bool innerOnly = false;

	private readonly List<UIVertex> _uiVertexList = new List<UIVertex>();

	public float FillAmount
	{
		get => fillAmount;
		set
		{
			tmpAmount = Mathf.Clamp01(value);
			// 误差过小就不更新
			if (!(Mathf.Abs(fillAmount - tmpAmount) > Epsilon)) return;
			fillAmount = tmpAmount;
			if (cacheGraphic != null)
			{
				cacheGraphic.SetVerticesDirty();
			}
		}
	}

	[SerializeField, HideInInspector]
	protected RectTransform cacheRectTransform;
	[SerializeField, HideInInspector]
	protected Graphic cacheGraphic;
	[SerializeField, HideInInspector]
	protected Image cacheImage;


	private new void Awake()
	{
		base.Awake();
		CacheComponents();
	}

#if UNITY_EDITOR
	
	public new void OnValidate()
	{
		base.OnValidate();

		CacheComponents();

		if (cacheGraphic != null)
		{
			cacheGraphic.SetVerticesDirty();
		}
	}

#endif

	private void CacheComponents()
	{
		if (cacheRectTransform == null)
		{
			cacheRectTransform = GetComponent<RectTransform>();
		}
		if (cacheGraphic == null)
		{
			cacheGraphic = GetComponent<Graphic>();
		}
		if (cacheImage == null)
		{
			cacheImage = GetComponent<Image>();
		}
	}

	public void ModifyMesh(Mesh mesh) { }
	public void ModifyMesh(VertexHelper verts)
	{
		if (!this.IsActive())
		{
			return;
		}

		_uiVertexList.Clear();
		verts.GetUIVertexStream(_uiVertexList);

		ModifyVertices(_uiVertexList);

		verts.Clear();
		verts.AddUIVertexTriangleStream(_uiVertexList);
	}


	private void ModifyVertices(List<UIVertex> vertexList)
	{
		if (cacheRectTransform == null)
		{
			return;
		}
		if (cacheImage == null || cacheImage.sprite == null || !cacheImage.hasBorder)
		{
			return;
		}

		if (innerOnly)
		{
			FillInner(vertexList);
		}
		else
		{
			FillNormal(vertexList);
		}
	}

	/// <summary>
	/// 从头到尾，一边巧妙地调整UV值一边进行Fill
	/// </summary>
	protected abstract void FillNormal(List<UIVertex> vertexList);

	/// <summary>
	/// 只在Slice内部变动的Fill
	/// </summary>
	protected abstract void FillInner(List<UIVertex> vertexList);
}
}