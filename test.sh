#!/bin/bash

mono giza/bin/Debug/giza.exe --render Supergrammar.txt Supergrammar | git diff --no-index Supergrammar.cs -

