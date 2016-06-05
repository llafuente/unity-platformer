/**!
The MIT License (MIT)

Copyright (c) 2015 Sebastian
Original file: https://github.com/SebLague/2DPlatformer-Tutorial/blob/master/Episode%2011/RaycastController.cs

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
  [RequireComponent (typeof (BoxCollider2D))]
  public class RaycastController : MonoBehaviour {

    public LayerMask collisionMask;

    /// <summary>
    /// How far from then env the Character must be.
    /// NOTE: must be less than skinWidth, to allow continuous ground contact
    /// </summary>
    public float minDistanceToEnv = 0.08f;
    /// <summary>
    /// Defines how far in from the edges of the collider rays are we going to cast from.
    /// NOTE: This value must be geater than minDistanceToEnv
    /// </summary>
    public float skinWidth = 0.10f;
    /// <summary>
    /// How many rays to check horizontal collisions
    /// </summary>
    public int horizontalRayCount = 4;
    /// <summary>
    /// How many rays to check vertical collisions
    /// </summary>
    public int verticalRayCount = 4;

    [HideInInspector]
    public float horizontalRaySpacing;

    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D box;

    public RaycastOrigins raycastOrigins;

    public virtual void Awake() {
      box = GetComponent<BoxCollider2D> ();
    }

    public virtual void Start() {
      CalculateRaySpacing ();
    }

    public void UpdateRaycastOrigins() {
      Bounds bounds = box.bounds;
      // * 2 so it's shrink skinWidth by each side
      bounds.Expand (skinWidth * -2);

      // cache
      Vector3 min = bounds.min;
      Vector3 max = bounds.max;

      raycastOrigins.bottomLeft = new Vector2 (min.x, min.y);
      raycastOrigins.bottomCenter = new Vector2 (min.x + bounds.size.x * 0.5f, min.y);
      raycastOrigins.bottomRight = new Vector2 (max.x, min.y);
      raycastOrigins.topLeft = new Vector2 (min.x, max.y);
      raycastOrigins.topRight = new Vector2 (max.x, max.y);
    }

    public RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float rayLength, int mask, Color? color = null) {
      Debug.DrawRay(origin, direction * rayLength, color ?? Color.red);

      return Physics2D.Raycast(origin, direction, rayLength, mask);
    }

    /// <summary>
    /// Recalculate distance between rays (horizontalRaySpacing & verticalRaySpacing)
    /// </summary>
    public void CalculateRaySpacing() {
      Bounds bounds = box.bounds;
      bounds.Expand (skinWidth * -2);

      horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
      verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

      horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
      verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins {
      public Vector2 topLeft;
      public Vector2 topRight;
      public Vector2 bottomLeft;
      public Vector2 bottomCenter;
      public Vector2 bottomRight;
    }

    public RaycastHit2D DoVerticalRay(float directionY, int i, float rayLength, ref Vector3 velocity, Color? c = null) {
        Vector2 rayOrigin = (directionY == -1) ?
          raycastOrigins.bottomLeft :
          raycastOrigins.topLeft;

        rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
        RaycastHit2D hit = Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask, c ?? Color.red);

        return hit;
    }

    public RaycastHit2D DoFeetRay(float rayLength, LayerMask mask) {
      RaycastHit2D hit = Raycast(raycastOrigins.bottomCenter, Vector2.down, rayLength, mask, Color.blue);

      return hit;
    }
  }
}
