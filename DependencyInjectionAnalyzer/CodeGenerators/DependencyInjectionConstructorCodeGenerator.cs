using DependencyInjectionAnalyzer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyInjectionAnalyzer.CodeGenerators
{
    public class DependencyInjectionConstructorCodeGenerator : CodeGeneratorBase<ConstructorDeclarationSyntax>
    {

        public DependencyInjectionConstructorCodeGenerator(SyntaxGenerator syntaxGenerator) : base(syntaxGenerator)
        {
        }

        public override CompilationUnitSyntax Generate(CompilationUnitSyntax root, ConstructorDeclarationSyntax node)
        {
            var @class = node.Parent as ClassDeclarationSyntax;

            var newConstructor = GenerateBody(node);
            var newClass = @class.ReplaceNode(node, newConstructor);

            newClass = GenerateFields(newConstructor, newClass);

            return root
                .ReplaceNode(@class, newClass);
        }

        private ClassDeclarationSyntax GenerateFields(ConstructorDeclarationSyntax constructor, ClassDeclarationSyntax @class)
        {

            var variables = GetRelevantFields();

            var baseFields = @class.RemoveFields(variables).ToList();

            var fieldsOrdered = variables
                .Select(c => syntaxGenerator.FieldDeclaration(c))
                .Cast<MemberDeclarationSyntax>();

            baseFields.InsertRange(0, fieldsOrdered);
            @class = @class.WithMembers(new SyntaxList<MemberDeclarationSyntax>(baseFields));

            return @class;

            IEnumerable<VariableDeclaratorSyntax> GetRelevantFields()
            {
                VariableDeclaratorSyntax[] result = new VariableDeclaratorSyntax[constructor.ParameterList.Parameters.Count];

                int index = 0;
                foreach (var param in constructor.ParameterList.Parameters)
                {

                    var item = @class.FindField(c => c.Identifier.ValueText == param.Identifier.ValueText);
                    if (item == null)
                    {
                        var declared = syntaxGenerator.FieldDeclaration(param.Identifier.ValueText, param.Type, Accessibility.Private, DeclarationModifiers.ReadOnly) as FieldDeclarationSyntax;
                        item = declared.Declaration.Variables.First(); //Only 1 variable inited with the field
                    }
                    result[index] = item;
                    index++;
                }
                return result;
            }
        }

        private ConstructorDeclarationSyntax GenerateBody(ConstructorDeclarationSyntax node)
        {
            var block = SyntaxFactory.Block(node.ParameterList.Parameters
                .Select(c => syntaxGenerator.MemberAssignStatement(c.Identifier.ValueText)));

            return node.WithBody(block);
        }


    }
}
