
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;

namespace MetaphysicsIndustries.Giza
{
    public struct Token : IInputElement
    {
        public Token(Definition definition=null,
                     InputPosition startPosition=new InputPosition(),
                     string value="",
                     int indexOfNextTokenization=-1)
        {
            if (indexOfNextTokenization < 0)
            {
                indexOfNextTokenization = startPosition.Index + (value ?? "").Length;
            }

            Definition = definition;
            StartPosition = startPosition;
            Value = value;
            IndexOfNextTokenization = indexOfNextTokenization;
        }

        public Definition Definition;
        public InputPosition StartPosition;
        public string Value;
        public int IndexOfNextTokenization;

        #region IInputElement implementation
        string IInputElement.Value { get { return Value; } }
        InputPosition IInputElement.Position { get { return StartPosition; } }
        int IInputElement.IndexOfNextElement { get { return IndexOfNextTokenization; } }
        #endregion
    }
}

