using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UISpriteSetting : MonoBehaviour
{
	public	bool	_DestroyOnAwake;
	public	Camera	_linkedCamera;
	public	Vector3	_position = new Vector3(0.5f, 0.5f, 1.0f);
	public	Vector3	_scale = Vector3.one;
	public	AdjustPoint	_adjustPoint = AdjustPoint.None;
	public	AdjustSize	_adjustSacleAspect = AdjustSize.None;

	private	Transform	mTransform;
	private	SpriteRenderer	mRenderer;
	private	Sprite	mSprite;
	private	Vector3	mPivot;
	private	Vector3	mCenter;
	private	Vector3	mFix;
	private	Vector3	mLossyScale;
	private	float	mSpriteWidth;
	private	float	mSpriteHeight;

	private	void	Awake() {
		if (_linkedCamera == null) {
			_linkedCamera	= Camera.main;
			if (_linkedCamera == null) {
				return;
			}
		}

		mTransform	= transform;

		mRenderer	= GetComponent<SpriteRenderer>();
		if (mRenderer == null) {
			return;
		}

		mSprite	= mRenderer.sprite;
		if (mSprite == null) {
			return;
		}
		
		AdjustScale();
		AdjustPosition();

		if (_DestroyOnAwake && Application.isPlaying) {
			Destroy(this);
		}
	}

	private	void	AdjustPosition() {
		mLossyScale		= mTransform.lossyScale;
		mCenter			= mSprite.bounds.center;
		mSpriteWidth	= mSprite.bounds.size.x * mLossyScale.x;
		mSpriteHeight	= mSprite.bounds.size.y * mLossyScale.y;

		switch (_adjustPoint) {
		case AdjustPoint.None:
			return;
		case AdjustPoint.Center_Center:
			mPivot	= mCenter;
			break;
		case AdjustPoint.Center_Left:
			mFix.Set(mSpriteWidth*-0.5f, 0.0f, 0.0f);
			mPivot	= mCenter + mFix;
			break;
		case AdjustPoint.Center_Righy:
			mFix.Set(mSpriteWidth*0.5f, 0.0f, 0.0f);
			mPivot	= mCenter + mFix;
			break;
		case AdjustPoint.Up_Center:
			mFix.Set(0.0f, mSpriteHeight*0.5f, 0.0f);
			mPivot	= mCenter + mFix;
			break;
		case AdjustPoint.Up_Left:
			mFix.Set(mSpriteWidth*-0.5f, mSpriteHeight*0.5f, 0.0f);
			mPivot	= mCenter + mFix;
			break;
		case AdjustPoint.Up_Right:
			mFix.Set(mSpriteWidth*0.5f, mSpriteHeight*0.5f, 0.0f);
			mPivot	= mCenter + mFix;
			break;
		case AdjustPoint.Down_Center:
			mFix.Set(0.0f, mSpriteHeight*-0.5f, 0.0f);
			mPivot	= mCenter + mFix;
			break;
		case AdjustPoint.Down_Left:
			mFix.Set(mSpriteWidth*-0.5f, mSpriteHeight*-0.5f, 0.0f);
			mPivot	= mCenter + mFix;
			break;
		case AdjustPoint.Down_right:
			mFix.Set(mSpriteWidth*0.5f, mSpriteHeight*-0.5f, 0.0f);
			mPivot	= mCenter + mFix;
			break;
		}
		mTransform.position	= _linkedCamera.ViewportToWorldPoint(_position) - mPivot;
	}

	private	void	AdjustScale() {
		float	aOrthographicSize	= _linkedCamera.orthographicSize * 2.0f;
		switch (_adjustSacleAspect) {
		case AdjustSize.None:
			return;
		case AdjustSize.WidthAndHeight:
			mFix.Set(_scale.x, _scale.y, 1.0f);
			mFix.x	*= aOrthographicSize * _linkedCamera.aspect;
			mFix.x	*= (1.0f/mSprite.bounds.size.x);
			mFix.y	*= aOrthographicSize;
			mFix.y	*= (1.0f/mSprite.bounds.size.y);
			break;
		case AdjustSize.Width:
			mFix.Set(_scale.x, mTransform.lossyScale.y, 1.0f);
			mFix.x	*= aOrthographicSize * _linkedCamera.aspect;
			mFix.x	*= (1.0f/mSprite.bounds.size.x);
			break;
		case AdjustSize.Height:
			mFix.Set(mTransform.lossyScale.x, _scale.y, 1.0f);
			mFix.y	*= aOrthographicSize;
			mFix.y	*= (1.0f/mSprite.bounds.size.y);
			break;
		case AdjustSize.Width_Maintain:
			mFix.Set(_scale.x, 1.0f, 1.0f);
			mFix.x	*= aOrthographicSize * _linkedCamera.aspect;
			mFix.x	*= (1.0f/mSprite.bounds.size.x);
			mFix.y	= mFix.x;
			break;
		case AdjustSize.Height_Maintain:
			mFix.Set(1.0f, _scale.y, 1.0f);
			mFix.y	*= aOrthographicSize;
			mFix.y	*= (1.0f/mSprite.bounds.size.y);
			mFix.x	= mFix.y;
			break;
		}
		SetLossyScale(mTransform, mFix.x, mFix.y, mFix.z);
	}

	private	Vector3	SetLossyScale(Transform iTransform, float iX, float iY, float iZ) {
		Vector3	aVector3	= Vector3.zero;

		aVector3	= iTransform.lossyScale;
		aVector3.x	= (iTransform.localScale.x / aVector3.x) * iX;
		aVector3.y	= (iTransform.localScale.y / aVector3.y) * iY;
		aVector3.z	= (iTransform.localScale.z / aVector3.z) * iZ;
		
		if (float.IsNaN(aVector3.x)) {
			aVector3.x	= 0.0f;
		}
		
		if (float.IsNaN(aVector3.y)) {
			aVector3.y	= 0.0f;
		}
		
		if (float.IsNaN(aVector3.z)) {
			aVector3.z	= 0.0f;
		}
		
		iTransform.localScale	= aVector3;
		return	aVector3;
	}

	private	void	Update() {
		if (mSprite == null) {
#if UNITY_EDITOR
			Debug.LogWarning("Not found a sprite!" + " ("+gameObject.name+":"+gameObject.GetInstanceID()+")");
#endif
			return;
		}
		if (_linkedCamera == null) {
			_linkedCamera	= Camera.main;
			if (_linkedCamera == null) {
				return;
			}
		}

		AdjustScale();
		AdjustPosition();
	}

	public	enum AdjustPoint {
		None,
		Center_Center,
		Center_Righy,
		Center_Left,
		Up_Center,
		Up_Left,
		Up_Right,
		Down_Center,
		Down_Left,
		Down_right,
	}

	public	enum AdjustSize {
		None,
		WidthAndHeight,
		Width,
		Height,
		Width_Maintain,
		Height_Maintain,
	}

}
