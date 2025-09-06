using System;
using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public struct DisposableIndent : IDisposable
{
    private readonly IndentedTextWriter _codeWriter;
    private bool _isDisposed;

    public DisposableIndent(IndentedTextWriter codeWriter)
    {
        _isDisposed = false;
        _codeWriter = codeWriter;
        _codeWriter.Indent++;
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _codeWriter.Indent--;
        _isDisposed = true;
    }
}