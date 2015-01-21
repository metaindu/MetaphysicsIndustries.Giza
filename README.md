MetaphysicsIndustries.Giza
==========================

A parser

Intro
-----
Giza is a parser, but not in the same vein as what you're used to. For one, it's not a [parser-generator](https://en.wikipedia.org/wiki/Compiler-compiler). Rather than turning a grammar into a bunch of source code, it instead converts the grammar into a ready-to-use form, and can then start parsing immediately. No compiling necessary!

One of the cool benefits of this is that you can try out and edit your grammar in real time. Take a look at the included `giza` program. It's actually a [REPL](https://en.wikipedia.org/wiki/Read%E2%80%93eval%E2%80%93print_loop) for editing grammars and parsing text. _How cool is that!?_

Giza would probably be classified as a kind of [GLR parser](https://en.wikipedia.org/wiki/GLR_parser). It goes through all possible paths without the need for backtracking. It can also handle ambiguities, by producing multiple parse trees, if needed. The format of its grammars is not quite [BNF](https://en.wikipedia.org/wiki/Backus%E2%80%93Naur_Form); instead the syntax is more inspired by programming languages and regexes. The syntax of grammars is described entirely by the "Supergrammar", which is written in it's own syntax. Take a look at [`Supergrammar.txt`](https://github.com/metaindu/MetaphysicsIndustries.Giza/blob/master/Supergrammar.txt).

Giza maintains a distinction between "tokenized" and "non-tokenized" processing. The former is what parsers usually do, constructing a parse tree from a stream of input tokens. The latter is more akin to what regexes do, matching against a stream of input *characters* instead. Herein, non-tokenized processing often goes by the name "spanning" and is done by a "spanner".

Example
-------
Here's how you would work with a grammar in the REPL:
```
>>> expr = ( mult-expr | add-expr | sub-expr );
>>> mult-expr = sub-expr ( [*/%] sub-expr )+;
>>> add-expr = ( mult-expr | sub-expr ) ( [-+] ( mult-expr | sub-expr ) )+;
>>> sub-expr = ( number | var | paren );
>>> <token> number = [\d]+;
>>> <token> var = [\l]+;
```
Then you can check the definitions for errors like this:
```
>>> check --tokenized
There are errors in the grammar:
  Definition 'sub-expr' references a definition 'paren' which is not defined.
```
Oops, we forgot to define `paren`:
```
>>> paren = '(' expr ')';
>>> check --tokenized
There are no errors or warnings.
```
Once that's settled, we can parse some text:
```
>>> parse expr '1 + 2 * 3 - four / 5 % 6 + seven'
There is 1 valid parse of the input.
```
To get more info, use the `--verbose` flag to print out the parse tree:
```
>>> parse expr --verbose '1 + 2 * 3 - four / 5 % 6 + seven'
There is 1 valid parse of the input.
          expr
            add-expr
              sub-expr
1               number
+             $implicit char class +-
              mult-expr
                sub-expr
2                 number
*               $implicit char class %*/
                sub-expr
3                 number
-             $implicit char class +-
              mult-expr
                sub-expr
four              var
/               $implicit char class %*/
                sub-expr
5                 number
%               $implicit char class %*/
                sub-expr
6                 number
+             $implicit char class +-
              sub-expr
seven           var
```
In the left column, you see the sequence of tokens in the input. On the right is the parse tree with indentation to show hierarchy, with each matching node on the same line of text as the token that matched it.

But that's not all. There's tons of stuff the REPL can do. It has a built-in help system to explain everything:
```
>>> help
Usage:
    >>> [options]
    >>> help [command_or_topic]
    >>> command [args...]

Commands:

    help       Display general help, or help on a specific topic.
    list       List all of the definitions currently defined.
    print      Print definitions as text in giza grammar format
    delete     Delete the specified definitions.
    save       Save definitions to a file as text in giza grammar format
    load       Load definitions from a file
    check      Check definitions for errors
    parse      Parse one or more inputs with a tokenized grammar, starting with a given definition, and print how many valid parse trees are found
    span       Span one or more inputs with a non-tokenized grammar, starting with a given definition, and print how many valid span trees are found
    render     Convert definitions to state machine format and render the state machines to a C# class.
```

Example in Code
---------------
Once you've worked out your language's grammar, you typically want to use it in some other application. To do that, follow these steps:

1. Save all relevant definitions to a file. By convention, the file extension is `.giza`, but you can use anything. See an example [here](https://github.com/metaindu/MetaphysicsIndustries.Solus/blob/master/SolusGrammar.giza).
2. Use the `render` command. This takes your grammar, converts all of the definitions into state machine format, and then emits the C# code for a class that creates the same state machine representation<sup>1</sup>. See example [here](https://github.com/metaindu/MetaphysicsIndustries.Solus/blob/master/SolusGrammar.cs).
3. Create a class that takes your `*Grammar` class and plugs it into a `Parser` object. Whenever your want to parse some input text, pass it to the `Parser.Parse` method to get a list of parse trees. See example [here](https://github.com/metaindu/MetaphysicsIndustries.Solus/blob/master/SolusParser.cs##L9-L35).
4. [Optional] If there's more than one parse tree, then there's some ambiguity in your grammar with that particular input. You can do some kind of semantic analysis to choose which of the parse trees is the 'right' one. [Or you can just pick the first one, if you're lazy.](https://github.com/metaindu/MetaphysicsIndustries.Solus/blob/master/SolusParser.cs#L42-L52)
5. Convert the generalized parse tree(s) into whatever domain objects you need. See example [here](https://github.com/metaindu/MetaphysicsIndustries.Solus/blob/master/SolusParser.cs#L75-L429).

<sup>1</sup> This kinda violates the "not turning a grammar into a bunch of source code" assertion above, but not entirely. The code so generated is not capable of parsing anything. It's basically a data structure serialized as C#. We could just as well store the raw text of the grammar file and run it through `SupergrammarSpanner` to generate the state machine representation, which is what the `render` command does for you. Whatever. We're working on making it prettier.

State of the project
--------------------
Unfortunately, you caught us right in the middle of a major architecture overhaul. Much of the internals of how the system work are being completely re-worked. In particular, we're trying to treat tokenized and non-tokenized processing as special cases of a generalized pattern-matching system. Hence, the hideous [`Spanner2`](https://github.com/metaindu/MetaphysicsIndustries.Giza/blob/master/Spanner2.cs) class. It's a work in progress. Code and API may change at any time, although the command-line tool will be pretty stable. There's a lot of cosmetic stuff to be fixed up as well, such as the fact that `Supergrammar.txt` should really be named `Supergrammar.giza`. Stay tuned!
