/*
 * * * Change Logs * * * 
 * __________________________
 * Thuseday/August/21/2014
 * * Add "bool IsTapped(out int oFingerID)"
 * __________________________
 *  Thuseday/July/31/2014
 * * Add "int TouchCount()"
 * * Add "int TouchSensorCount()"
 * __________________________
 * Tuesday/June/18/2014
 * * Add "bool IsTouched(Transform iTransform, Camera iCamera, int iFingerID, int iLayerMask)"
 * * Fix "bool IsTouched(Transform iTransform, Camera iCamera, int iLayerMask)"
 * __________________________
 * Tuesday/January/27/2015
 * * A method "Start ()" called by a method "Initialize () (overrided)" and rename to "start ()"
 * __________________________
 * Monday/February/9/2015
 * * Add Generics "T TouchComponent()"
 * __________________________
 * June/Tuesday/30/2015
 * * Add "void SetActive(bool)" and "bool IsActive()"
 * __________________________
 * July/Friday/31/2015
 * * Add "float TouchTime()" and "float TouchTime(int)"
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchManager : Singleton<TouchManager>
{
	public	int			touchCountMax		= 5;
	public	float		tapTimeLimit		= 2.0f;
	public	float		flickTimeLimit		= 2.0f;
	public	float		flickSensitivity	= 1000.0f;
	public	bool		useMouse			= true;
	public	int			mouseButtonCount	= 2;
	
	private	TouchAndMouse[]		mTouchSensors;
	private	bool	mIsActive	= true;

	public	bool		IsActive ()
	{
		return	mIsActive;
	}

	public	void		SetActive (bool iIsActive)
	{
		mIsActive	= iIsActive;
	}

	public	int			TouchSensorCount()
	{
		return	mTouchSensors.Length;
	}

	public	int			TouchCount()
	{
		int	aTouchCount	= 0;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (mTouchSensors[aIndex].isActive) {
				aTouchCount++;
			}
		}
		return	aTouchCount;
	}

	public	bool		IsTapped()
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (IsTapped(aIndex)) {
				return	true;
			}
		}
		return	false;
	}

	public	float		TouchTime ()
	{
		float	aTouchTime		= -1.0f;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			aTouchTime		= TouchTime (aIndex);
			if (aTouchTime != -1.0f) {
				break;
			}
		}
		return	aTouchTime;
	}

	public	float		TouchTime (int iFingerID)
	{
		if (mTouchSensors[iFingerID].isActive) {
			return	(Time.realtimeSinceStartup - mTouchSensors[iFingerID].touchBeganTime);
		}
		return	-1.0f;
	}

	public	bool		IsTapped(out int oFingerID)
	{
		oFingerID	= -1;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (IsTapped(aIndex)) {
				oFingerID	= aIndex;
				return	true;
			}
		}
		return	false;
	}

	public	bool		IsTapped(int iFingerID)
	{
		if (	mTouchSensors[iFingerID].isActive
			&&	mTouchSensors[iFingerID].touchDeltaTime > 0.0f
		    &&	mTouchSensors[iFingerID].touchDeltaTime < tapTimeLimit
			&&	mTouchSensors[iFingerID].movement.sqrMagnitude < flickSensitivity) {
			return	true;
		}
		return	false;
	}

	public	bool		IsTapped(Transform iTransform)
	{
		List<Camera>	aAllCamera	= new List<Camera>();
		foreach (Camera aCamera in Camera.allCameras) {
			aAllCamera.Add (aCamera);
		}
		aAllCamera.Sort ((Camera a, Camera b) => (int)b.depth - (int)a.depth);
		for (int aIndex = 0; aIndex < aAllCamera.Count; aIndex++) {
			if (IsTapped(iTransform, aAllCamera[aIndex])) {
				return	true;
			}
		}
		return	false;
	}

	public	bool		IsTapped(Transform iTransform, Camera iCamera)
	{
		return	IsTapped(iTransform, iCamera, ~0);
	}

	public	bool		IsTapped(Transform iTransform, Camera iCamera, int iLayerMask)
	{
		if (iCamera.orthographic) {
			for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
				Vector3		aTouchViewPoint;

				aTouchViewPoint		= iCamera.ScreenToViewportPoint(mTouchSensors[aIndex].position);
				if (	aTouchViewPoint.x < 0.0f
				    ||	aTouchViewPoint.x > 1.0f
				    ||	aTouchViewPoint.y < 0.0f
				    ||	aTouchViewPoint.y > 1.0f) {

					continue;
				}

				if (IsTapped(aIndex)) {

					Ray				aRay;
					RaycastHit		aHit;

					aRay	= iCamera.ScreenPointToRay(mTouchSensors[aIndex].position);
					if (Physics.Raycast(aRay, out aHit, 1000.0f, iLayerMask)) {
						if (aHit.transform		== iTransform) {
							return	true;
						}
					}
				}
			}
		}
		return	false;
	}

	public	bool		IsFlicked()
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (IsFlicked(aIndex)) {
				return	true;
			}
		}
		return	false;
	}

	public	bool		IsFlicked(int iFingerID)
	{
		if (	mTouchSensors[iFingerID].isActive
			&&	mTouchSensors[iFingerID].movement.sqrMagnitude > flickSensitivity 
			&&	mTouchSensors[iFingerID].touchDeltaTime > 0.0f
			&&	mTouchSensors[iFingerID].touchDeltaTime < flickTimeLimit) {
			return	true;
		}

		return	false;
	}

	public	bool		IsTouched()
	{
		for(int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (IsTouched(aIndex)) {
				return	true;
			}
		}
		return	false;
	}

	public	bool		IsTouched(out int oFingerID)
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (IsTouched(aIndex)) {
				oFingerID	= aIndex;
				return	true;
			}
		}
		oFingerID	= -1;
		return	false;
	}

	public	bool		IsTouched(int iFingerID)
	{
		TouchPhase	aPhase	= TouchPhase.Ended;
		return	IsTouched(iFingerID, out aPhase);
	}

	public	bool		IsTouched(int iFingerID, out TouchPhase oPhase)
	{
		if (	iFingerID >= 0
			&&	iFingerID < mTouchSensors.Length) {
			if (mTouchSensors[iFingerID].isActive) {
				oPhase	= mTouchSensors[iFingerID].phase;
				return	true;
			}
		}
		oPhase	= TouchPhase.Ended;
		return	false;
	}

	public	bool		IsTouched(Transform iTransform)
	{
		List<Camera>	aAllCamera	= new List<Camera>();
		foreach (Camera aCamera in Camera.allCameras) {
			aAllCamera.Add (aCamera);
		}
		aAllCamera.Sort ((Camera a, Camera b) => (int)b.depth - (int)a.depth);
		for (int aIndex = 0; aIndex < aAllCamera.Count; aIndex++) {
			if (IsTouched(iTransform, aAllCamera[aIndex])) {
				return	true;
			}
		}
		return	false;
	}

	public	bool		IsTouched(Transform iTransform, Camera iCamera)
	{
		return	IsTouched(iTransform, iCamera, ~0);
	}

	public	bool		IsTouched(Transform iTransform, Camera iCamera, int iLayerMask)
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (IsTouched(iTransform, iCamera, aIndex, iLayerMask)) {
				return	true;
			}
		}
		return	false;
	}

	public	bool		IsTouched(Transform iTransform, Camera iCamera, int iFingerID, int iLayerMask)
	{
		if (iCamera.orthographic) {
			if (!mTouchSensors[iFingerID].isActive) {
				return	false;
			}
			Ray				aRay;
			
			aRay	= iCamera.ScreenPointToRay(mTouchSensors[iFingerID].position);
			RaycastHit[]	aHits	= Physics.RaycastAll (aRay, 1000.0f, iLayerMask);
			foreach (RaycastHit aHit in aHits) {
				if (aHit.transform		== iTransform) {
					return	true;
				}
			}
		}
		return	false;
	}

	public	bool		EqualPhase(TouchPhase iPhase)
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (EqualPhase(iPhase, aIndex)) {
				return	true;
			}
		}
		return	false;
	}

	public	bool		EqualPhase(TouchPhase iPhase, int iFingerID)
	{
		if (	iFingerID >= 0
			&&	mTouchSensors[iFingerID].isActive) {
			if (mTouchSensors[iFingerID].phase == iPhase) {
				return	true;
			}
 		}
		return	false;
	}

	public	bool		EqualPhase(TouchPhase iPhase, out int oFingerID)
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (mTouchSensors[aIndex].isActive) {
				if (mTouchSensors[aIndex].phase	== iPhase) {
					oFingerID	= aIndex;
					return	true;
				}
			}
		}
		oFingerID	= -1;
		return	false;
	}

	public	Vector2		Movement()
	{
		Vector2		aMovement		= Vector2.zero;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			aMovement		= Movement(aIndex);
			if (aMovement != Vector2.zero) {
				break;
			}
		}
		return	aMovement;
	}

	public	Vector2		Movement(int iFingerID)
	{
		if (mTouchSensors[iFingerID].isActive) {
			return	mTouchSensors[iFingerID].movement;
		}
		return	Vector2.zero;
	}

	public	Vector2		Swipe4Direction()
	{
		Vector2		aDirection	= Vector2.zero;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			aDirection	= Swipe4Direction(aIndex);
			if (aDirection == Vector2.zero) {
				continue;
			}
			return	aDirection;
		}
		return	Vector2.zero;
	}

	public	Vector2		Swipe4Direction(int iFingerID)
	{
		Vector2		aDirection;
		
		aDirection		= SwipeDirection(iFingerID);
		if (aDirection	== Vector2.zero) {
			return	aDirection;
		}
		
		if (Mathf.Abs(aDirection.x) > Mathf.Abs(aDirection.y)) {
			return	Vector2.right * Mathf.Sign(aDirection.x);
		} else {
			return	Vector2.up * Mathf.Sign(aDirection.y);
		}
	}

	public	Vector2		SwipeDirection()
	{
		Vector2		aSwipeDirection		= Vector2.zero;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			aSwipeDirection		= SwipeDirection(aIndex);
			if (aSwipeDirection != Vector2.zero) {
				break;
			}
		}
		return	aSwipeDirection;
	}

	public	Vector2		SwipeDirection(int iFingerID)
	{
		if (mTouchSensors[iFingerID].isActive) {
			return		mTouchSensors[iFingerID].moveDirection;
		}
		return	Vector2.zero;
	}

	public	Vector2		Flick4Direction()
	{
		Vector2	aDirection	= Vector2.zero;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			aDirection	= Flick4Direction(aIndex);
			if (aDirection	== Vector2.zero) {
				continue;
			}
			return	aDirection;
		}
		return	Vector2.zero;
	}

	public	Vector2		Flick4Direction(int iFingerID)
	{
		Vector2		aDirection;

		aDirection	= FlickDirection(iFingerID);
		if (aDirection	== Vector2.zero) {
			return	aDirection;
		}

		if (Mathf.Abs(aDirection.x) > Mathf.Abs(aDirection.y)) {
			return	Vector2.right * Mathf.Sign(aDirection.x);
		} else {
			return	Vector2.up * Mathf.Sign(aDirection.y);
		}
	}

	public	Vector2		FlickDirection()
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (FlickDirection(aIndex)	== Vector2.zero) {
				continue;
			}

			return	FlickDirection(aIndex);
		}

		return	Vector2.zero;
	}

	public	Vector2		FlickDirection(int iFingerID)
	{
		if (IsFlicked(iFingerID)) {
			return	mTouchSensors[iFingerID].deltaPosition;
		}

		return	Vector2.zero;
	}

	public	Vector2		TouchPosition()
	{
		Vector2		aPosition	= Vector2.zero;
		
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (mTouchSensors[aIndex].isActive) {
				aPosition	= mTouchSensors[aIndex].position;
				break;
			}
		}
		return	aPosition;
	}

	public	Vector2		TouchPosition(int iFingerID) {
		return	mTouchSensors[iFingerID].position;
	}

	public	Transform	TappedTransform()
	{
		Transform	aTappedTransform;

		aTappedTransform	= null;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			aTappedTransform	= TappedTransform(aIndex);
			if (aTappedTransform	!= null) {
				break;
			}
		}

		return	aTappedTransform;
	}

	public	Transform	TappedTransform(Camera iCamera)
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (!mTouchSensors[aIndex].isActive) {
				continue;
			}
			return	TappedTransform(iCamera, aIndex, ~0);
		}

		return	null;
	}

	public	Transform	TappedTransform(int iFingerID)
	{
		return	TappedTransform(iFingerID, ~0);
	}

	public	Transform	TappedTransform(Camera iCamera, int iFingerID)
	{
		return	TappedTransform(iCamera, iFingerID, ~0);
	}

	public	Transform	TappedTransform(int iFingerID, int iLayerMask)
	{
		if (!mTouchSensors[iFingerID].isActive || !IsTapped(iFingerID)) {
			return	null;
		}

		Transform	aTappedTransform;

		List<Camera>	aAllCamera	= new List<Camera>();
		foreach (Camera aCamera in Camera.allCameras) {
			aAllCamera.Add (aCamera);
		}
		aAllCamera.Sort ((Camera a, Camera b) => (int)b.depth - (int)a.depth);

		aTappedTransform	= null;
		for (int aIndex = 0; aIndex < aAllCamera.Count; aIndex++) {
			aTappedTransform	= TappedTransform(aAllCamera[aIndex], iFingerID, iLayerMask);
			if (aTappedTransform != null) {
				return	aTappedTransform;
			}
		}
		return	null;
	}

	public	Transform	TappedTransform(Camera iCamera, int iFingerID, int iLayerMask)
	{
		if (!mTouchSensors[iFingerID].isActive || !IsTapped(iFingerID)) {
			return	null;
		}

		Ray				aRay;
		RaycastHit		aHit;

		aRay	= iCamera.ScreenPointToRay(mTouchSensors[iFingerID].position);
		if (Physics.Raycast(aRay, out aHit, 1000.0f, iLayerMask)) {
			return	aHit.transform;
		}

		return	null;
	}


	public	T	TouchComponent<T> () where T : Component
	{
		T aTouchComponent;
		
		aTouchComponent	= null;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			aTouchComponent	= TouchComponent<T>(aIndex);
			if (aTouchComponent	!= null) {
				break;
			}
		}
		
		return	aTouchComponent;
	}
	
	public	T	TouchComponent<T> (Camera iCamera) where T : Component
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (!mTouchSensors[aIndex].isActive) {
				continue;
			}
			return	TouchComponent<T>(iCamera, aIndex, ~0);
		}
		
		return	null;
	}
	
	public	T	TouchComponent<T> (int iFingerID) where T : Component
	{
		return	TouchComponent<T>(iFingerID, ~0);
	}
	
	public	T	TouchComponent<T> (Camera iCamera, int iFingerID) where T : Component
	{
		return	TouchComponent<T>(iCamera, iFingerID, ~0);
	}
	
	public	T	TouchComponent<T> (int iFingerID, int iLayerMask) where T : Component
	{
		if (!mTouchSensors[iFingerID].isActive || !IsTouched(iFingerID)) {
			return	null;
		}
		
		T	aTouchComponent;
		
		List<Camera>	aAllCamera	= new List<Camera>();
		foreach (Camera aCamera in Camera.allCameras) {
			aAllCamera.Add (aCamera);
		}
		aAllCamera.Sort ((Camera a, Camera b) => (int)b.depth - (int)a.depth);
		
		aTouchComponent	= null;
		for (int aIndex = 0; aIndex < aAllCamera.Count; aIndex++) {
			aTouchComponent	= TouchComponent<T>(aAllCamera[aIndex], iFingerID, iLayerMask);
			if (aTouchComponent != null) {
				return	aTouchComponent;
			}
		}
		return	null;
	}
	
	public	T	TouchComponent<T> (Camera iCamera, int iFingerID, int iLayerMask) where T : Component
	{
		if (iCamera == null || !mTouchSensors[iFingerID].isActive || !IsTouched(iFingerID)) {
			return	null;
		}

		Ray				aRay;
		RaycastHit		aHit;
		
		aRay	= iCamera.ScreenPointToRay(mTouchSensors[iFingerID].position);
		if (Physics.Raycast(aRay, out aHit, 1000.0f, iLayerMask)) {
			return	aHit.transform.GetComponent<T>();
		}
		
		return	null;
	}


	[System.Obsolete ("old method. should be use \"TouchComponent<T>()\".")]
	public	Transform	TouchTransform()
	{
		Transform	aTouchTransform;
		
		aTouchTransform	= null;
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			aTouchTransform	= TouchTransform(aIndex);
			if (aTouchTransform	!= null) {
				break;
			}
		}
		
		return	aTouchTransform;
	}
	
	[System.Obsolete ("old method. should be use \"TouchComponent<T>()\".")]
	public	Transform	TouchTransform(Camera iCamera)
	{
		for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
			if (!mTouchSensors[aIndex].isActive) {
				continue;
			}
			return	TouchTransform(iCamera, aIndex, ~0);
		}
		
		return	null;
	}
	
	[System.Obsolete ("old method. should be use \"TouchComponent<T>()\".")]
	public	Transform	TouchTransform(int iFingerID)
	{
		return	TouchTransform(iFingerID, ~0);
	}
	
	[System.Obsolete ("old method. should be use \"TouchComponent<T>()\".")]
	public	Transform	TouchTransform(Camera iCamera, int iFingerID)
	{
		return	TouchTransform(iCamera, iFingerID, ~0);
	}
	
	[System.Obsolete ("old method. should be use \"TouchComponent<T>()\".")]
	public	Transform	TouchTransform(int iFingerID, int iLayerMask)
	{
		if (!mTouchSensors[iFingerID].isActive || !IsTouched(iFingerID)) {
			return	null;
		}
		
		Transform	aTouchTransform;
		
		List<Camera>	aAllCamera	= new List<Camera>();
		foreach (Camera aCamera in Camera.allCameras) {
			aAllCamera.Add (aCamera);
		}
		aAllCamera.Sort ((Camera a, Camera b) => (int)b.depth - (int)a.depth);
		
		aTouchTransform	= null;
		for (int aIndex = 0; aIndex < aAllCamera.Count; aIndex++) {
			aTouchTransform	= TouchTransform(aAllCamera[aIndex], iFingerID, iLayerMask);
			if (aTouchTransform != null) {
				return	aTouchTransform;
			}
		}
		return	null;
	}
	
	[System.Obsolete ("old method. should be use \"TouchComponent<T>()\".")]
	public	Transform	TouchTransform(Camera iCamera, int iFingerID, int iLayerMask)
	{
		if (!mTouchSensors[iFingerID].isActive || !IsTouched(iFingerID)) {
			return	null;
		}
		
		Ray				aRay;
		RaycastHit		aHit;
		
		aRay	= iCamera.ScreenPointToRay(mTouchSensors[iFingerID].position);
		if (Physics.Raycast(aRay, out aHit, 1000.0f, iLayerMask)) {
			return	aHit.transform;
		}
		
		return	null;
	}

	protected override void Initialize ()
	{
		if (	Application.platform == RuntimePlatform.Android
		    ||	Application.platform == RuntimePlatform.IPhonePlayer) {
			useMouse	= false;
		}

		start ();
	}

	private void	start()
	{
		int		aTouchCountMax	= touchCountMax;

		if (useMouse) {
			aTouchCountMax	+= mouseButtonCount;
		}

		if (mTouchSensors	== null) {
			mTouchSensors	= new TouchAndMouse[aTouchCountMax];
			for (int aIndex = 0; aIndex < mTouchSensors.Length; aIndex++) {
				if (aIndex < touchCountMax) {
					mTouchSensors[aIndex]		= new TouchSensor();
				} else {
					mTouchSensors[aIndex]		= new MouseTouchSensor();
				}
			}
		}
	}

	private	void	Update()
	{
		foreach (TouchAndMouse aTouch in mTouchSensors) {
			aTouch.InitUpdate();
		}

		for (int aIndex = 0; aIndex < Input.touchCount; aIndex++) {
			if (Input.touches[aIndex].fingerId >= touchCountMax) {
				continue;
			}
			mTouchSensors[Input.touches[aIndex].fingerId].SetTouch(Input.touches[aIndex].fingerId, mIsActive);
		}

		if (useMouse) {
			for (int aIndex = 0; aIndex < mouseButtonCount; aIndex++) {
				if (Input.GetMouseButton(aIndex) || Input.GetMouseButtonUp(aIndex)) {
					mTouchSensors[touchCountMax+aIndex].SetTouch(aIndex, mIsActive);
				}
			}

		}
	}
}

public	class TouchSensor : TouchAndMouse
{
	public	override void		InitUpdate()
	{
		isActive	= false;
	}

	public	override void		SetTouch(int iFingerID, bool iIsActive)
	{
		foreach(Touch aTouch in Input.touches) {
			if (aTouch.fingerId		== iFingerID) {
				pTouch		= aTouch;
				break;
			}
		}

		if (pTouch.phase == TouchPhase.Began) {
			touchBeganTime		= Time.realtimeSinceStartup;
			touchEndTime		= touchBeganTime;
			touchDeltaTime		= 0.0f;
			beganPosition		= pTouch.position;
			deltaPosition		= Vector2.zero;
			movement			= Vector2.zero;
		} else if (pTouch.phase == TouchPhase.Ended) {
			touchEndTime		= Time.realtimeSinceStartup;
			touchDeltaTime		= touchEndTime - touchBeganTime;
			movement			+= new Vector2(deltaPosition.x * Mathf.Sign(deltaPosition.x), 
					                          (deltaPosition.y * Mathf.Sign(deltaPosition.y)));
			moveDirection		= pTouch.position - beganPosition;
		} else {
			deltaPosition		= pTouch.deltaPosition;
			movement			+= new Vector2(deltaPosition.x * Mathf.Sign(deltaPosition.x), 
					                          (deltaPosition.y * Mathf.Sign(deltaPosition.y)));
			moveDirection		= pTouch.position - beganPosition;
		}

		phase		= pTouch.phase;
		position	= pTouch.position;
		isActive	= iIsActive;
	}
}

public	class MouseTouchSensor : TouchAndMouse
{
	private Vector2		mLatePosition;

	public override void		InitUpdate()
	{
		isActive	= false;
	}

	public override void		SetTouch(int iFingerID, bool iIsActive)
	{
		Vector2		aMousePosition	= new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		
		if (Input.GetMouseButtonDown(iFingerID)) {
			touchBeganTime		= Time.realtimeSinceStartup;
			touchEndTime		= touchBeganTime;
			touchDeltaTime		= 0.0f;
			beganPosition		= aMousePosition;
			mLatePosition		= aMousePosition;
			position			= aMousePosition;
			deltaPosition		= mLatePosition;
			movement			= Vector2.zero;

			phase				= TouchPhase.Began;
		} else if (Input.GetMouseButtonUp(iFingerID)) {
			touchEndTime		= Time.realtimeSinceStartup;
			touchDeltaTime		= touchEndTime - touchBeganTime;
			moveDirection		= aMousePosition - beganPosition;
			movement			+= new Vector2(deltaPosition.x * Mathf.Sign(deltaPosition.x), 
					                          (deltaPosition.y * Mathf.Sign(deltaPosition.y)));

			phase				= TouchPhase.Ended;
		} else if (Input.GetMouseButton(iFingerID)) {
			position			= aMousePosition;
			deltaPosition		= aMousePosition - mLatePosition;
			mLatePosition		= aMousePosition;
			moveDirection		= aMousePosition - beganPosition;
			movement			+= new Vector2(deltaPosition.x * Mathf.Sign(deltaPosition.x),
					                          (deltaPosition.y * Mathf.Sign(deltaPosition.y)));

			phase				= TouchPhase.Moved;
		}
		
		isActive	= iIsActive;
	}
}

public abstract	class TouchAndMouse
{
	protected	Touch			pTouch;

	public	float			touchBeganTime {
		get;
		protected	set;
	}
	public	float			touchEndTime {
		get;
		protected	set;
	}
	public	float			touchDeltaTime {
		get;
		protected	set;
	}
	public	TouchPhase		phase {
		get;
		protected	set;
	}
	public	Vector2			beganPosition {
		get;
		protected	set;
	}
	public	Vector2			position {
		get;
		protected	set;
	}
	public	Vector2			deltaPosition {
		get;
		protected	set;
	}
	public	Vector2			movement {
		get;
		protected	set;
	}
	public	Vector2			moveDirection {
		get;
		protected	set;
	}
	public	bool			isActive {
		get;
		protected	set;
	}

	public virtual void		InitUpdate(){}

	public virtual void		SetTouch(int iFingerID, bool iIsActive){}
}
