// Copyright rubicon IT GmbH
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

//
// OptionValueCollection.cs
//
// Authors:
//  Jonathan Pryor <jpryor@novell.com>
//
// Copyright (C) 2008 Novell (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace MixinXRef.Utility.Options
{
  public class OptionValueCollection : IList, IList<string>
  {
    private readonly List<string> _values = new List<string> ();
    private readonly OptionContext _context;

    internal OptionValueCollection (OptionContext context)
    {
      _context = context;
    }

    #region ICollection
    void ICollection.CopyTo (Array array, int index) { (_values as ICollection).CopyTo (array, index); }
    bool ICollection.IsSynchronized { get { return (_values as ICollection).IsSynchronized; } }
    object ICollection.SyncRoot { get { return (_values as ICollection).SyncRoot; } }
    #endregion

    #region ICollection<T>
    public void Add (string item) { _values.Add (item); }
    public void Clear () { _values.Clear (); }
    public bool Contains (string item) { return _values.Contains (item); }
    public void CopyTo (string[] array, int arrayIndex) { _values.CopyTo (array, arrayIndex); }
    public bool Remove (string item) { return _values.Remove (item); }
    public int Count { get { return _values.Count; } }
    public bool IsReadOnly { get { return false; } }
    #endregion

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator () { return _values.GetEnumerator (); }
    #endregion

    #region IEnumerable<T>
    public IEnumerator<string> GetEnumerator () { return _values.GetEnumerator (); }
    #endregion

    #region IList
    int IList.Add (object value) { return (_values as IList).Add (value); }
    bool IList.Contains (object value) { return (_values as IList).Contains (value); }
    int IList.IndexOf (object value) { return (_values as IList).IndexOf (value); }
    void IList.Insert (int index, object value) { (_values as IList).Insert (index, value); }
    void IList.Remove (object value) { (_values as IList).Remove (value); }
    void IList.RemoveAt (int index) { (_values as IList).RemoveAt (index); }
    bool IList.IsFixedSize { get { return false; } }
    object IList.this[int index] { get { return this[index]; } set { (_values as IList)[index] = value; } }
    #endregion

    #region IList<T>
    public int IndexOf (string item) { return _values.IndexOf (item); }
    public void Insert (int index, string item) { _values.Insert (index, item); }
    public void RemoveAt (int index) { _values.RemoveAt (index); }

    private void AssertValid (int index)
    {
      if (_context.Option == null)
        throw new InvalidOperationException ("OptionContext.Option is null.");
      if (index >= _context.Option.MaxValueCount)
        throw new ArgumentOutOfRangeException ("index");
      if (_context.Option.OptionValueType == OptionValueType.Required &&
          index >= _values.Count)
        throw new OptionException (string.Format (
            _context.OptionSet.MessageLocalizer ("Missing required value for option '{0}'."), _context.OptionName),
                                   _context.OptionName);
    }

    public string this[int index]
    {
      get
      {
        AssertValid (index);
        return index >= _values.Count ? null : _values[index];
      }
      set
      {
        _values[index] = value;
      }
    }
    #endregion

    public override string ToString ()
    {
      return string.Join (", ", _values.ToArray ());
    }
  }
}