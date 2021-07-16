#!/usr/bin/env python

# MetaphysicsIndustries.Giza - A Parsing System
# Copyright (C) 2008-2021 Metaphysics Industries, Inc.
#
# This library is free software; you can redistribute it and/or
# modify it under the terms of the GNU Lesser General Public
# License as published by the Free Software Foundation; either
# version 3 of the License, or (at your option) any later version.
#
# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Lesser General Public License for more details.
#
# You should have received a copy of the GNU Lesser General Public
# License along with this library; if not, write to the Free Software
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
# USA

import os
import sys

git_desc = os.popen('git describe --always --tags --abbrev=40 --long').read().rstrip()

print('Writing git description: {}\n'.format(git_desc))

filename = "AssemblyInfo.Git.cs"
if len(sys.argv) > 1 and sys.argv[1]:
    filename = sys.argv[1]

with open(filename, 'w') as f:
    f.write('using System.Reflection;\n')
    f.write('\n')
    f.write('[assembly: AssemblyInformationalVersion("')
    f.write(git_desc)
    f.write('")]\n')
