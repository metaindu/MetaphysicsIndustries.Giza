
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
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
using NCommander;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Giza;

namespace giza
{
    public class ListReplCommand : ReplCommand
    {
        public ListReplCommand(Dictionary<string, Definition> env)
            : base(env)
        {
            Name = "list";
            Description = "List all of the definitions currently defined.";
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var names = Env.Keys.ToList();
            names.Sort();
            foreach (var name in names)
            {
                Console.WriteLine(name);
            }
        }
    }
}

