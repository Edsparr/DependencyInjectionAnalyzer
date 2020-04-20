using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DependencyInjectionAnalyzer.Actions
{
    internal class DefaultCodeAction : CodeAction
    {
        private readonly Func<CancellationToken, Task<Document>> generateDocument;
        private readonly string title;

        public DefaultCodeAction(string title, Func<CancellationToken, Task<Document>> generateDocument)
        {
            this.title = title;
            this.generateDocument = generateDocument;
        }

        public override string Title => title;

        protected override Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken) => generateDocument(cancellationToken);
    }
}
