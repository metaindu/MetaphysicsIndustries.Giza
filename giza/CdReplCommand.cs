
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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
using System.Collections.Generic;
using System.IO;
using MetaphysicsIndustries.Giza;
using NCommander;

namespace giza
{
    public class CdReplCommand : ReplCommand
    {
        public CdReplCommand(Dictionary<string, Definition> env)
            : base(env)
        {
            Name = "cd";
            Description = "Change the current working directory.";
            HelpText = "This command changes the process's working " +
                       "directory. The working directory affects things like " +
                       "loading or rendering files. The format of the path " +
                       "of the directory to change to depends on the " +
                       "underlying operating system.";
            Params = new[]
            {
                new Parameter
                {
                    Name = "path",
                    ParameterType = ParameterType.String,
                    Description = "A path to the directory to change to. Can " +
                                  "be absolute or relative. If '-', the " +
                                  "working directory will be changed to the " +
                                  "previous one.",
                },
            };
        }

        private string _oldPwd;

        protected override void InternalExecute(Dictionary<string, object> args)
        {
            var path = (string) args["path"];
            path = path.Trim();
            string toPrint = null;
            if (path == "-")
            {
                path = _oldPwd;
                toPrint = path;
            }

            var oldPwd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(path);
            _oldPwd = oldPwd;
            if (toPrint != null)
                Console.WriteLine(toPrint);
        }
    }
}