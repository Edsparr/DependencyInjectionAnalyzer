using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionAnalyzer.CodeGenerators
{
    public interface ICodeGenerator
    {
        CompilationUnitSyntax Generate(CompilationUnitSyntax root, SyntaxNode node);
    }

    public abstract class CodeGeneratorBase<TNode> : ICodeGenerator where TNode : SyntaxNode
    {
        protected readonly SyntaxGenerator syntaxGenerator;

        protected CodeGeneratorBase(SyntaxGenerator syntaxGenerator)
        {
            this.syntaxGenerator = syntaxGenerator ?? throw new ArgumentNullException(nameof(syntaxGenerator));
        }



        public CompilationUnitSyntax Generate(CompilationUnitSyntax root, SyntaxNode node)
        {
            if (!(node is TNode))
                throw new ArgumentException(nameof(node));
            return Generate(root, (TNode)node);
        }

        public abstract CompilationUnitSyntax Generate(CompilationUnitSyntax root, TNode node);
    }
}
