using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaphysicsIndustries.Giza
{
    public class ImportTransform : IGrammarTransform
    {
        public ImportTransform(IFileSource fileSource=null)
        {
            if (fileSource == null)
                fileSource = new FileSource();
            _fileSource = fileSource;
        }

        private readonly IFileSource _fileSource;

        private readonly ImportCache _importCache = new ImportCache();

        Grammar IGrammarTransform.Transform(Grammar g)
        {
            return Transform(g);
        }
        public Grammar Transform(Grammar g, List<Error> errors=null,
            IFileSource fileSource=null, ImportCache importCache=null)
        {
            if (errors == null) errors = new List<Error>();
            if (fileSource == null) fileSource = _fileSource;
            if (importCache == null) importCache = _importCache;

            return ProcessImports(g, errors, fileSource, importCache);
        }
        public static Grammar ProcessImports(Grammar g,
            List<Error> errors, IFileSource fileSource,
            ImportCache importCache=null)
        {
            var defsByName =
                g.Definitions.ToDictionary(d => d.Name);
            var errors2 = new List<Error>();
            foreach (var importStmt in g.ImportStatements)
            {
                var importedDefs = ImportDefinitions(importStmt, errors2,
                    fileSource, importCache);
                errors.AddRange(errors2);
                if (errors2.ContainsNonWarnings())
                    // TODO: proper exception type, like ImportException
                    throw new InvalidOperationException(
                        "Errors while importing file: " + errors2);
                // Add the definitions to the collection, overwriting any with
                // the same name that were defined previously.
                foreach (var def in importedDefs)
                {
                    defsByName[def.Name] = def;
                }
            }

            return new Grammar
            {
                Definitions = defsByName.Values.ToList()
            };
        }

        static Definition[] ImportDefinitions(
            ImportStatement importStmt, List<Error> errors,
            IFileSource fileSource, ImportCache importCache)
        {
            var fileToImport = importStmt.Filename;
            var importRefs = importStmt.ImportRefs;

            if (!importCache.ContainsKey(fileToImport))
            {
                var content = fileSource.GetFileContents(fileToImport);
                var errors2 = new List<Error>();
                var ss = new SupergrammarSpanner();
                var ig1 = ss.GetGrammar(content, errors2);
                errors.AddRange(errors2);
                if (errors2.ContainsNonWarnings())
                    return null;

                var ig = ProcessImports(ig1, errors2, fileSource,
                    importCache);
                errors.AddRange(errors2);
                if (errors2.ContainsNonWarnings())
                    return null;

                var importedDefs = ig.Definitions;
                errors.AddRange(errors2);
                if (errors2.Count > 0)
                    return null;

                var importedDefsByName1 =
                    importedDefs.ToDictionary(
                        d => d.Name,
                        d => d);
                importCache[fileToImport] = importedDefsByName1;
            }

            var importedDefsByName = importCache[fileToImport];

            IEnumerable<Definition> defsToImport;
            if (importRefs == null)
            {
                importRefs = importedDefsByName.Values.Select(
                    defToImport => new ImportRef
                    {
                        SourceName = defToImport.Name,
                        DestName = defToImport.Name
                    }).ToArray();
            }

            var defsToImport1 = new List<Definition>();
            defsToImport = defsToImport1;
            var hasErrors = false;
            foreach (var importRef in importRefs)
            {
                if (importedDefsByName.ContainsKey(importRef.SourceName))
                    continue;

                errors.Add(new ImportError
                {
                    ErrorType = ImportError.DefinitionNotFound,
                    DefinitionName = importRef.SourceName
                });

                hasErrors = true;
            }

            if (hasErrors) return null;

            foreach (var importRef in importRefs)
            {
                var sourceName = importRef.SourceName;
                var sourceDef = importedDefsByName[sourceName];
                var destName = importRef.DestName;
                var destDef = new Definition(destName,
                    sourceDef.Directives, sourceDef.Expr);
                destDef.IsImported = true;
                defsToImport1.Add(destDef);
            }

            return defsToImport.ToArray();
        }
    }
}
