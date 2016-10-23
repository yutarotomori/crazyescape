using UnityEngine;
using System.Collections;

public static class Easing {
	public static float InQuad(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		return max*t*t + min;
	}

	public static float OutQuad(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		return -max*t*(t-2)+min;
	}

	public static float InOutQuad(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		if( t / 2 < 1 ) {
			return max/2 * t * t + min;
		}
		--t;
		return -max * (t * (t-2)-1) + min;
	}

	public static float InCubic(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		return max * t*t*t + min;
	}

	public static float OutCubic(float t,float totaltime,float max ,float min ) {
		max -= min;
		t = t/totaltime-1;
		return max * (t*t*t+1) + min;
	}

	public static float InOutCubic(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		if( t/2 < 1 ) {
			return max/2*t*t*t + min;
		}
		t -= 2;
		return max/2 * (t*t*t+2) + min;
	}

	public static float InQuart(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		return max * t*t*t*t + min;
	}

	public static float OutQuart(float t,float totaltime,float max ,float min ) {
		max -= min;
		t = t/totaltime-1;
		return -max*( t*t*t*t-1) +min;
	}

	public static float InOutQuart(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		if( t/2 < 1 ) {
			return max/2 * t*t*t*t +min;
		}
		t -= 2;
		return -max/2 * (t*t*t*t-2) + min;
	}

	public static float InQuint(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		return max*t*t*t*t*t + min;
	}

	public static float OutQuint(float t,float totaltime,float max ,float min ) {
		max -= min;
		t = t/totaltime-1;
		return max*(t*t*t*t*t+1)+min;
	}

	public static float InOutQuint(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		if( t/2 < 1 ) { 
			return max/2*t*t*t*t*t + min;
		}
		t -= 2;
		return max/2 * (t*t*t*t*t+2) + min;
	}

	public static float InSine(float t,float totaltime,float max ,float min ) {
		max -= min;
		return -max*Mathf.Cos( t*Mathf.Deg2Rad / totaltime ) + max + min;
	}

	public static float OutSine(float t,float totaltime,float max ,float min ) {
		max -= min;
		return max * Mathf.Sin( t*Mathf.Deg2Rad/totaltime ) + min;
	}

	public static float InOutSine(float t,float totaltime,float max ,float min ){
		max -= min;
		return -max/2* (Mathf.Cos( t*Mathf.PI/totaltime)-1) + min;
	}

	public static float InExp(float t,float totaltime,float max ,float min ) {
		max -= min;
		return t == 0.0f ? min : max*Mathf.Pow(2,10*(t/totaltime-1)) + min;
	}

	public static float OutExp(float t,float totaltime,float max ,float min ) {
		max -= min;
		return t == totaltime ? max + min : max*(-Mathf.Pow(2,-10*t/totaltime)+1)+min;
	}

	public static float InOutExp(float t,float totaltime,float max ,float min ) {
		if( t == 0.0f )
			return min;
		if( t == totaltime ) {
			return max;
		}
		max -= min;
		t /= totaltime;
		
		if( t/2 < 1 ) {
			return max/2*Mathf.Pow(2,10*(t-1)) + min;
		}
		--t;
		return max/2*(-Mathf.Pow(2,-10*t)+2) + min;
		
	}

	public static float InCirc(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		return -max*(Mathf.Sqrt(1-t*t)-1)+min;
	}

	public static float OutCirc(float t,float totaltime,float max ,float min ) {
		max -= min;
		t = t/totaltime-1;
		return max*Mathf.Sqrt( 1-t*t)+min;
	}

	public static float InOutCirc(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		if( t/2 < 1 ) {
			return -max/2 * (Mathf.Sqrt(1-t*t)-1) + min;
		}
		t -= 2;
		return max/2 * (Mathf.Sqrt(1-t*t)+1) + min;
	}

	public static float InBack(float t,float totaltime,float max ,float min ,float s ) {
		max -= min;
		t /= totaltime;
		return max*t*t*( (s + 1)*t - s) + min;
	}

	public static float OutBack(float t,float totaltime,float max ,float min ,float s ) {
		max -= min;
		t = t/totaltime-1;
		return max*(t*t*((s+1)*t*s)+1)+min;
	}

	public static float InOutBack(float t,float totaltime,float max ,float min ,float s ) {
		max -= min;
		s *= 1.525f;
		if( t/2 < 1 ) {
			return max*(t*t*((s+1)*t-s))+min;
		}
		t -= 2;
		return max/2 * (t*t*((s+1)*t+s)+2) + min;
	}

	public static float OutBounce(float t,float totaltime,float max ,float min ) {
		max -= min;
		t /= totaltime;
		
		if ( t < 1/2.75f ) {
			return max*(7.5625f*t*t)+min;
		} else if (t < 2/2.75f ) {
			t-= 1.5f/2.75f;
			return max*(7.5625f*t*t+0.75f)+min;
		} else if ( t< 2.5f/2.75f ) {
			t -= 2.25f/2.75f;
			return max*(7.5625f*t*t+0.9375f)+min;
		} else {
			t-= 2.625f/2.75f;
			return max*(7.5625f*t*t+0.984375f) + min;
		}
	}

	public static float InBounce(float t,float totaltime,float max ,float min ) {
		return max - OutBounce( totaltime - t , totaltime , max - min , 0 ) + min;  
	}

	public static float InOutBounce(float t,float totaltime,float max ,float min ) {
		if( t < totaltime/2 ) {
			return InBounce( t*2 , totaltime , max-min , max )*0.5f + min;
		} else {
			return OutBounce(t*2-totaltime,totaltime,max-min,0)*0.5f+min + (max-min)*0.5f;
		}
	}

	public static float Linear(float t,float totaltime,float max ,float min ) {
		return (max-min)*t/totaltime + min;
	}
}
