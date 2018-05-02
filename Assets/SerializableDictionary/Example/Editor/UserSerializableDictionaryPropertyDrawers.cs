using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringBooleanDictionary))]
[CustomPropertyDrawer(typeof(StringIntDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
