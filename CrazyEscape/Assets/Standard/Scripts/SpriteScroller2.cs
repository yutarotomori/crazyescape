using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpriteScroller2 : MonoBehaviour {
	public	Transform	_parent;
	public	Vector3		_direction;
	public	SortType	_sortType;
	public	bool		_isLocked;

	#region adjust side members
	public	AdjustSide	_adjustSide;
	public	FixType		_fixPositionType;
	public	Vector3		_fixPosition;
	#endregion

	#region scaling members
	public	float	_scalingStrength = 1.0f;
	public	float	_maxSize	= 2.0f;
	public	float	_minSize	= 1.0f;
	#endregion

	#region moving members
	public	float	_space		= 0.5f;
	public	float	_trackSpeed	= 0.35f;
	public	float	_floatIndex	= 0.0f;
	public	float	_targetFloatIndex	= 0.0f;
	public	bool	_useLimit;
	public	float	_leftLimit;
	public	float	_rightLimit;
	public	bool	_useMoveToCenter	= true;
	#endregion

	#region accelerate members
	public	float	_speed	= 0.0f;
	public	float	_accelerationRate	= 0.981f;
	public	float	_accuracyAccelerate	= 0.001f;
	public	float	_bounciness	= 1.0f;
	public	float	_adsorptionForce;//0.0f ~ 1.0f
	#endregion

	#region self touch members
	public	bool	_selfTouchMove;
	public	Camera	_camera;
	public	float	_touchSensitivity = 0.015f;
	public	bool	_useInfluence;
	public	Rect	_influenceArea	= new Rect(0.0f, 0.2f, 1.0f, 0.6f);
	private	int		mFingerId;
	private	Vector2	mMovement;
	private	Vector3	mOldMousePosition;
	private	Vector3	mCurrentMousePosition;
	#endregion

	private	List<ChildItem>	mChildList;


	public	void	outSort (Comparison<Transform> iComp) {
		List<Transform> aTList	= new List<Transform>();
		foreach (ChildItem aItem in mChildList) {
			aTList.Add (aItem.transform);
		}

		aTList.Sort (iComp);
		mChildList.Clear ();
		mChildList	= new List<ChildItem>();

		foreach (Transform aItem in aTList) {
			mChildList.Add (new ChildItem (aItem));
		}
	}


	private	void	Start () {
		if (_parent == null) {
			_parent	= transform;
		}

		initChildList ();
		alignChildList ();
	}

	private	void	initChildList () {
		mChildList	= new List<ChildItem>();
		if (_parent.childCount != 0) {
			ChildItem	aItem;
			for (int aIndex = 0; aIndex < _parent.childCount; aIndex++) {
				if (_parent.GetChild(aIndex).gameObject.activeSelf == false) {
					continue;
				}
				aItem	= new ChildItem(_parent.GetChild(aIndex));
				mChildList.Add (aItem);
			}
		}

		Sort ();
	}

	private	void	Sort () {
		switch (_sortType) {
		case SortType.Name:
			mChildList.Sort ((ChildItem a, ChildItem b) => string.Compare (a.transform.name, b.transform.name));
			break;
		case SortType.InstanceID:
			mChildList.Sort ((ChildItem a, ChildItem b) =>  a.transform.GetInstanceID() - b.transform.GetInstanceID());
			break;
		}
	}

	private	void	LateUpdate () {
		if (checkChildCount () == false) {
			initChildList ();
		}

		if (mChildList.Count <= 0) {
			return;
		}

		if (_isLocked) {
			return;
		}

		if (_speed == 0.0f) {
			adsorption ();
		}

		if (_selfTouchMove) {
			updateSelfTouchMove ();
		}
		accelerateMove ();
		move ();

		if (!_useLimit) {
			_floatIndex	= Mathf.Clamp (_floatIndex, 0.0f, mChildList.Count-1);
			_targetFloatIndex	= Mathf.Clamp (_targetFloatIndex, 0.0f, mChildList.Count-1);
		} else {
			_floatIndex	= Mathf.Clamp (_floatIndex
			                           ,	Mathf.Max(_leftLimit, 0.0f)
			                           ,	Mathf.Min(_rightLimit, mChildList.Count-1));
			_targetFloatIndex	= Mathf.Clamp (_targetFloatIndex
			                                 ,	Mathf.Max(_leftLimit, 0.0f)
			                                 ,	Mathf.Min(_rightLimit, mChildList.Count-1));
		}
		bounce ();

		scalingChildList ();
		alignChildList ();
		if (_useMoveToCenter == true) {
			alignChioldListToCenter ();
		}
		adjustSide ();
	}

	private	bool	checkChildCount () {
		if (this.mChildList == null) {
			return	false;
		}

		int	aChildCount	= 0;

		for (int aIndex = 0; aIndex < _parent.childCount; aIndex++) {
			if (_parent.GetChild(aIndex).gameObject == null) {
				continue;
			}
			
			bool aWasFound	= false;
			for (int aIndex2 = 0; aIndex2 < mChildList.Count; aIndex2++) {
				if (mChildList[aIndex2].transform == _parent.GetChild(aIndex)) {
					aWasFound = true;
					break;
				}
			}
			if (aWasFound == false) {
				return	false;
			}

			aChildCount++;
		}

		if (mChildList.Count != aChildCount) {
			return	false;
		} else {
			return	true;
		}
	}
	
	private	void	adsorption () {
		_adsorptionForce	= Mathf.Clamp (_adsorptionForce, 0.0f, 1.0f);
		if (_targetFloatIndex % 1.0f != 0.0f) {
			if (_targetFloatIndex % 1.0f < 0.5f) {
				_targetFloatIndex	-= (_targetFloatIndex%1.0f)*_adsorptionForce;
			} else {
				_targetFloatIndex	+= (1.0f-_targetFloatIndex%1.0f)*_adsorptionForce;
			}
		}
	}
	
	private	void	updateSelfTouchMove () {
		if (_camera == null) {
			_camera = Camera.main;
		}

		drawRect (_influenceArea, _camera);

#if UNITY_EDITOR || UNITY_WEBPLAYER
		mouseTouchMove ();
#else
		
		if (mFingerId == -1) {
			mFingerId	= getTouchFingerId (TouchPhase.Began, true);
		}
		
		if (mFingerId != -1) {
			Touch? aTouch	= getTouch (mFingerId);
			if (	aTouch == null
			    ||	aTouch.Value.phase == TouchPhase.Ended) {
				_speed	= Vector3.Scale (_direction, mMovement).magnitude
					* Mathf.Sign (Vector3.Angle(_direction,mMovement)-90.0f)
						* _touchSensitivity;
				mFingerId	= -1;
			} else {
				mMovement	= aTouch.Value.deltaPosition;
				_floatIndex	+= Vector3.Scale (_direction, mMovement).magnitude
					* Mathf.Sign (Vector3.Angle(_direction,mMovement)-90.0f)
						* _touchSensitivity;
				_targetFloatIndex	= _floatIndex;
				_speed	= 0.0f;
			}
		} else {
			mFingerId	= -1;
			mMovement	= Vector2.zero;
		}
#endif
	}
	
	private	void	accelerateMove () {
		_floatIndex	+= _speed;
		_targetFloatIndex	+= _speed;
		_speed	*= _accelerationRate;
		
		if (	_speed < Mathf.Abs (_accuracyAccelerate)
		    &&	_speed > -Mathf.Abs (_accuracyAccelerate)) {
			_speed	= 0.0f;
		}
	}
	
	private	void	move () {
		if (_targetFloatIndex != _floatIndex) {
			_floatIndex	+=	(_targetFloatIndex - _floatIndex)*_trackSpeed;
		}
	}
		
	private	void	bounce () {
		if (_floatIndex == 0.0f || _floatIndex == mChildList.Count-1) {
			_speed	*= -1.0f*_bounciness;
		}
	}
	
	private	void	adjustSide () {
		Sprite	aSprite;
		Vector3	aLossyScale		= Vector3.zero;
		Vector3	aSpriteCenter	= Vector3.zero;
		Vector3	aPivot	= Vector3.zero;
		float	aSpriteWidth	= 0.0f;
		float	aSpriteHeight	= 0.0f;
		
		for (int aIndex = 0; aIndex < mChildList.Count; aIndex++) {

			aSprite	= mChildList[aIndex].renderer.sprite;
			aLossyScale		= mChildList[aIndex].transform.localScale;
			aSpriteCenter	= aSprite.bounds.center;
			aSpriteWidth	= aSprite.bounds.size.x * aLossyScale.x;
			aSpriteHeight	= aSprite.bounds.size.y * aLossyScale.y;

			switch (_adjustSide) {
			case AdjustSide.Center:
				aPivot	= aSpriteCenter + Vector3.zero;
				break;
			case AdjustSide.Up:
				aPivot	= aSpriteCenter + Vector3.up * aSpriteHeight * -0.5f;
				break;
			case AdjustSide.Bottom:
				aPivot	= aSpriteCenter + Vector3.down * aSpriteHeight * -0.5f;
				break;
			case AdjustSide.Right:
				aPivot	= aSpriteCenter + Vector3.right * aSpriteWidth * -0.5f;
				break;
			case AdjustSide.Left:
				aPivot	= aSpriteCenter + Vector3.left * aSpriteWidth * -0.5f;
				break;
			case AdjustSide.UpRight:
				aPivot	= aSpriteCenter + (Vector3.up*aSpriteHeight + Vector3.right*aSpriteWidth) * -0.5f;
				break;
			case AdjustSide.UpLeft:
				aPivot	= aSpriteCenter + (Vector3.up*aSpriteHeight + Vector3.left*aSpriteWidth) * -0.5f;
				break;
			case AdjustSide.BottmRight:
				aPivot	= aSpriteCenter + (Vector3.down*aSpriteHeight + Vector3.right*aSpriteWidth) * -0.5f;
				break;
			case AdjustSide.BottomLeft:
				aPivot	= aSpriteCenter + (Vector3.down*aSpriteHeight + Vector3.left*aSpriteWidth) * -0.5f;
				break;
			}
			mChildList[aIndex].transform.localPosition += aPivot;
//			mChildList[aIndex].centerPoint	= aPivot;
		}
	}
	
	private	void	scalingChildList () {
		float	aIndex = 0.0f;
		foreach (ChildItem aChild in mChildList) {
			aChild.transform.localScale	= Vector3.one *
				(
					_maxSize-(_maxSize-_minSize)*Mathf.Abs(aIndex-_floatIndex)*_scalingStrength
				);
			Vector3	aScale	= aChild.transform.localScale;
			aScale.x	= Mathf.Clamp (aScale.x, Mathf.Min (_minSize, _maxSize), Mathf.Max (_minSize, _maxSize));
			aScale.y	= Mathf.Clamp (aScale.y, Mathf.Min (_minSize, _maxSize), Mathf.Max (_minSize, _maxSize));
			aScale.z	= Mathf.Clamp (aScale.z, Mathf.Min (_minSize, _maxSize), Mathf.Max (_minSize, _maxSize));
			aChild.transform.localScale	= aScale;
			aIndex++;
		}
	}
	
	private	void	alignChildList () {
		int	aIndex = 0;
		Vector3	aPrevSide	= Vector3.zero;
		Vector3	aCurSide	= Vector3.zero;
//		Vector3	aFixSide	= Vector3.zero;
		foreach (ChildItem aChild in mChildList) {
			aCurSide	= Vector3.Scale (	aChild.renderer.sprite.bounds.size
			                          ,	aChild.transform.localScale) * 0.5f;
			aCurSide	= Vector3.Scale (aCurSide, _direction);
			//aCurSide	+= aChild.centerPoint;

			if (aIndex != 0) {
				//aFixSide = (mChildList[(int)(_floatIndex+1.0f)].centerPoint
				  //          + mChildList[(int)_floatIndex].centerPoint) * 0.5f;
				aChild.transform.localPosition	= aCurSide + aPrevSide  + _direction * _space;
			} else {
				aChild.transform.localPosition	= aPrevSide;
			}
			
			if (_fixPositionType == FixType.Linear) {
				aPrevSide	=	aChild.transform.localPosition
						+	aCurSide + _fixPosition
						*	Mathf.Clamp(1.0f-(_floatIndex-aIndex+0.5f),-1.0f,1.0f);
			} else {
				aPrevSide	= aChild.transform.localPosition + aCurSide + _fixPosition*(aIndex-_floatIndex+0.5f);
			}
//			aPrevSide	-= aFixSide;
//			aChild.transform.localPosition	+= aChild.centerPoint;
			aIndex++;
		}
	}

	private	void	alignChioldListToCenter () {
		Vector3	aFixPos;
		if (_floatIndex < mChildList.Count-1) {
			aFixPos	=	mChildList[(int)_floatIndex].transform.localPosition
					+ (mChildList[(int)(_floatIndex+1.0f)].transform.localPosition
				   	- mChildList[(int)_floatIndex].transform.localPosition) * (_floatIndex%1.0f);

//			aFixPos -=	(mChildList[(int)(_floatIndex+1.0f)].centerPoint
//					  	+ mChildList[(int)_floatIndex].centerPoint) * 0.5f;
		} else {
			aFixPos	= mChildList[(int)_floatIndex].transform.localPosition;
		}
		foreach (ChildItem aChild in mChildList) {
			aChild.transform.localPosition	-= aFixPos;
		}
	}

	private	void	mouseTouchMove () {
		if (Input.GetMouseButtonDown (0)
		    && isInfluenceArea (Input.mousePosition) == true) {
			mOldMousePosition	= Input.mousePosition;
			mCurrentMousePosition	= mOldMousePosition;
			mFingerId	= 1;
		}

		if (Input.GetMouseButtonUp (0) && mFingerId == 1) {
			_speed	= Vector3.Scale (_direction, mMovement).magnitude
					* Mathf.Sign (Vector3.Angle (_direction,mMovement)-90.0f)
					* _touchSensitivity;
			mFingerId	= -1;
		} else if (Input.GetMouseButton (0) && mFingerId == 1) {
			mCurrentMousePosition	= Input.mousePosition;
			mMovement	= (mCurrentMousePosition - mOldMousePosition) * 0.5f;
			mOldMousePosition	= mCurrentMousePosition;

			_floatIndex	+= Vector3.Scale (_direction, mMovement).magnitude
						* Mathf.Sign (Vector3.Angle (_direction,mMovement)-90.0f)
						* _touchSensitivity;
			_targetFloatIndex	= _floatIndex;
			_speed	= 0.0f;
			mFingerId	= 1;
		} else {
			mMovement	= Vector2.zero;
			mCurrentMousePosition	= Vector3.zero;
		}
	}
	
	private	int		getTouchFingerId (TouchPhase iPhase, bool iCheckInfluence = false) {
		for (int aIndex = 0; aIndex < Input.touchCount; aIndex++) {
			if (	Input.GetTouch (aIndex).phase == iPhase) {
				if (	iCheckInfluence == true
				    &&	isInfluenceArea (Input.GetTouch(aIndex).position) == false) {
					return	-1;
				}
				return	Input.GetTouch (aIndex).fingerId;
			}
		}
		return	-1;
	}
	
	private	bool	isInfluenceArea (Vector3 iScreenoint) {
		if (_useInfluence == false) {
			return	true;
		}

		if (	s2v (iScreenoint).x > _influenceArea.x
		    &&	s2v (iScreenoint).x < _influenceArea.x+_influenceArea.width
		    &&	s2v (iScreenoint).y > _influenceArea.y
		    &&	s2v (iScreenoint).y < _influenceArea.y+_influenceArea.height) {
			return	true;
		}
		return	false;
	}

	private	Vector3	s2v (Vector3 iScreenPoint) {
		return	_camera.ScreenToViewportPoint (iScreenPoint);
	}

	// nullable
	private	Touch? getTouch (int iFingerId) {
		foreach (Touch aTouch in Input.touches) {
			if (aTouch.fingerId == iFingerId) {
				return	aTouch;
			}
		}
		return	null;
	}
	
	private	void	drawRect(Rect rect, Camera iCamera) {
		Vector3	aRightUp		= iCamera.ViewportToWorldPoint (new Vector3(rect.x + rect.width, rect.y + rect.height, 1.0f));
		Vector3	aLeftUp			= iCamera.ViewportToWorldPoint (new Vector3(rect.x, rect.y + rect.height, 1.0f));
		Vector3	aRightBottom	= iCamera.ViewportToWorldPoint (new Vector3(rect.x + rect.width, rect.y, 1.0f));
		Vector3	aLeftBottom		= iCamera.ViewportToWorldPoint (new Vector3(rect.x, rect.y, 1.0f));
		
		Debug.DrawLine(aRightUp, aLeftUp);
		Debug.DrawLine(aRightUp, aRightBottom);
		Debug.DrawLine(aLeftBottom, aLeftUp);
		Debug.DrawLine(aLeftBottom, aRightBottom);
	}

	#region class | struct | enum
	private	class ChildItem {
		public	Transform transform {
			get;
			private	set;
		}
		public	SpriteRenderer	renderer {
			get;
			private	set;
		}
		public	Vector3	centerPoint {
			get;
			set;
		}

		// Constructor
		public ChildItem (Transform iTransform) {
			transform	= iTransform;
			renderer	= transform.GetComponent<SpriteRenderer>();
		}
	}

	public enum AdjustSide {
		Center,
		Up,
		Bottom,
		Right,
		Left,
		UpRight,
		UpLeft,
		BottmRight,
		BottomLeft,
	}

	public enum FixType {
		Linear,
		Smooth,
	}

	public enum SortType {
		None,
		Name,
		InstanceID,
	}
	#endregion
}
