#!/bin/bash

rm -rf Supergrammar-parsed-new.json
mono giza/bin/Debug/giza.exe --super Supergrammar.txt > Supergrammar-parsed-new.json
diff Supergrammar-parsed-ref.json Supergrammar-parsed-new.json
