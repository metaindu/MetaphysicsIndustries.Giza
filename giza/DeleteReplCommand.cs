
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
using MetaphysicsIndustries.Giza;

namespace giza
{
    public class DeleteReplCommand : ReplCommand
    {
        public DeleteReplCommand(Dictionary<string, Definition> env)
            : base(env)
        {
            Name = "delete";
            Description = "Delete the specified definitions.";
            Params = new [] {
                new Parameter {
                    Name="def-names",
                    ParameterType=ParameterType.StringArray,
                    Description="The names of the definitions to delete. If '*' is given, then all definitions are deleted.",
                },
            };
        }

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var defsToDelete = (string[])args["def-names"];

            if (defsToDelete == null ||
                defsToDelete.Length < 1)
            {
                Console.WriteLine("Error: no definitions were specified.");
                return;
            }

            if (defsToDelete.Length == 1 &&
                defsToDelete[0] == "*")
            {
                Env.Clear();
                Console.WriteLine("All definitions were deleted.");
                return;
            }

            var someAreMissing = false;
            foreach (var name in defsToDelete)
            {
                if (!Env.ContainsKey(name))
                {
                    Console.WriteLine("Error: There is no definition named \"{0}\".", name);
                    someAreMissing = true;
                }
            }

            if (someAreMissing)
            {
                Console.WriteLine("No definitions were deleted.");
            }
            else
            {
                foreach (var name in defsToDelete)
                {
                    Env.Remove(name);
                }
            }
        }
    }
}

