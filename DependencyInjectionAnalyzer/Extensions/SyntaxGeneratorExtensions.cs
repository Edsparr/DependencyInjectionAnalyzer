using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionAnalyzer.Extensions
{
    public static partial class SyntaxGeneratorExtensions
    {
        public static StatementSyntax MemberAssignStatement(this SyntaxGenerator source, string name)
        {
            return source.ExpressionStatement(
                source.AssignmentStatement(
                    source.MemberAccessExpression(
                        source.ThisExpression(),
                        source.IdentifierName(name)),
                    source.IdentifierName(name))) as StatementSyntax;
        }

        public static FieldDeclarationSyntax FieldDeclaration(this SyntaxGenerator source, VariableDeclaratorSyntax variable)
        {
            var field = variable.Parent.Parent as FieldDeclarationSyntax; //First one is VariableDeclerationSyntax
            var result = source.FieldDeclaration(variable.Identifier.ValueText, field.Declaration.Type) as FieldDeclarationSyntax;
            result = result.WithModifiers(field.Modifiers);
            return result;
        }
    }
}
