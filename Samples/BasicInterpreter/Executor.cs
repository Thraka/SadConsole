// <copyright file="Executor.cs" company="Peter Ibbotson">
// (C) Copyright 2017 Peter Ibbotson - Modifications by Steve De George JR
// </copyright>

using System;
using System.Collections;
using ClassicBasic.Interpreter;

namespace BasicTerminal
{
    /// <summary>
    /// Executes the code.
    /// </summary>
    public class ExecutorFrameBased : IExecutor
    {
        private readonly IRunEnvironment _runEnvironment;
        private readonly IToken _letToken;
        private readonly IProgramRepository _programRepository;
        private readonly ITokeniser _tokeniser;

        public bool IsExecuting { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutorFrameBased"/> class.
        /// </summary>
        /// <param name="teletype">Teletype to use for input output.</param>
        /// <param name="runEnvironment">Run time environment.</param>
        /// <param name="programRepository">Program repository.</param>
        /// <param name="tokensProvider">Provider of tokens.</param>
        /// <param name="tokeniser">Tokeniser to pass to ITokeniserCommands.</param>
        public ExecutorFrameBased(
            ITeletype teletype,
            IRunEnvironment runEnvironment,
            IProgramRepository programRepository,
            ITokensProvider tokensProvider,
            ITokeniser tokeniser)
        {
            _runEnvironment = runEnvironment;
            _programRepository = programRepository;
            _tokeniser = tokeniser;
            _letToken = tokensProvider.Tokens.Find(t => t.Statement == TokenType.Let);
            teletype.CancelEventHandler += InterruptExecution;
        }

        /// <summary>
        /// Executes multiple lines.
        /// </summary>
        /// <returns>true if the user type SYSTEM.</returns>
        public bool ExecuteLine()
        {
            bool? returnValue = null;
            while (!returnValue.HasValue)
            {
                try
                {
                    returnValue = ExecuteLineWithoutErrorHandler();
                }
                catch (ClassicBasic.Interpreter.Exceptions.BasicException exception)
                {
                    _runEnvironment.LastErrorLine = _runEnvironment.ContinueLineNumber;
                    _runEnvironment.LastErrorNumber = exception.ErrorCode;

                    if (!_runEnvironment.OnErrorGotoLineNumber.HasValue)
                    {
                        throw;
                    }

                    _runEnvironment.OnErrorHandler(_programRepository);
                }
            }

            return returnValue.Value;
        }

        /// <summary>
        /// Executes multiple lines without calling the BASIC error handler.
        /// </summary>
        /// <returns>true if the user type SYSTEM.</returns>
        private bool ExecuteLineWithoutErrorHandler()
        {
            _runEnvironment.KeyboardBreak = false;

            if (_runEnvironment.CurrentLine.LineNumber.HasValue)
            {
                _runEnvironment.ContinueLineNumber = null;
            }

            if (!_runEnvironment.CurrentLine.EndOfLine)
            {
                if (ExecuteStatement())
                {
                    IsExecuting = false;
                    return true;
                }
            }

            // End of line get next line.
            if (_runEnvironment.CurrentLine.EndOfLine)
            {
                if (_runEnvironment.CurrentLine.LineNumber.HasValue)
                {
                    _runEnvironment.CurrentLine =
                        _programRepository.GetNextLine(_runEnvironment.CurrentLine.LineNumber.Value);
                    if (_runEnvironment.CurrentLine == null)
                    {
                        IsExecuting = false;
                        return false;
                    }
                }
                else
                {
                    IsExecuting = false;
                    // Immediate mode go home.
                    return false;
                }
            }

            // Not at the beginning of the Line?
            IToken token;
            if (_runEnvironment.CurrentLine.CurrentToken != 0)
            {
                // Next token should be a colon or an else
                token = _runEnvironment.CurrentLine.NextToken();
                if (token.Statement == TokenType.Else)
                {
                    _runEnvironment.CurrentLine.PushToken(token);
                }
                else if (token.Seperator != TokenType.Colon)
                {
                    throw new ClassicBasic.Interpreter.Exceptions.SyntaxErrorException();
                }
            }

            // Eat any colons.
            do
            {
                token = _runEnvironment.CurrentLine.NextToken();
            }
            while (token.Seperator == TokenType.Colon);

            _runEnvironment.CurrentLine.PushToken(token);
            IsExecuting = true;
            return false;
        }

        private bool ExecuteStatement()
        {
            // Set location for continue
            if (_runEnvironment.CurrentLine.LineNumber.HasValue)
            {
                _runEnvironment.ContinueLineNumber = _runEnvironment.CurrentLine.LineNumber;
                _runEnvironment.ContinueToken = _runEnvironment.CurrentLine.CurrentToken;
            }

            _runEnvironment.DataErrorLine = null;

            IToken token;
            do
            {
                token = _runEnvironment.CurrentLine.NextToken();

                // It's a colon go home.
                if (token.Seperator == TokenType.Colon)
                {
                    _runEnvironment.CurrentLine.PushToken(token);
                    return false;
                }

                // System command go home.
                if (token.Statement == TokenType.System)
                {
                    return true;
                }

                // Variable without a LET statment. So lets fake it.
                if (token.TokenClass == TokenClass.Variable)
                {
                    _runEnvironment.CurrentLine.PushToken(token);
                    token = _letToken;
                }

                ExecuteToken(token);

                // User hit break
                if (_runEnvironment.KeyboardBreak)
                {
                    IsExecuting = false;
                    throw new ClassicBasic.Interpreter.Exceptions.BreakException();
                }
            }
            while (token is IRepeatExecuteCommand && !_runEnvironment.CurrentLine.EndOfLine);
            return false;
        }

        private void ExecuteToken(IToken token)
        {
            var executor = token as ICommand;
            var interuptableExecutor = token as IInterruptableCommand;
            var tokeniserExecutor = token as ITokeniserCommand;

            if (executor == null && interuptableExecutor == null && tokeniserExecutor == null)
            {
                throw new ClassicBasic.Interpreter.Exceptions.SyntaxErrorException();
            }
            else
            {
                if (interuptableExecutor != null)
                {
                    interuptableExecutor.Setup();
                    while (!_runEnvironment.KeyboardBreak)
                    {
                        if (interuptableExecutor.Execute())
                        {
                            break;
                        }
                    }

                    _runEnvironment.KeyboardBreak = false;
                }
                else
                {
                    if (tokeniserExecutor != null)
                    {
                        tokeniserExecutor.Execute(_tokeniser);
                    }
                    else
                    {
                        executor.Execute();
                    }
                }
            }
        }

        public void Break()
        {
            IsExecuting = false;
        }

        private void InterruptExecution(object sender, ConsoleCancelEventArgs e)
        {
            _runEnvironment.KeyboardBreak = true;
            e.Cancel = true;
        }
    }
}
