﻿using System;
using UnityEngine;

public class utils
{
  static public bool layermask_contains(LayerMask lm, int l) {
		return ((0x1 << l) & lm) != 0;
  }
}
