using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MetaphysicsIndustries.Giza
{
    public class ImportTransform : IPreGrammarTransform
    {
        public ImportTransform(SupergrammarSpanner ss, IFileSource fileSource)
        {
            _ss = ss;
            _fileSource = fileSource;
        }

        private readonly SupergrammarSpanner _ss;
        private readonly IFileSource _fileSource;

        private readonly ImportCache _importCache = new ImportCache();

        PreGrammar IPreGrammarTransform.Transform(PreGrammar pg)
        {
            return Transform(pg);
        }
        public PreGrammar Transform(PreGrammar pg, List<Error> errors=null,
            SupergrammarSpanner ss=null, IFileSource fileSource=null,
            ImportCache importCache=null)
        {
            if (errors == null) errors = new List<Error>();
            if (ss == null) ss = _ss;
            if (fileSource == null) fileSource = _fileSource;
            if (importCache == null) importCache = _importCache;

            return ProcessImports(pg, errors, ss, fileSource, importCache);
        }
        public static PreGrammar ProcessImports(PreGrammar pg,
            List<Error> errors, SupergrammarSpanner ss, IFileSource fileSource,
            ImportCache importCache=null)
        {
            var defsByName =
                pg.Definitions.ToDictionary(d => d.Name);
            var errors2 = new List<Error>();
            foreach (var importStmt in pg.ImportStatements)
            {
                var importedDefs = ImportDefinitions(importStmt, errors2, ss,
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

            return new PreGrammar
            {
                Definitions = defsByName.Values.ToList()
            };
        }

        static DefinitionExpression[] ImportDefinitions(ImportStatement importStmt,
            List<Error> errors, SupergrammarSpanner ss, IFileSource fileSource,
            ImportCache importCache)
        {
            var fileToImport = importStmt.ModuleOrFile;
            var importRefs = importStmt.ImportRefs;

            if (!importCache.ContainsKey(fileToImport))
            {
                var content = fileSource.GetFileContents(fileToImport);
                var errors2 = new List<Error>();
                var ipg1 = ss.GetPreGrammar(content, errors2);
                errors.AddRange(errors2);
                if (errors2.ContainsNonWarnings())
                    return null;

                var ipg = ProcessImports(ipg1, errors2, ss, fileSource,
                    importCache);
                errors.AddRange(errors2);
                if (errors2.ContainsNonWarnings())
                    return null;

                var importedDefs = ipg.Definitions;
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

            IEnumerable<DefinitionExpression> defsToImport;
            if (importRefs != null)
            {
                var defsToImport1 = new List<DefinitionExpression>();
                defsToImport = defsToImport1;
                foreach (var importRef in importRefs)
                {
                    var sourceName = importRef.SourceName;
                    var destName = importRef.DestName;
                    if (!importedDefsByName.ContainsKey(sourceName))
                        errors.Add(new ImportError
                        {
                            ErrorType = ImportError.DefinitionNotFound,
                            DefinitionName = sourceName
                        });
                    else
                    {
                        // TODO: refactor to de-duplicate
                        var sourceDef = importedDefsByName[sourceName];
                        var destDef = new DefinitionExpression(destName,
                            sourceDef.Directives, sourceDef.Items);
                        destDef.IsImported = true;
                        defsToImport1.Add(destDef);
                    }
                }
            }
            else
            {
                var defsToImport1 = new List<DefinitionExpression>();
                defsToImport = defsToImport1;
                foreach (var defToImport in importedDefsByName.Values)
                {
                    // TODO: refactor to de-duplicate
                    var sourceDef = defToImport;
                    var destName = defToImport.Name;
                    var destDef = new DefinitionExpression(destName,
                        sourceDef.Directives, sourceDef.Items);
                    destDef.IsImported = true;
                    defsToImport1.Add(destDef);
                }
            }

            return defsToImport.ToArray();
        }
    }
}
