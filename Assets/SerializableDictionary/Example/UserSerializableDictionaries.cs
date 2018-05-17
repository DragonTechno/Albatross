﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> {}

[Serializable]
public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> {}

[Serializable]
public class StringBooleanDictionary: SerializableDictionary<string, bool> { }

[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> { }