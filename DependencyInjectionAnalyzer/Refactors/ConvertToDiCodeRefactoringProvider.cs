using DependencyInjectionAnalyzer.Actions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DependencyInjectionAnalyzer.Refactors
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ConvertToDIConstructorCodeRefactoringProvider)), Shared]
    internal class ConvertToDIConstructorCodeRefactoringProvider : CodeRefactoringProvider
    {
        private const string Title = "Generate Dependency Injection constructor";

        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var document = context.Document;
            var cancellationToken = context.CancellationToken;
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) as CompilationUnitSyntax;
            var token = root.FindToken(context.Span.Start);

            var constructorDeclaration = token.Parent?.FirstAncestorOrSelf<ConstructorDeclarationSyntax>();

            if (constructorDeclaration == null)
                return;

            context.RegisterRefactoring(new DefaultCodeAction(Title,
                async (c) => await GenerateDependencyInjectionConstructor(document, root, constructorDeclaration, cancellationToken)));
        }

        public async Task<Document> GenerateDependencyInjectionConstructor(Document document,
            CompilationUnitSyntax root, ConstructorDeclarationSyntax constructor,
            CancellationToken cancellationToken)
        {
            var codeGenerator = new DependencyInjectionConstructorCodeGenerator(SyntaxGenerator.GetGenerator(document));

            document = document.WithSyntaxRoot(codeGenerator.Generate(root, constructor));

            document = await Formatter.FormatAsync(document, Formatter.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);

            return document;
        }
    }

}
