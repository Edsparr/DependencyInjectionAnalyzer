using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DependencyInjectionAnalyzer.Extensions
{
    public static class ClassDeclerationSyntaxExtensions
    {
        public static VariableDeclaratorSyntax FindField(this ClassDeclarationSyntax source, Func<VariableDeclaratorSyntax, bool> func)
        {
            foreach (var field in source.Members
                .OfType<FieldDeclarationSyntax>())
                foreach (var variable in field.Declaration.Variables)
                {
                    if (func(variable))
                        return variable;
                }
            return null;
        }

        public static IEnumerable<MemberDeclarationSyntax> RemoveFields(this ClassDeclarationSyntax source, IEnumerable<VariableDeclaratorSyntax> variables)
        {
            List<MemberDeclarationSyntax> result = new List<MemberDeclarationSyntax>();
            foreach (var member in source.Members)
            {
                if (!(member is FieldDeclarationSyntax field))
                {
                    result.Add(member);
                    continue;
                }

                SeparatedSyntaxList<VariableDeclaratorSyntax> newVariables = new SeparatedSyntaxList<VariableDeclaratorSyntax>();
                foreach (var variable in field.Declaration.Variables)
                {
                    if (!variables.Any(c => c.Identifier.ValueText == variable.Identifier.ValueText))
                        newVariables.Add(variable);
                }
                if (newVariables.Count == 0)
                    continue;
                result.Add(field.WithDeclaration(field.Declaration.WithVariables(newVariables)));
            }
            return result;
        }
    }

}
