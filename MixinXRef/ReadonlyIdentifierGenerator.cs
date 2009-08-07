// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ReadonlyIdentifierGenerator<T> : IIdentifierGenerator<T>
  {
    private readonly Dictionary<T, string> _readonlyIdentifiers;
    private readonly string _defaultValue;


    public ReadonlyIdentifierGenerator (Dictionary<T, string> idintifierDictionary, string defaultValue)
    {
      ArgumentUtility.CheckNotNull ("idintifierDictionary", idintifierDictionary);
      ArgumentUtility.CheckNotNull ("defaultValue", defaultValue);

      _readonlyIdentifiers = idintifierDictionary;
      _defaultValue = defaultValue;
    }

    public string GetIdentifier (T item)
    {
      return _readonlyIdentifiers.ContainsKey (item) ? _readonlyIdentifiers[item] : _defaultValue;
    }
  }
}