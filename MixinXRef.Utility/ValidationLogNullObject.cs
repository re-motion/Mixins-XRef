﻿// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 

using System;

namespace MixinXRef.Utility
{
  public class ValidationLogNullObject
  {
    public ValidationLogNullObject ()
    {
    }

    public int NumberOfRulesExecuted
    {
      get { return 0; }
    }

    public int NumberOfUnexpectedExceptions
    {
      get { return 0; }
    }

    public int NumberOfFailures
    {
      get { return 0; }
    }

    public int NumberOfWarnings
    {
      get { return 0; }
    }

    public int NumberOfSuccesses
    {
      get { return 0; }
    }
  }
}