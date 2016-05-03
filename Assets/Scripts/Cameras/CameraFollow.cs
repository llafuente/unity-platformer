/**!
The MIT License (MIT)

Copyright (c) 2015 Sebastian
Original file: https://github.com/SebLague/2DPlatformer-Tutorial/blob/master/Episode%2011/Controller2D.cs

Modifications (c) 2016 Luis Lafuente

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
**/

﻿using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
	public class CameraFollow : MonoBehaviour {

		public Character target;
		public PlatformerInput targetInput;

		public float verticalOffset;
		public float lookAheadDstX;
		public float lookSmoothTimeX;
		public float verticalSmoothTime;
		public Vector2 focusAreaSize;

		FocusArea focusArea;

		float currentLookAheadX;
		float targetLookAheadX;
		float lookAheadDirX;
		float smoothLookVelocityX;
		float smoothVelocityY;

		bool lookAheadStopped;
		public bool debug = false;

		void Start() {
			focusArea = new FocusArea (target.controller.box.bounds, focusAreaSize);
		}

		void LateUpdate() {
			focusArea.Update (target.controller.box.bounds);

			Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

			if (focusArea.velocity.x != 0) {
				lookAheadDirX = Mathf.Sign (focusArea.velocity.x);
				float x = targetInput.GetAxisRawX();
				if (Mathf.Sign(x) == Mathf.Sign(focusArea.velocity.x) && x != 0) {
					lookAheadStopped = false;
					targetLookAheadX = lookAheadDirX * lookAheadDstX;
				}
				else {
					if (!lookAheadStopped) {
						lookAheadStopped = true;
						targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX)/4f;
					}
				}
			}


			currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

			focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
			focusPosition += Vector2.right * currentLookAheadX;
			transform.position = (Vector3)focusPosition + Vector3.forward * -10;
		}

		void OnDrawGizmos() {
			if (debug) {
				Gizmos.color = new Color (0, 1, 0, .25f);
				Gizmos.DrawCube (focusArea.centre, focusAreaSize);
			}
		}

		struct FocusArea {
			public Vector2 centre;
			public Vector2 velocity;
			float left,right;
			float top,bottom;


			public FocusArea(Bounds targetBounds, Vector2 size) {
				left = targetBounds.center.x - size.x/2;
				right = targetBounds.center.x + size.x/2;
				bottom = targetBounds.min.y;
				top = targetBounds.min.y + size.y;

				velocity = Vector2.zero;
				centre = new Vector2((left+right)/2,(top +bottom)/2);
			}

			public void Update(Bounds targetBounds) {
				float shiftX = 0;
				//Debug.Log (targetBounds.min.x + "::" + targetBounds.max.x);
				if (targetBounds.min.x < left) {
					shiftX = targetBounds.min.x - left;
				} else if (targetBounds.max.x > right) {
					shiftX = targetBounds.max.x - right;
				}
				left += shiftX;
				right += shiftX;

				float shiftY = 0;
				if (targetBounds.min.y < bottom) {
					shiftY = targetBounds.min.y - bottom;
				} else if (targetBounds.max.y > top) {
					shiftY = targetBounds.max.y - top;
				}
				top += shiftY;
				bottom += shiftY;
				centre = new Vector2((left+right)/2,(top +bottom)/2);
				velocity = new Vector2 (shiftX, shiftY);
			}
		}
	}
}
