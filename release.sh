#!/bin/bash

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

TAG="$1"

if [ "$TAG" = "" ]
then
    TAG=$TRAVIS_TAG
fi

if [ "$TAG" = "" ]
then
    echo 'No tag given. Package will not be created.'
    exit 1
fi

VERSION=`echo "$TAG" | perl -ne 'print /^v(\d+\.\d+(?:\.\d+(?:\.\d+)?)?)$/'`
if [ "$VERSION" = "" ];
then
    echo 'Wrong version format. Package will not be created.'
    exit 1
fi

AVERSION=`grep AssemblyVersion AssemblyInfo.cs | perl -npe 's/^.*?\"//;s/\".*$//'`

if [ "$VERSION" != "$AVERSION" ]
then
    echo "Tag doesn't match assembly version. Package will not be created."
    exit 1
fi

echo 'Creating the nuget package...'
if ! nuget pack MetaphysicsIndustries.Giza.nuspec -Properties version=$VERSION ; then
    echo 'Error creating the package. The package will not be uploaded.'
    exit 1
fi

echo 'Uploading the package to nuget...'
if ! nuget push MetaphysicsIndustries.Giza.$VERSION.nupkg -Source nuget.org -ApiKey $NUGET_API_KEY ; then
    echo 'Error uploading the package. Quitting.'
    exit 1
fi

echo 'Done.'
